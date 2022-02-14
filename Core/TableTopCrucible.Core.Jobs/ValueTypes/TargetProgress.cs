using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Jobs.ValueTypes;

public class TargetProgress : ValueType<double, TargetProgress>
{
    public static explicit operator TargetProgress(double value) => From(value);
}