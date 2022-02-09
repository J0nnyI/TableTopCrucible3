using System;

namespace TableTopCrucible.Core.DependencyInjection.Attributes
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
    public class TransientAttribute : Attribute
    {
    }
}