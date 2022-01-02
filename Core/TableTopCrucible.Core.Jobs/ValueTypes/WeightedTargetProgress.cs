using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Core.Jobs.ValueTypes
{
    public class WeightedTargetProgress : ValueType<double, WeightedTargetProgress>
    {
        public static explicit operator WeightedTargetProgress(double value) => From(value);

        public static explicit operator TargetProgress(WeightedTargetProgress value) => (TargetProgress)value.Value;
    }
}