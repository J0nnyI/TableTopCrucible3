using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TableTopCrucible.Core.Wpf.Helper
{
    public static  class VisualTreeNavigationHelper
    {

        public static T GetByType<T>(this UIElementCollection collection)
            where T : FrameworkElement
        {
            foreach (var child in collection)
            {
                if (child is T element)
                    return element;
            }

            return null;
        }
    }
}
