using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableTopCrucible.Core.Helper
{
    public static class MathHelper
    {
        public static int FloorInt(double value)
            => Convert.ToInt32(Math.Floor(value));
        public static int CeilingInt(double value)
            => Convert.ToInt32(Math.Ceiling(value));
    }
}
