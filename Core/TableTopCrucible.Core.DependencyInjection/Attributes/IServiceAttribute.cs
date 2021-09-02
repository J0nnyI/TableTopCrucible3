using System;

namespace TableTopCrucible.Core.DependencyInjection.Attributes
{
    public interface IServiceAttribute
    {
        public Type Implementation { get; }
    }
}
