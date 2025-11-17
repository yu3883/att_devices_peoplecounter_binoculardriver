using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ATTSystems.Net.Devices.PeopleCounter.API
{
    [XmlRoot]
    public class Diags
    {
        [XmlElement("Properties")]
        public List<Properties> Properties { get; set; }
    }
}
