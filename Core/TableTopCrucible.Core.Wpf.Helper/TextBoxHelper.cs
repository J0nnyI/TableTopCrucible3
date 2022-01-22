using System.Windows.Controls;

namespace TableTopCrucible.Core.Wpf.Helper
{
    public static class TextBoxHelper
    {
        public static void ScrollToRight(this TextBox textBox)
        {
            textBox.CaretIndex = textBox.Text.Length;
            var rect = textBox.GetRectFromCharacterIndex(textBox.CaretIndex);
            textBox.ScrollToHorizontalOffset(rect.Right);
        }
    }
}