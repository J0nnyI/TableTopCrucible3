using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Core.Jobs.ValueTypes
{
    public class JobWeight : ValueType<double, JobWeight>
    {
        public static JobWeight Default { get; } = From(1);

        public static explicit operator JobWeight(double value) => From(value);

        public static WeightedTargetProgress operator *(TargetProgress targetProgress, JobWeight weight) =>
            (WeightedTargetProgress)(targetProgress.Value * weight.Value);
    }
}