using ValueOf;

namespace TableTopCrucible.Core.Jobs.ValueTypes
{
    public class TrackingWeight : ValueOf<double, TrackingWeight>
    {
        public static explicit operator TrackingWeight(double value)
            => From(value);

        public static WeightedTrackingTarget operator *(TrackingTarget trackingTarget, TrackingWeight weight)
            => (WeightedTrackingTarget)(trackingTarget.Value * weight.Value);

        public static TrackingWeight Default { get; } = From(1);
    }
}
