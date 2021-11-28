using System;

namespace TableTopCrucible.Core.DependencyInjection.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class TransientAttribute : Attribute
    {
    }
}