using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace TableTopCrucible.Core.WPF.Helper
{
    public static class TextBoxHelper
    {
        /// <summary>
        /// scrolls the text so that you can see the end of it
        /// </summary>
        /// <param name="textBox"></param>
        public static void ScrollToHorizontalEnd(this TextBox textBox)
        {
            textBox.CaretIndex = textBox.Text.Length;
            var rect = textBox.GetRectFromCharacterIndex(textBox.CaretIndex);
            textBox.ScrollToHorizontalOffset(rect.Right);
        }
    }
}
