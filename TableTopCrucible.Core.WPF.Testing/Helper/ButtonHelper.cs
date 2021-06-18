using System.Windows.Controls;

namespace TableTopCrucible.Core.WPF.Testing.Helper
{
    public static class ButtonHelper
    {
        public static ButtonActions Send(this Button button)
            => new ButtonActions(button);
    }
    public class ButtonActions
    {
        private readonly Button _button;
        private string _buttonText =>
            _button.Name ?? (
                _button.Content is string
                ? _button.Content as string
                : "name not found");

        public ButtonActions(Button button)
            => _button = button;
        public void Click(object parameter = null)
            => _button.Command.Send(_buttonText).Execute(parameter);
    }
}
