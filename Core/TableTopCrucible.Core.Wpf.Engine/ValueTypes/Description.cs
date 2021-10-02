using ValueOf;

namespace TableTopCrucible.Core.Wpf.Engine.ValueTypes
{
    public class Description : ValueOf<string, Description>
    {
        public static explicit operator Description(string value)
            => From(value);
    }
}
