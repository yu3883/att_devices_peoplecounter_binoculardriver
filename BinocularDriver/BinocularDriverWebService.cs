using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
namespace ATTSystems.Net.Devices.PeopleCounter
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "BinocularDriverWebService" in both code and config file together.
    public class BinocularDriverWebService : IBinocularDriverWebService
    {
        static public event EventHandler<API.Diags> HeartBeatRequestReceived;
        static public event EventHandler<API.RTMetrics> CounterTransactionRequestReceived;
        static public event EventHandler<Exception> ServiceErrorReceived;
        static public event EventHandler<string> DeviceDataRequestReceived;
        static public event EventHandler<string> DeviceDataSentResponse;
        public string GetInfo()
        {
            if (DeviceDataRequestReceived != null)
                DeviceDataRequestReceived(this, String.Format("Requesting the web service info."));
            return "This is Binocular Device Driver Webservice.";
        }

        public API.CounterTransactionResponse HeartBeatReceived(XElement request)
        {

            if (DeviceDataRequestReceived != null)
                DeviceDataRequestReceived(this, String.Format("Heart Beat Json Request received : {0}", request.ToString()));

            API.CounterTransactionResponse response = new API.CounterTransactionResponse();
            try
            {

                API.Diags value;
                var serializer = new XmlSerializer(typeof(API.Diags));
                value = (API.Diags)serializer.Deserialize(request.CreateReader());

                if (HeartBeatRequestReceived != null)
                    HeartBeatRequestReceived(this, value);

                response.code = "0";
                response.message = "success";

                if (DeviceDataSentResponse != null)
                    DeviceDataSentResponse(this, JsonConvert.SerializeObject(response));

            }
            catch (Exception ex)
            {
                response.code = API.CounterTransactionResponseCode.Failed.ToString();
                response.message = ex.Message;

                if (ServiceErrorReceived != null)
                    ServiceErrorReceived(this, ex);
            }

            return response;

        }
        public API.CounterTransactionResponse CounterTransactionReceived(XElement request)
        {
            API.CounterTransactionResponse response = new API.CounterTransactionResponse();
            try
            {
                bool needToResponse = true;

                if (DeviceDataRequestReceived != null)
                    DeviceDataRequestReceived(this, String.Format("Upload Data Json Request received : {0}", request.ToString()));

                API.RTMetrics rtTransaction;

                if (request.Name.LocalName == "Metrics")
                {
                    var serializerOld = new XmlSerializer(typeof(API.Metrics));
                    var metricsOld = (API.Metrics)serializerOld.Deserialize(request.CreateReader());

                    // Convert to RTMetrics
                    rtTransaction = new API.RTMetrics
                    {
                        SiteId = metricsOld.SiteId,
                        SiteName = metricsOld.SiteName,
                        DeviceId = metricsOld.DeviceId,
                        DeviceName = metricsOld.DeviceName,
                        Properties = metricsOld.Properties,
                        ReportData = metricsOld.ReportData
                    };
                    needToResponse = false;

                }
                else
                {
                    // New version with <RTMetrics> root
                    var serializer = new XmlSerializer(typeof(API.RTMetrics));
                    rtTransaction = (API.RTMetrics)serializer.Deserialize(request.CreateReader());
                    
                }

                if (CounterTransactionRequestReceived != null)
                    CounterTransactionRequestReceived(this, rtTransaction);


                if (needToResponse == false)
                {
                    if (DeviceDataSentResponse != null)
                        DeviceDataSentResponse(this, "response only status 200 with no XML body");

                    return null;
                    // WCF will automatically return HTTP 200 with no body when method returns void
                    //WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.OK;
                }

                response.code = "0";
                response.message = "success";

                if (DeviceDataSentResponse != null)
                    DeviceDataSentResponse(this, JsonConvert.SerializeObject(response));

                
                

            }
            catch (Exception ex)
            {
                response.code = API.CounterTransactionResponseCode.Failed.ToString();
                response.message = ex.Message;

                if (ServiceErrorReceived != null)
                    ServiceErrorReceived(this, ex);
            }
            return response;
        }

        

        #region Testing Code
        //public XElement DoWork(XElement xml)
        //{
        //    //XElement request = XElement.Load(xml);
        //    //API.Metrics transaction;
        //    //var serializer = new XmlSerializer(typeof(API.Metrics));
        //    //transaction = (API.Metrics)serializer.Deserialize(request.CreateReader());

        //    //StreamReader reader = null;
        //    //XDocument xDocRequest = null;
        //    //string strXmlRequest = string.Empty;
        //    //reader = new StreamReader(xml);
        //    //strXmlRequest = reader.ReadToEnd();
        //    //xDocRequest = XDocument.Parse(strXmlRequest);
        //    string response = "<Result>OK</Result>";
        //    return XElement.Parse(response);


        //}
        //public API.CounterTransactionResponse CounterTransactionReceived()
        //{
        //    try
        //    {
        //        var messageContent = Encoding.UTF8.GetString(OperationContext.Current.RequestContext.RequestMessage.GetBody<byte[]>());

        //        //var messageInspector = new MessageInspector();

        //        //// Where do request & client channel come from?
        //        //var values = messageInspector.AfterReceiveRequest(ref request, clientChannel, OperationContext.Current.InstanceContext);            


        //        //var x = Encoding.UTF8.GetString(OperationContext.Current.RequestContext
        //        //           .RequestMessage.GetBody<byte[]>());

        //        //var url = OperationContext.Current.RequestContext.RequestMessage.Headers.To.LocalPath;
        //        //var sb = new StringBuilder("Request: " + url + Environment.NewLine);




        //        //var messageContent = Encoding.UTF8.GetString(OperationContext.Current.RequestContext.RequestMessage.GetBody<byte[]>());

        //        //API.Metrics dataObject = OperationContext.Current.RequestContext.RequestMessage.GetBody<API.Metrics>(new DataContractJsonSerializer(typeof(API.Metrics)));
        //        //Console.WriteLine(dataObject.categoryid);



        //        var r = OperationContext.Current.RequestContext.RequestMessage.GetReaderAtBodyContents();


        //        var temp = OperationContext.Current.RequestContext.RequestMessage;
        //        //API.Metrics a = OperationContext.Current.RequestContext.RequestMessage.GetBody<API.Metrics>();
        //        //var reader = OperationContext.Current.RequestContext.RequestMessage.GetReaderAtBodyContents();


        //        //reader.ReadOuterXml();


        //        //byte[] requestObject = OperationContext.Current.RequestContext.RequestMessage.GetBody<byte[]>();
        //        //byte[] requestBinary = OperationContext.Current.RequestContext.RequestMessage.GetBody<byte[]>();
        //        string requestString = OperationContext.Current.RequestContext.RequestMessage.ToString();

        //        if (DeviceDataRequestReceived != null)
        //            DeviceDataRequestReceived(this, String.Format("Upload Data Json Request received : {0}", requestString));



        //        API.CounterTransactionResponse response = new API.CounterTransactionResponse();
        //        try
        //        {

        //            XElement request = XElement.Parse(requestString);

        //            API.Metrics transaction;
        //            var serializer = new XmlSerializer(typeof(API.Metrics));
        //            transaction = (API.Metrics)serializer.Deserialize(request.CreateReader());

        //            if (CounterTransactionRequestReceived != null)
        //                CounterTransactionRequestReceived(this, transaction);

        //            response.code = "0";
        //            response.message = "success";

        //            if (DeviceDataSentResponse != null)
        //                DeviceDataSentResponse(this, JsonConvert.SerializeObject(response));

        //        }
        //        catch (Exception ex)
        //        {
        //            response.code = API.CounterTransactionResponseCode.Failed.ToString();
        //            response.message = ex.Message;
        //        }

        //        return response;
        //    }
        //    catch (XmlException xmlEx)
        //    {
        //        throw (xmlEx);
        //    }
        //    catch (SerializationException serEx)
        //    {
        //        throw (serEx);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}
        #endregion
        
    }
}
