using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ATTSystems.Net.Devices.PeopleCounter.API
{
    [XmlRoot("ReportData")]
    public class ReportData
    {
        [XmlAttribute("Interval")]
        public string Interval { get; set; }
        [XmlElement("Report")]
        public List<Report> Report { get; set; }
    }
}
