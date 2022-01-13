using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace TableTopCrucible.Core.ValueTypes.Helper
{
    public class SerializedStringList : ValueType<string, SerializedStringList>
    {
        public static SerializedStringList From(IEnumerable<string> list)
            => From(JsonSerializer.Serialize(list ?? Enumerable.Empty<string>()));

        public IEnumerable<string> Deserialize()
            => JsonSerializer.Deserialize<IEnumerable<string>>(Value);
    }

    public static class StringHelper
    {
        public static SerializedStringList ToJson(this IEnumerable<string> list)
            => SerializedStringList.From(list);
    }
}