using System;
using System.Collections.Generic;
using System.Text;

namespace TableTopCrucible.Core.DI.Attributes
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
