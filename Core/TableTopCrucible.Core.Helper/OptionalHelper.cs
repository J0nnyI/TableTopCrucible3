using DynamicData.Kernel;

namespace TableTopCrucible.Core.Helper
{
    public static class OptionalHelper
    {
        public static Optional<T> ToOptional<T>(this T obj) => Optional<T>.ToOptional(obj);
    }
}