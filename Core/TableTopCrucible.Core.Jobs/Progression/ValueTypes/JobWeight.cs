using ValueOf;

namespace TableTopCrucible.Core.Jobs.Progression.ValueTypes
{
    public class JobWeight : ValueOf<double, JobWeight>
    {
        public static explicit operator JobWeight(double value)
            => From(value);

        public static WeightedTargetProgress operator *(TargetProgress targetProgress, JobWeight weight)
            => (WeightedTargetProgress)(targetProgress.Value * weight.Value);

        public static JobWeight Default { get; } = From(1);
    }
}
