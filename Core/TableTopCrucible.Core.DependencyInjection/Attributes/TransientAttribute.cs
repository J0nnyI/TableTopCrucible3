using System;

namespace TableTopCtucible.Core.DependencyInjection.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class TransientAttribute : Attribute, IServiceAttribute
    {
        public TransientAttribute(Type implementation)
        {
            Implementation = implementation;
        }

        public Type Implementation { get; }
    }
}
