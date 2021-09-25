using ValueOf;

namespace TableTopCrucible.Core.Jobs.ValueTypes
{
    public class WeightedTrackingTarget : ValueOf<double, WeightedTrackingTarget>
    {
        public static explicit operator WeightedTrackingTarget(double value)
            => From(value);
        public static explicit operator TrackingTarget(WeightedTrackingTarget value)
            => (TrackingTarget)value.Value;
    }
}
