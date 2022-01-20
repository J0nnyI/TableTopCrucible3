using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableTopCrucible.Core.ValueTypes
{
    public class Version : ValueType<int, int, int, Version>
    {
        public int Major
        {
            get => ValueA;
            init => ValueA = value;
        }
        public int Minor
        {
            get => ValueB;
            init => ValueB = value;
        }
        public int Patch
        {
            get => ValueC;
            init => ValueC = value;
        }
    }
}
