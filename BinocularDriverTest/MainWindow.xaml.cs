using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Description;
using System.Windows.Threading;
namespace ATTSystems.Net.Devices.PeopleCounter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IPeopleCounterDriver driver;
        ControllerProperties deviceController = new ControllerProperties();
        public MainWindow()
        {
            InitializeComponent();

            deviceController.ServerIP = ServerIPTextBox.Text;
            deviceController.ServerPort = Convert.ToInt32(ServerPortTextBox.Text);

            driver = new BinocularDriver(deviceController);
            

            driver.CounterTransactionReceived += driver_CounterTransactionReceived;
            driver.DeviceDataReceived += driver_DeviceDataReceived;
            driver.DeviceDataSent += driver_DeviceDataSent;
            driver.ErrorReceived += driver_ErrorReceived;
            driver.InformationReceived += driver_InformationReceived;
            driver.Connected += driver_Connected;
            driver.Disconnected += driver_Disconnected;
            driver.DeviceOnline += driver_DeviceOnline;
            driver.DeviceOffline += driver_DeviceOffline;
            this.Closed += MainWindow_Closed;
        }

        void driver_DeviceOffline(object sender, PeopleCounterDevice e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)delegate
            {
                try
                {
                    WriteLog(String.Format("{0} is offline.", e.DeviceID));
                }
                catch (Exception ex)
                {
                    WriteLog(String.Format("Error : {0} -> {1}", "driver_DeviceOffline", ex.Message));
                }
            });
        }

        void driver_DeviceOnline(object sender, PeopleCounterDevice e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)delegate
            {
                try
                {
                    WriteLog(String.Format("{0} is online.", e.DeviceID));
                }
                catch (Exception ex)
                {
                    WriteLog(String.Format("Error : {0} -> {1}", "driver_DeviceOnline", ex.Message));
                }
            });
        }

        void driver_Disconnected(object sender, DeviceProperties e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)delegate
            {
                try
                {
                    ConnectDriverButton.IsEnabled = true;
                    DisconnectDriverButton.IsEnabled = false;
                }
                catch (Exception ex)
                {
                    WriteLog(String.Format("Error : {0} -> {1}","driver_Disconnected", ex.Message));
                }
            });
        }

        void driver_Connected(object sender, DeviceProperties e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)delegate
            {
                try
                {
                    ConnectDriverButton.IsEnabled = false;
                    DisconnectDriverButton.IsEnabled = true;
                }
                catch (Exception ex)
                {
                    WriteLog(String.Format("Error : {0} -> {1}", "driver_Connected", ex.Message));
                }
            });
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            try
            {
                if(driver!=null)
                {
                    if (driver.IsConnected)
                        driver.Disconnect();

                    driver.CounterTransactionReceived -= driver_CounterTransactionReceived;
                    driver.DeviceDataReceived -= driver_DeviceDataReceived;
                    driver.DeviceDataSent -= driver_DeviceDataSent;
                    driver.ErrorReceived -= driver_ErrorReceived;
                    driver.InformationReceived -= driver_InformationReceived;
                    driver.Connected -= driver_Connected;
                    driver.Disconnected -= driver_Disconnected;
                    driver = null;
                }
            }
            catch
            {
            }
            
        }
        void driver_InformationReceived(object sender, InformationEventArgs e)
        {
            WriteLog(String.Format("Information : {0} -> {1}", e.DeviceId.ToString(), e.Message));
        }

        void driver_ErrorReceived(object sender, ErrorEventArgs e)
        {
            WriteLog(String.Format("Error in Driver : {0} -> {1}",e.MessageString, e.Exp.Message));
        }

        void driver_DeviceDataSent(object sender, DataSentEventArgs e)
        {
            WriteLog(String.Format("Device Data Sent : {0}", e.MessageString));
        }

        void driver_DeviceDataReceived(object sender, DataReceivedEventArgs e)
        {
            WriteLog(String.Format("Device Data Received : {0}", e.MessageString));
        }

        void driver_CounterTransactionReceived(object sender, CounterTransaction e)
        {
            WriteLog(String.Format("Counter Transaction Received => Device ID : {6}, Transaction Received Time : {0}, Start Time : {1}, End Time : {2}, Enters : {3}, Exits : {4}, Total People : {5}",
                e.TransactionReceivedTime,e.StartTime,e.EndTime,e.Enters,e.Exits,e.TotalPeople,e.DeviceId));
        }


        private void ControllerSettingButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ConnectDriverButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                driver.Connect();
                WriteLog(String.Format("Device server host connection is opened."));
            }
            catch (Exception ex)
            {
                WriteLog(String.Format("Error in connecting controller : {0}", ex.Message));
            }
            
        }

        private void DisconnectDriverButton_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                driver.Disconnect();
                WriteLog(String.Format("Device server host connection is closed."));
            }
            catch (Exception ex)
            {
                WriteLog(String.Format("Error in disconnecting controller : {0}", ex.Message));
            }
            
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void WriteLog(string message)
        {
            message = DateTime.Now + " -> " + message;
            Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)delegate
            {
                int maxLines = 200;
                string crlf = "\r\n";

                if (logTextBox.Document.Blocks.Count == 0)
                {
                    logTextBox.Document.Blocks.Add(new Paragraph(new Run(message + crlf)));
                    return;
                }

                Paragraph p = logTextBox.Document.Blocks.FirstBlock as Paragraph;

                if (p.Inlines.Count == maxLines)
                {
                    p.Inlines.Remove(p.Inlines.FirstInline);
                }
                if (p.Inlines.LastInline == null)
                    p.Inlines.Add(new Run(message + crlf));
                else
                    p.Inlines.InsertAfter(p.Inlines.LastInline, new Run(message + crlf));
                logTextBox.ScrollToEnd();

            });

            

        }
    }
}
