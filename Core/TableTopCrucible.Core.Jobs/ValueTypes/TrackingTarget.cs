using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValueOf;

namespace TableTopCrucible.Core.Jobs.ValueTypes
{
    public class TrackingTarget:ValueOf<double,TrackingTarget>
    {
        public static explicit operator TrackingTarget(double value)
            => From(value);
    }
}
