using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Core.Jobs.ValueTypes
{
    public class TargetProgress : ValueType<double, TargetProgress>
    {
        public static explicit operator TargetProgress(double value) => From(value);
    }
}