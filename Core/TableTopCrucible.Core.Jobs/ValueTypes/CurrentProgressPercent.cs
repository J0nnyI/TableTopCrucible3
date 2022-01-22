using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Jobs.ValueTypes
{
    // the current progress in percent (0.0 to 100.0)
    public class CurrentProgressPercent : ValueType<double, CurrentProgressPercent>
    {
        private static double _min;
        public static CurrentProgressPercent
            Min { get; }= new() { Value = _min }; // cant use from since it runs through the validator which uses these constants

        private static double _max;
        public static CurrentProgressPercent Max { get; }= new() { Value = _max };

        public static explicit operator CurrentProgressPercent(double value) => From(value);

        public static bool operator >(CurrentProgressPercent val1, CurrentProgressPercent val2) =>
            val1.Value > val2.Value;

        public static bool operator <(CurrentProgressPercent val1, CurrentProgressPercent val2) =>
            val1.Value < val2.Value;

        public static CurrentProgressPercent From(CurrentProgress current, TargetProgress target) =>
            From(current.Value / target.Value * 100);

        protected override double Sanitize(double value)
        {
            return
                Value < _min
                ? _min
                : Value > _max
                    ? _max
                    : value;
        }
    }
}