using System.Text.RegularExpressions;

namespace TableTopCrucible.Core.Helper;

public static class StringHelper
{
    public static bool Like(this string value, string wildcard) => Regex.IsMatch(value, WildcardToRegex(wildcard));

    private static string WildcardToRegex(string pattern) =>
        "^" + Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";

    public static bool IsNullOrWhitespace(this string str)
        => string.IsNullOrWhiteSpace(str);
    public static bool IsNullOrEmpty(this string str)
        => string.IsNullOrEmpty(str);
}