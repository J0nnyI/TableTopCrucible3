using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.ValueTypes.Exceptions;
using TableTopCrucible.Shared.Wpf.ValueTypes;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

public class FractionTag : ValueType<Tag, Fraction, FractionTag>
{
    public Tag Tag => ValueA;
    public Fraction Distribution => ValueB;

    protected override void Validate(Tag? valueA, Fraction? valueB)
    {
        if (valueA is null)
            throw new InvalidValueException(nameof(Tag) + " must not be null");
    }
}