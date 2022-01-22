using DynamicData.Kernel;

namespace TableTopCrucible.Core.Helper
{
    public static class StructHelper
    {
        public static T? ToNullable<T>(this T value) where T : struct => value.Equals(default(T))
            ? null
            : value;

        public static T ToValue<T>(this Optional<T> value) => value.HasValue
            ? value.Value
            : default;
    }
}