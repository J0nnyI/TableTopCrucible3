using System;
using System.Linq;

namespace TableTopCrucible.Core.Helper;

public static class ObjectHelper
{
    public static T[] AsArray<T>(this T obj, params T[] otherValues)
    {
        var values = otherValues.ToList();
        values.Insert(0, obj);
        return values.ToArray();
    }

    public static bool IsIn<T>(this T obj, params T[] compValues) => compValues.Contains(obj);
    public static bool IsNotIn<T>(this T obj, params T[] compValues) => !compValues.Contains(obj);
    public static bool HasCustomAttribute<T>(this Type type, bool inherit = false) where T : Attribute =>
        type.GetCustomAttributes(typeof(T), inherit).Length > 0;
}