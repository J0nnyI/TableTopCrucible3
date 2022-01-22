using System.Windows.Input;

namespace TableTopCrucible.Core.Wpf.Helper
{
    public static class KeyboardHelper
    {
        public static bool IsKeyPressed(ModifierKeys key)
            => (Keyboard.Modifiers & key) == key;
    }
}