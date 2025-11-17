using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ATTSystems.Net.Devices.PeopleCounter.API
{
    [XmlRoot("Properties")]
    public class Properties
    {
        [XmlElement("Version")]
        public string Version { get; set; }
        [XmlElement("TransmitTime")]
        public string TransmitTime { get; set; }
        [XmlElement("MacAddress")]
        public string MacAddress { get; set; }
        [XmlElement("IpAddress")]
        public string IpAddress { get; set; }
        [XmlElement("ConnectionType")]
        public string ConnectionType { get; set; }
        [XmlElement("WifiSSID")]
        public string WifiSSID { get; set; }
        [XmlElement("WifiPassword")]
        public string WifiPassword { get; set; }
        [XmlElement("IpAddressMethod")]
        public string IpAddressMethod { get; set; }
        [XmlElement("HostName")]
        public string HostName { get; set; }

        [XmlElement("Timezone")]
        public string Timezone { get; set; }
        [XmlElement("DST")]
        public string DST { get; set; }
        [XmlElement("HwPlatform")]
        public string HwPlatform { get; set; }
        [XmlElement("SerialNumber")]
        public string SerialNumber { get; set; }
        [XmlElement("DeviceType")]
        public string DeviceType { get; set; }
        [XmlElement("SwRelease")]
        public string SwRelease { get; set; }

        

    }
}
