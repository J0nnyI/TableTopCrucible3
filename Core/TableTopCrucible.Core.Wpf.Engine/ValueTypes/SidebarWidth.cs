using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Wpf.Engine.ValueTypes
{
    public class SidebarWidth : ValueType<double, SidebarWidth>
    {
        public static readonly SidebarWidth Default = (SidebarWidth)300;

        public static explicit operator SidebarWidth(double value)
            => From(value);
    }
}