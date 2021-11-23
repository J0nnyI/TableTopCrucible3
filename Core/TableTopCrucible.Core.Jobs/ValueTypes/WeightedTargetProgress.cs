using ValueOf;

namespace TableTopCrucible.Core.Jobs.ValueTypes
{
    public class WeightedTargetProgress : ValueOf<double, WeightedTargetProgress>
    {
        public static explicit operator WeightedTargetProgress(double value) => From(value);

        public static explicit operator TargetProgress(WeightedTargetProgress value) => (TargetProgress) value.Value;
    }
}