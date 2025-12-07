using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
namespace ATTSystems.Net.Devices.PeopleCounter.API
{
    [XmlRoot("AgeCount")]
    public class AgeCount
    {
        [XmlAttribute("StartTime")]
        public string StartTime { get; set; }

        [XmlAttribute("EndTime")]
        public string EndTime { get; set; }

        [XmlAttribute("UnixStartTime")]
        public string UnixStartTime { get; set; }

        [XmlAttribute("ageLess10Num")]
        public int AgeLess10Num { get; set; }

        [XmlAttribute("age10to16Num")]
        public int Age10to16Num { get; set; }

        [XmlAttribute("age17to30Num")]
        public int Age17to30Num { get; set; }

        [XmlAttribute("age31to45Num")]
        public int Age31to45Num { get; set; }

        [XmlAttribute("age46to60Num")]
        public int Age46to60Num { get; set; }

        [XmlAttribute("ageOver60Num")]
        public int AgeOver60Num { get; set; }

        [XmlAttribute("unknownAgeNum")]
        public int UnknownAgeNum { get; set; }

        [XmlAttribute("Status")]
        public string Status { get; set; }
    }
}
