using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATTSystems.Net.Devices.PeopleCounter.API
{
    public class CounterTransactionResponse:BasicResponse
    {
    }
    public enum CounterTransactionResponseCode
    {
        success=0,
        SNDoesNotExist=1,
        Failed=2
    }
}
