using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using ValueOf;

namespace TableTopCrucible.Core.Helper
{
    public class SerializedStringList:ValueOf<string, SerializedStringList>
    {
        public static SerializedStringList From(IEnumerable<string> list)
            => From(JsonSerializer.Serialize(list ?? Enumerable.Empty<string>()));

        public IEnumerable<string> Deserialize()
            => JsonSerializer.Deserialize<IEnumerable<string>>(Value);
    }

    public static class StringHelper
    {
        public static bool Like(this string value, string wildcard) => Regex.IsMatch(value, WildcardToRegex(wildcard));

        private static string WildcardToRegex(string pattern) =>
            "^" + Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";

        public static SerializedStringList ToJson(this IEnumerable<string> list)
            => SerializedStringList.From(list);
    }
}