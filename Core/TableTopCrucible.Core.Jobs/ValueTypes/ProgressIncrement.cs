using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Core.Jobs.ValueTypes
{
    public class ProgressIncrement : ValueType<double, ProgressIncrement>
    {
        public static explicit operator ProgressIncrement(double value) => From(value);
    }
}