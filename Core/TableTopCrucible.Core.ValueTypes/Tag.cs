using TableTopCrucible.Core.ValueTypes.Exceptions;

namespace TableTopCrucible.Core.ValueTypes
{
    public class Tag : ValueType<string, Tag>
    {
        protected override void Validate(string value)
        {
            base.Validate(value);
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidValueException("a tag must not be empty");
        }
    }
}