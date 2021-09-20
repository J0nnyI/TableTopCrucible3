using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.CompilerServices;
using ValueOf;

namespace TableTopCrucible.Core.Jobs.ValueTypes
{
    public class CurrentProgress:ValueOf<double, CurrentProgress>
    {
        public static explicit operator CurrentProgress(double value)
            => From(value);

        public static CurrentProgress operator +(CurrentProgress current, ProgressIncrement increment)
            => (CurrentProgress) (current.Value + increment.Value);

        public static WeightedCurrentProgress operator *(CurrentProgress current, TrackingWeight weight)
            => (WeightedCurrentProgress)(current.Value * weight.Value);
        public static explicit operator CurrentProgress (WeightedCurrentProgress current)
            => (CurrentProgress)current.Value;
    }
}
