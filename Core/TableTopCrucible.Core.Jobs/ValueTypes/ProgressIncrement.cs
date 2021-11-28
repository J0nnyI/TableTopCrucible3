using ValueOf;

namespace TableTopCrucible.Core.Jobs.ValueTypes
{
    public class ProgressIncrement : ValueOf<double, ProgressIncrement>
    {
        public static explicit operator ProgressIncrement(double value) => From(value);
    }
}