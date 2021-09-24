using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValueOf;


namespace TableTopCrucible.Core.Jobs.ValueTypes
{
    public class ProgressIncrement:ValueOf<double, ProgressIncrement>
    {
        public static explicit operator ProgressIncrement(double value)
            => From(value);
    }
}
