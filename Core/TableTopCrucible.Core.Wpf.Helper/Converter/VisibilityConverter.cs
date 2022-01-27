using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace TableTopCrucible.Core.Wpf.Helper.Converter
{
    /// <summary>
    /// valid parameters: h(false => hidden)  + i(invert result)
    /// </summary>
    public class VisibilityConverter:IValueConverter
    {
        public static IValueConverter Instance = new VisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var notVisible = Visibility.Collapsed;
            var invert = false;
            if (parameter is string strPar)
            {
                if (strPar.ToLower().Contains("i"))
                    invert = true;
                if (strPar.ToLower().Contains("h"))
                    notVisible = Visibility.Hidden;
            }

            return (value is true ^ invert) || ((value is not null && value is not false) ^ invert)
                ? Visibility.Visible
                : notVisible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
         =>   value is Visibility.Visible;
    }
}
