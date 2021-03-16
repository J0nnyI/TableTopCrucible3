using System;
using System.Collections.Generic;
using System.Text;

namespace TableTopCrucible.Core.DI.Attributes
{
    public interface IServiceAttribute
    {
        public Type Implementation { get; }
    }
}
