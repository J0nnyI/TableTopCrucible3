using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.CompilerServices;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Engine.ValueTypes
{
    public class CreationTime:ComparableValueType<DateTime, CreationTime>
    {
        public static CreationTime Now => From(DateTime.Now);
    }
}
