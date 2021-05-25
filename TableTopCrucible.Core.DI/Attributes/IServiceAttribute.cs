using System;

namespace TableTopCrucible.Core.DI.Attributes
{
    public interface IServiceAttribute
    {
        public Type Implementation { get; }
    }
}
