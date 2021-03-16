
using System;
using System.Collections.Generic;
using System.Text;

namespace TableTopCrucible.Core.WPF.Helper.Attributes
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ViewModelAttribute : Attribute
    {
        public Type viewType { get; }
        public ViewModelAttribute(Type view)
        {
            viewType = view;
        }
    }
}
