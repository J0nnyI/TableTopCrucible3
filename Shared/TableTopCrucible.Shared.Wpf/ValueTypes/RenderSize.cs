using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.ValueTypes.Exceptions;

namespace TableTopCrucible.Shared.Wpf.ValueTypes;

public class RenderSize : ValueType<int, RenderSize>
{
    protected override void Validate(int value)
    {
        base.Validate(value);
        if (value < 0)
            throw new InvalidValueException("the value must be positive");
    }
}