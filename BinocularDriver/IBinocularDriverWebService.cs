using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;
using System.Xml.Linq;
using System.IO;
namespace ATTSystems.Net.Devices.PeopleCounter
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IBinocularDriverWebService" in both code and config file together.
    
    [ServiceContract]
    public interface IBinocularDriverWebService
    {
        

        [OperationContract]
        [WebGet(UriTemplate = "/api/service/info",
                ResponseFormat = WebMessageFormat.Json)]
        string GetInfo();

        [OperationContract]
        [WebInvoke
       (Method = "POST",
        BodyStyle =WebMessageBodyStyle.Bare,
       UriTemplate = "/api/device/heartBeat")]
        API.CounterTransactionResponse HeartBeatReceived(XElement request);

        
       [WebInvoke
      (Method = "POST",
      UriTemplate = "/api/device/uploadData",
      RequestFormat=WebMessageFormat.Xml,
      BodyStyle = WebMessageBodyStyle.Bare)]
        API.CounterTransactionResponse CounterTransactionReceived(XElement request);

        #region Test Code
        //[OperationContract]
        //[WebInvoke(Method = "POST", UriTemplate = "Push",
        //       RequestFormat = WebMessageFormat.Xml,
        //       ResponseFormat = WebMessageFormat.Xml,
        //       BodyStyle = WebMessageBodyStyle.Bare)]
        //XElement DoWork(XElement xml);
        #endregion
        
    }
}
