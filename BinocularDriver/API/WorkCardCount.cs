using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
namespace ATTSystems.Net.Devices.PeopleCounter.API
{
    [XmlRoot("WorkCardCount")]
    public class WorkCardCount
    {
        [XmlAttribute("StartTime")]
        public string StartTime { get; set; }

        [XmlAttribute("EndTime")]
        public string EndTime { get; set; }

        [XmlAttribute("UnixStartTime")]
        public string UnixStartTime { get; set; }

        [XmlAttribute("workCardNum")]
        public int WorkCardNum { get; set; }

        [XmlAttribute("Status")]
        public string Status { get; set; }
    }
}
