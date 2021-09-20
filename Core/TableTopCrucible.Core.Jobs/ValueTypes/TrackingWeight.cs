using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValueOf;

namespace TableTopCrucible.Core.Jobs.ValueTypes
{
    public class TrackingWeight:ValueOf<double, TrackingWeight>
    {
        public static explicit operator TrackingWeight(double value)
            => From(value);

        public static WeightedTrackingTarget operator *(TrackingTarget trackingTarget, TrackingWeight weight)
            => (WeightedTrackingTarget) (trackingTarget.Value * weight.Value);
    }
}
