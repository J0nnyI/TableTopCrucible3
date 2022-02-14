using DynamicData.Kernel;

namespace TableTopCrucible.Core.Helper;

public static class OptionalHelper
{
    public static Optional<T> ToOptional<T>(this T obj) => Optional<T>.ToOptional(obj);

    public static T ToNullable<T>(this Optional<T> obj) where T : class
        => obj.HasValue
            ? obj.Value
            : null;
}