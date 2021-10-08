using ValueOf;

namespace TableTopCrucible.Core.Jobs.Progression.ValueTypes
{
    public class TargetProgress : ValueOf<double, TargetProgress>
    {
        public static explicit operator TargetProgress(double value) => From(value);
    }
}