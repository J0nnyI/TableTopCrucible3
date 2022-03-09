using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableTopCrucible.Core.Helper
{
    public static class DateTimeHelper
    {
        public static bool IsWithinTimeframe(this DateTime timeA, DateTime timeB, TimeSpan timeframe)
            => timeA - timeframe >= timeB && timeA + timeframe <= timeB;
    }
}
