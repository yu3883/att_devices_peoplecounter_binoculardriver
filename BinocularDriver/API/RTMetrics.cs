using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ATTSystems.Net.Devices.PeopleCounter.API
{
    [XmlRoot]
    public class RTMetrics
    {
        [XmlAttribute("SiteId")]
        public string SiteId { get; set; }
        [XmlAttribute("SiteName")]
        public string SiteName { get; set; }
        [XmlAttribute("DeviceId")]
        public string DeviceId { get; set; }
        [XmlAttribute("DeviceName")]
        public string DeviceName { get; set; }
        [XmlElement("Properties")]
        public List<Properties> Properties { get; set; }
        [XmlElement("ReportData")]
        public List<ReportData> ReportData { get; set; }
    }
}
