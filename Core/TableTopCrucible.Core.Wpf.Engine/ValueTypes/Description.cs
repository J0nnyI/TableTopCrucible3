using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Core.Wpf.Engine.ValueTypes
{
    public class Description : ValueType<string, Description>
    {
        public static explicit operator Description(string value)
            => From(value);
    }
}