using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ATTSystems.Net.Devices.PeopleCounter.API
{
    [XmlRoot("Count")]
    public class Count
    {
        [XmlAttribute("StartTime")]
        public string StartTime { get; set; }
        [XmlAttribute("EndTime")]
        public string EndTime { get; set; }
        [XmlAttribute("UnixStartTime")]
        public string UnixStartTime { get; set; }
        [XmlAttribute("Enters")]
        public int Enters { get; set; }
        [XmlAttribute("Exits")]
        public int Exits { get; set; }
        [XmlAttribute("Passings")]
        public int Passings { get; set; }
        [XmlAttribute("Returns")]
        public int Returns { get; set; }
        [XmlAttribute("Status")]
        public string Status { get; set; }

    }
}
