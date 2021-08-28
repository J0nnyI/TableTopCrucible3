using System;

namespace TableTopCtucible.Core.DependencyInjection.Attributes
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
    public class ScopedAttribute : Attribute, IServiceAttribute
    {
        public ScopedAttribute(Type implementation)
        {
            Implementation = implementation;
        }

        public Type Implementation { get; }
    }
}
