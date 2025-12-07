using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ATTSystems.Net.Devices.PeopleCounter.API
{
    [XmlRoot("GenderCount")]
    public class GenderCount
    {
        [XmlAttribute("StartTime")]
        public string StartTime { get; set; }

        [XmlAttribute("EndTime")]
        public string EndTime { get; set; }

        [XmlAttribute("UnixStartTime")]
        public string UnixStartTime { get; set; }

        [XmlAttribute("maleNum")]
        public int MaleNum { get; set; }

        [XmlAttribute("femaleNum")]
        public int FemaleNum { get; set; }

        [XmlAttribute("unknownGenderNum")]
        public int UnknownGenderNum { get; set; }

        [XmlAttribute("Status")]
        public string Status { get; set; }
    }
}
