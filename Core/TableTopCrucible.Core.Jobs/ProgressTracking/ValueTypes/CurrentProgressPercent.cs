
using ValueOf;

namespace TableTopCrucible.Core.Jobs.ProgressTracking.ValueTypes
{
    // the current progress in percent (0.0 to 100.0)
    public class CurrentProgressPercent : ValueOf<double, CurrentProgressPercent>
    {
        public static CurrentProgressPercent Min = new() { Value = 0 };// cant use from since it runs through the validator which uses these constants
        public static CurrentProgressPercent Max = new() { Value = 100 };

        public static explicit operator CurrentProgressPercent(double value)
            => From(value);

        public static CurrentProgressPercent From(CurrentProgress current, TrackingTarget target)
            => From(current.Value / target.Value * 100);

        protected override void Validate()
        {
            if (Value < Min.Value)
                Value = Min.Value;
            if (Value > Max.Value)
                Value = Max.Value;
        }
    }
}
