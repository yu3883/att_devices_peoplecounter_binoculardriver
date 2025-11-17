using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATTSystems.Net.Devices.PeopleCounter.API
{
    public class CounterTransactionRequest:BasicRequest
    {
        
        public string sn { get; set; }
        public long startTime { get; set; }
        public long endTime { get; set; }
        public DateTime time { get; set; }

        public int enters { get; set; }
        public int exits { get; set; }


    }
}
