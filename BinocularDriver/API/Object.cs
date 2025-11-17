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


    }
}
