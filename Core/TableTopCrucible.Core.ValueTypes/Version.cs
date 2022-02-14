using Newtonsoft.Json;

namespace TableTopCrucible.Core.ValueTypes;

[JsonObject(MemberSerialization.OptIn)]
public class Version : ValueType<int, int, int, Version>
{
    [JsonProperty]
    public int Major
    {
        get => ValueA;
        init => ValueA = value;
    }

    [JsonProperty]
    public int Minor
    {
        get => ValueB;
        init => ValueB = value;
    }

    [JsonProperty]
    public int Patch
    {
        get => ValueC;
        init => ValueC = value;
    }
}