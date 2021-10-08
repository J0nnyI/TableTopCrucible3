using System;

namespace TableTopCrucible.Core.DependencyInjection.Attributes
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = true)]
    public class ScopedAttribute : Attribute
    {
    }
}