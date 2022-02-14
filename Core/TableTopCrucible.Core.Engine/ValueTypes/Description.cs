using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Engine.ValueTypes;

public class Description : ValueType<string, Description>
{
    public static implicit operator Description(string value)
        => From(value);

    public bool HasContent => string.IsNullOrWhiteSpace(Value);
}