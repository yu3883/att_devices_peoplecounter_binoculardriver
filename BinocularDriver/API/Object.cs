using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ATTSystems.Net.Devices.PeopleCounter.API
{
    [XmlRoot("Report")]
    public class Object
    {
        [XmlAttribute("Id")]
        public string Id { get; set; }
        [XmlAttribute("DeviceId")]
        public string DeviceId { get; set; }
        [XmlAttribute("DeviceName")]
        public string DeviceName { get; set; }
        [XmlAttribute("ObjectType")]
        public string ObjectType { get; set; }
        [XmlAttribute("Name")]
        public string Name { get; set; }
        [XmlElement("Count")]
        public List<Count> Count { get; set; }

        // NEW: Add support for v2.1 elements
        [XmlElement("GenderCount")]
        public List<GenderCount> GenderCount { get; set; }

        [XmlElement("AgeCount")]
        public List<AgeCount> AgeCount { get; set; }

        [XmlElement("WorkCardCount")]
        public List<WorkCardCount> WorkCardCount { get; set; }
    }
}
