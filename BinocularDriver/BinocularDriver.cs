using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ATTSystems.Net.Devices.PeopleCounter
{
    public class BinocularDriver:IPeopleCounterDriver,IDisposable
    {

        public event EventHandler<DataReceivedEventArgs> DeviceDataReceived;
        public event EventHandler<DataSentEventArgs> DeviceDataSent;
        public event EventHandler<ErrorEventArgs> ErrorReceived;
        public event EventHandler<InformationEventArgs> InformationReceived;
        public event EventHandler<DeviceProperties> Connected;
        public event EventHandler<DeviceProperties> Disconnected;
        public event EventHandler<PeopleCounterDevice> DeviceOffline;
        public event EventHandler<PeopleCounterDevice> DeviceOnline;

        ControllerProperties controller;

        List<PeopleCounterDevice> ActiveDeviceList = new List<PeopleCounterDevice>();

        private int deviceAliveTimeInSecond = 60;  //5 minutes
        System.Timers.Timer deviceCheckTimer;
        public ControllerProperties Controller
        {
            get
            {
                return controller;
            }
            set
            {
                controller=value;
            }
        }

        public event EventHandler<CounterTransaction> CounterTransactionReceived;

        ServiceHost deviceServerHost;

        List<ControllerProperties> controllerList = new List<ControllerProperties>();
        public BinocularDriver(ControllerProperties _controller)
        {
            controller = _controller;
            BinocularDriverWebService.CounterTransactionRequestReceived += service_CounterTransactionRequestReceived;
            BinocularDriverWebService.HeartBeatRequestReceived += BinocularDriverWebService_HeartBeatRequestReceived;
            BinocularDriverWebService.ServiceErrorReceived += BinocularDriverWebService_ServerErrorReceived;
            BinocularDriverWebService.DeviceDataRequestReceived += BinocularDriverWebService_DeviceDataRequestReceived;
            BinocularDriverWebService.DeviceDataSentResponse += BinocularDriverWebService_DeviceDataSentResponse;

            deviceCheckTimer = new System.Timers.Timer();
            deviceCheckTimer.Interval = 1000;
            deviceCheckTimer.Elapsed += deviceCheckTimer_Elapsed;
            deviceCheckTimer.Start();
        }

        void deviceCheckTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                deviceCheckTimer.Stop();

                List<PeopleCounterDevice> offlineDevices = new List<PeopleCounterDevice>();
                foreach(PeopleCounterDevice active in ActiveDeviceList)
                {
                    if((DateTime.Now - active.LastActiveTime).TotalSeconds>deviceAliveTimeInSecond)
                    {
                        if (DeviceOffline != null)
                            DeviceOffline(this, active);

                        offlineDevices.Add(active);
                    }
                }
                foreach (PeopleCounterDevice offlineDevice in offlineDevices)
                {
                    ActiveDeviceList.Remove(offlineDevice);
                }
            }
            catch (Exception ex)
            {
                if (ErrorReceived != null)
                    ErrorReceived(this, new ErrorEventArgs("deviceCheckTimer_Elapsed", ex));
            }
            finally
            {
                deviceCheckTimer.Start();
            }
        }

        
        public void Dispose()
        {
            BinocularDriverWebService.CounterTransactionRequestReceived -= service_CounterTransactionRequestReceived;
            BinocularDriverWebService.HeartBeatRequestReceived -= BinocularDriverWebService_HeartBeatRequestReceived;
            BinocularDriverWebService.ServiceErrorReceived -= BinocularDriverWebService_ServerErrorReceived;
            BinocularDriverWebService.DeviceDataRequestReceived -= BinocularDriverWebService_DeviceDataRequestReceived;
            BinocularDriverWebService.DeviceDataSentResponse -= BinocularDriverWebService_DeviceDataSentResponse;

            deviceCheckTimer.Stop();
            deviceCheckTimer = null;
        }

        

        void BinocularDriverWebService_DeviceDataSentResponse(object sender, string e)
        {
            try
            {
                if (DeviceDataSent != null)
                {
                    DeviceDataReceived(this, new DataReceivedEventArgs(controller.Id, e));
                }

            }
            catch(Exception ex)
            {
                if (ErrorReceived != null)
                    ErrorReceived(this, new ErrorEventArgs("BinocularDriverWebService_DeviceDataSentResponse", ex));
            }
        }

        void BinocularDriverWebService_DeviceDataRequestReceived(object sender, string e)
        {
            try
            {
                if (DeviceDataReceived != null)
                {
                    DeviceDataReceived(this, new DataReceivedEventArgs(controller.Id, e));
                }
                    
            }
            catch (Exception ex)
            {
                if (ErrorReceived != null)
                    ErrorReceived(this, new ErrorEventArgs("BinocularDriverWebService_DeviceDataRequestReceived", ex));
            }
        }
        void BinocularDriverWebService_ServerErrorReceived(object sender, Exception e)
        {
            try
            {
                if (ErrorReceived != null)
                    ErrorReceived(this, new ErrorEventArgs("BinocularDriverWebService_ServerErrorReceived", e));
            }
            catch 
            {
                
            }
            
        }
        void BinocularDriverWebService_HeartBeatRequestReceived(object sender, API.Diags e)
        {
            try
            {
                Thread thread = new Thread(delegate()
                {
                    
                    CheckActiveDevice(e.Properties.First().SerialNumber);
                });
                thread.Start();

            }
            catch (Exception)
            {
                throw;
            }
        }

        private void CheckActiveDevice(string deviceID)
        {
            try
            {
                PeopleCounterDevice activeDevice = ActiveDeviceList.FirstOrDefault(v => v.DeviceID == deviceID);

                if (activeDevice == null)
                {
                    PeopleCounterDevice newActiveDevice = new PeopleCounterDevice();
                    newActiveDevice.DeviceID = deviceID;
                    newActiveDevice.LastActiveTime = DateTime.Now;
                    ActiveDeviceList.Add(newActiveDevice);

                    if (DeviceOnline != null)
                    {
                        DeviceOnline(this, newActiveDevice);
                    }

                }
                else
                    activeDevice.LastActiveTime = DateTime.Now;
            }
            catch 
            {

                throw;
            }
        }
        void service_CounterTransactionRequestReceived(object sender, API.RTMetrics e)
        {
            try
            {
                Thread thread = new Thread(delegate()
                {
                    CheckActiveDevice(e.Properties.First().SerialNumber);

                    if (CounterTransactionReceived != null)
                    {
                        
                        API.ReportData reportData = e.ReportData.FirstOrDefault();
                        if (reportData == null) return;

                        API.Report report = reportData.Report.FirstOrDefault();
                        if (report == null) return;


                        // Process each Object (v2.1 has multiple objects with different ObjectTypes)
                        foreach (API.Object xmlObject in report.Object)
                        {
                            // Handle standard Count data (ObjectType 0, 1, 2)
                            if (xmlObject.Count != null && xmlObject.Count.Any())
                            {
                                IEnumerable<IGrouping<string, API.Count>> transactionGroupList =
                                    xmlObject.Count.GroupBy(v => v.StartTime);

                                foreach (IGrouping<string, API.Count> transactionGroup in transactionGroupList)
                                {
                                    CounterTransaction transaction = new CounterTransaction();
                                    transaction.DeviceId = e.DeviceId;
                                    transaction.TransactionId = e.Properties.First().TransmitTime;
                                    transaction.TransactionDevice = controller;
                                    DateTime reportDate = Convert.ToDateTime(report.Date);

                                    transaction.StartTime = reportDate.Date.Add(Convert.ToDateTime(transactionGroup.Last().StartTime).TimeOfDay);
                                    transaction.EndTime = reportDate.Date.Add(Convert.ToDateTime(transactionGroup.Last().EndTime).TimeOfDay);
                                    transaction.TransactionReceivedTime = transaction.StartTime;

                                    transaction.Enters = transactionGroup.Last().Enters;
                                    transaction.Exits = transactionGroup.Last().Exits;
                                    transaction.Returns = transactionGroup.Last().Returns;
                                    transaction.Passings = transactionGroup.Last().Passings;
                                    transaction.TotalPeople = transaction.Enters - transaction.Exits;

                                    // NEW: Add ObjectType to identify data type
                                    // You may need to add this property to CounterTransaction class
                                    // transaction.ObjectType = xmlObject.ObjectType;

                                    CounterTransactionReceived(this, transaction);
                                }
                            }

                            // NEW: Handle demographic data (ObjectType 3)
                            if (xmlObject.ObjectType == "3")
                            {
                                // Handle GenderCount data
                                if (xmlObject.GenderCount != null && xmlObject.GenderCount.Any())
                                {
                                    // Process gender data
                                    // You'll need to create appropriate events/handlers for this data
                                }

                                // Handle AgeCount data
                                if (xmlObject.AgeCount != null && xmlObject.AgeCount.Any())
                                {
                                    // Process age data
                                    // You'll need to create appropriate events/handlers for this data
                                }

                                // Handle WorkCardCount data
                                if (xmlObject.WorkCardCount != null && xmlObject.WorkCardCount.Any())
                                {
                                    // Process employee card data
                                    // You'll need to create appropriate events/handlers for this data
                                }
                            }
                        }

                        #region Old version before change to fix the impact of new API v2.1

                        //API.Object xmlObject = report.Object.FirstOrDefault();
                        //if (xmlObject == null) return;

                        //IEnumerable<IGrouping<string, API.Count>> transactionGroupList = xmlObject.Count.GroupBy(v => v.StartTime);

                        //foreach (IGrouping<string, API.Count> transactionGroup in transactionGroupList)
                        //{
                        //    //API.Count count = xmlObject.Count.FirstOrDefault();
                        //    //if (count == null) return;
                        //    CounterTransaction transaction = new CounterTransaction();

                        //    transaction.DeviceId = e.DeviceId;
                        //    transaction.TransactionId = e.Properties.First().TransmitTime;
                        //    transaction.TransactionDevice = controller;
                        //    DateTime reportDate = Convert.ToDateTime(report.Date);

                        //    transaction.StartTime = reportDate.Date.Add(Convert.ToDateTime(transactionGroup.Last().StartTime).TimeOfDay);
                        //    transaction.EndTime = reportDate.Date.Add(Convert.ToDateTime(transactionGroup.Last().StartTime).TimeOfDay);
                        //    transaction.TransactionReceivedTime = transaction.StartTime;

                        //    transaction.Enters = transactionGroup.Last().Enters;
                        //    transaction.Exits = transactionGroup.Last().Exits;
                        //    transaction.Returns = transactionGroup.Last().Returns;
                        //    transaction.Passings = transactionGroup.Last().Passings;
                        //    transaction.TotalPeople = transaction.Enters - transaction.Exits;

                        //    CounterTransactionReceived(this, transaction);
                        //}
                        #endregion
                        
                        #region Cumulative Scenario
                        //foreach(API.Count count in xmlObject.Count)
                        //{
                        //    //API.Count count = xmlObject.Count.FirstOrDefault();
                        //    //if (count == null) return;

                        //    transaction.DeviceId = e.DeviceId;
                        //    transaction.TransactionDevice = controller;
                        //    DateTime reportDate = Convert.ToDateTime(report.Date);

                        //    transaction.StartTime = reportDate.Date.Add(Convert.ToDateTime(count.StartTime).TimeOfDay);
                        //    transaction.EndTime = reportDate.Date.Add(Convert.ToDateTime(count.StartTime).TimeOfDay);
                        //    transaction.TransactionReceivedTime = transaction.StartTime;

                        //    transaction.Enters = count.Enters;
                        //    transaction.Exits = count.Exits;
                        //    transaction.Returns = count.Returns;
                        //    transaction.Passings = count.Passings;
                        //    transaction.TotalPeople = transaction.Enters - transaction.Exits;

                        //    CounterTransactionReceived(this, transaction);
                        //}
                        #endregion

                    }
                });
                thread.Start();

            }
            catch (Exception)
            {
                throw;
            }
        }

        private void StartServerHost()
        {
            if (deviceServerHost != null)
                return;

            // Step 1: Create a URI to serve as the base address.
            Uri baseAddress = new Uri(String.Format("http://{0}:{1}/BinocularWebService", controller.ServerIP, controller.ServerPort));

            // Step 2: Create a ServiceHost instance.
            deviceServerHost = new ServiceHost(typeof(BinocularDriverWebService), baseAddress);
            
            try
            {
                // Step 3: Add a service endpoint.
                deviceServerHost.AddServiceEndpoint(typeof(IBinocularDriverWebService), new WSHttpBinding(), "PeopleCountingWebService");

                // Step 4: Enable metadata exchange.
                //ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                //smb.HttpGetEnabled = true;
                //deviceServerHost.Description.Behaviors.Add(smb);


                // Step 5: Start the service.
                deviceServerHost.Open();
                

            }
            catch (CommunicationException ce)
            {
                deviceServerHost.Abort();
                deviceServerHost=null;
                throw ce;
            }
        }

        void Current_OperationCompleted(object sender, EventArgs e)
        {
            
        }

        private void StopServerHost()
        {
            if (deviceServerHost != null)
            {
                deviceServerHost.Close();
                deviceServerHost = null;
            }
                
        }
        public void Connect()
        {
            try
            {
                StartServerHost();
                if (Connected != null)
                    Connected(this, controller);
            }
            catch
            {
                throw;
            }
        }

        public void Disconnect()
        {
            try
            {
                StopServerHost();
                if (Disconnected != null)
                    Disconnected(this, controller);
            }
            catch 
            {
                
                throw;
            }
        }

        public bool IsConnected
        {
            get { return !(deviceServerHost == null); }
        }

        public void Restart()
        {
            StopServerHost();
            StartServerHost();
        }


        
    }
}
