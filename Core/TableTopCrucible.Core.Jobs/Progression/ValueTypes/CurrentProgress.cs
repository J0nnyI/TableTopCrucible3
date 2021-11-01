using ValueOf;

namespace TableTopCrucible.Core.Jobs.Progression.ValueTypes
{
    public class CurrentProgress : ValueOf<double, CurrentProgress>
    {
        public static explicit operator CurrentProgress(double value) => From(value);

        public static CurrentProgress operator +(CurrentProgress current, ProgressIncrement increment) =>
            (CurrentProgress) ((current?.Value ?? 0) + (increment?.Value ?? 0));

        public static WeightedCurrentProgress operator *(CurrentProgress current, JobWeight weight) =>
            (WeightedCurrentProgress) ((current?.Value ?? 0) * (weight?.Value ?? 0));

        public static explicit operator CurrentProgress(WeightedCurrentProgress current) =>
            (CurrentProgress) current.Value;

        public static explicit operator CurrentProgress(TargetProgress current) => (CurrentProgress) current.Value;

        public static bool operator ==(CurrentProgress cur, TargetProgress target) =>
            (cur?.Value ?? 0).Equals(target?.Value ?? 0);

        public static bool operator !=(CurrentProgress cur, TargetProgress target) =>
            !(cur?.Value ?? 0).Equals(target?.Value ?? 0);

        public override bool Equals(object obj)
        {
            if (obj is CurrentProgress other)
                return this == other;
            return false;
        }

        public override int GetHashCode()
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            => Value.GetHashCode();

        public static bool operator >(CurrentProgress cur, TargetProgress target) =>
            (cur?.Value ?? 0) > (target?.Value ?? 0);

        public static bool operator <(CurrentProgress cur, TargetProgress target) =>
            (cur?.Value ?? 0) < (target?.Value ?? 0);

        public static bool operator >=(CurrentProgress cur, TargetProgress target) =>
            (cur?.Value ?? 0) >= (target?.Value ?? 0);

        public static bool operator <=(CurrentProgress cur, TargetProgress target) =>
            (cur?.Value ?? 0) <= (target?.Value ?? 0);
    }
}