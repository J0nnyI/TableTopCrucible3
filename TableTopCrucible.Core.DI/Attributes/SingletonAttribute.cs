using System;

namespace TableTopCrucible.Core.DI.Attributes
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
    public class SingletonAttribute : Attribute, IServiceAttribute
    {
        public SingletonAttribute(Type implementation)
        {
            Implementation = implementation;
        }
        public Type Implementation { get; }
    }
}
