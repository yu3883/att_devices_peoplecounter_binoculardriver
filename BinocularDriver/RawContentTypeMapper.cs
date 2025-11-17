using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace ATTSystems.Net.Devices.PeopleCounter
{
    public class RawContentTypeMapper : WebContentTypeMapper
    {
        public override WebContentFormat GetMessageFormatForContentType(string contentType)
        {
            return WebContentFormat.Xml;

            //if (contentType.Contains("text/xml") || contentType.Contains("application/xml"))
            //{

            //    return WebContentFormat.Raw;

            //}

            //else
            //{

            //    return WebContentFormat.Default;

            //}
        }
    }
}
