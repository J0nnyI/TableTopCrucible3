using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Engine.ValueTypes
{
    public class Description : ValueType<string, Description>
    {
        public static explicit operator Description(string value)
            => From(value);
    }
}