using ValueOf;

namespace TableTopCrucible.Core.Wpf.Engine.ValueTypes
{
    public class SidebarWidth : ValueOf<double, SidebarWidth>
    {
        public static readonly SidebarWidth Default = (SidebarWidth)300;

        public static explicit operator SidebarWidth(double value)
            => From(value);
    }
}