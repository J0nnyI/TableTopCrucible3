using TableTopCrucible.Core.ValueTypes.Exceptions;

namespace TableTopCrucible.Core.ValueTypes;

public class Tag : ComparableValueType<string, Tag>
{
    protected override void Validate(string value)
    {
        base.Validate(value);
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidValueException("a tag must not be empty");
    }
    public static implicit operator Tag(string value)
        => From(value);

    public override bool Equals(object other)
        => other is Tag otherTag && this.Value.ToLower().Equals(otherTag.Value.ToLower());
    public override int GetHashCode()
        => Value.ToLower().GetHashCode();
}