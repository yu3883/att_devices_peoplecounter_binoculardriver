using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ATTSystems.Net.Devices.PeopleCounter.API
{
    [XmlRoot("Report")]
    public class Report
    {
        [XmlAttribute("Date")]
        public string Date { get; set; }
        [XmlElement("Object")]
        public List<Object> Object { get; set; }
    }
}
