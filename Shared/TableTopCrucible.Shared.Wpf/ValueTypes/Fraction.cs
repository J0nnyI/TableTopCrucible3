using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.ValueTypes.Exceptions;

namespace TableTopCrucible.Shared.Wpf.ValueTypes;

public class Fraction:ComparableValueType<double, Fraction>
{
    protected override void Validate(double value)
    {
        base.Validate(value);
        if (value < 0 || value > 1)
            throw new InvalidValueException($"value must be between 0 and 1, got {value}");
    }
}
