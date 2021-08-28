using System;

namespace TableTopCtucible.Core.DependencyInjection.Attributes
{
    public interface IServiceAttribute
    {
        public Type Implementation { get; }
    }
}
