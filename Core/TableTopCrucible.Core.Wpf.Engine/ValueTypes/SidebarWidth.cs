using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValueOf;

namespace TableTopCrucible.Core.Wpf.Engine.ValueTypes
{
    public class SidebarWidth:ValueOf<double, SidebarWidth>
    {
        public static explicit operator SidebarWidth(double value)
            => From(value);

        public static readonly SidebarWidth Default = (SidebarWidth) 300;
    }
}
