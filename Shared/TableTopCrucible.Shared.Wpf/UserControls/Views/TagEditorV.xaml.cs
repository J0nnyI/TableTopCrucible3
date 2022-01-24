using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using MaterialDesignThemes.Wpf;

using ReactiveUI;

using TableTopCrucible.Core.Wpf.Helper;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views
{
    public class EditModeToCloseIconConverter : IValueConverter
    {
        public static EditModeToCloseIconConverter Instance = new();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is true // true = editmode
                ? PackIconKind.Undo
                : PackIconKind.Close;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    /// <summary>
    /// Interaction logic for TagEditorV.xaml
    /// </summary>
    public partial class TagEditorV : ReactiveUserControl<TagEditorVm>
    {
        public TagEditorV()
        {
            InitializeComponent();
            this.WhenActivated(() => new[]
            {
                this.OneWayBind(ViewModel,
                    vm=>vm.TagList,
                    v=>v.TagList.ItemsSource)
            });
        }

        private void Chip_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Chip { DataContext: TagController tagController, Content: Panel child } chip)
                return;

            tagController.EditModeEnabled = true;
            var tb = child.Children.GetByType<Panel>()
                .Children.GetByType<TextBox>();

            focusTextBox(tb);
        }

        private void TextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is not TextBox { DataContext: TagController tagController } tb)
                return;


            switch (e.Key)
            {
                case Key.Enter:
                    tagController.Confirm();
                    break;
                case Key.Escape:
                    tagController.Revert();
                    break;
                default:
                    return;
            }

            tb.SelectionLength = 0;
            Keyboard.ClearFocus();
        }

        private void NewTag_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is not Button { DataContext: TagController tagController, Parent: Panel panel })
                return;

            tagController.IsNew = false;
            tagController.EditModeEnabled = true;

            var tb = panel.Children.GetByType<StackPanel>()
                .Children.GetByType<TextBox>();

            focusTextBox(tb);
        }

        private static void focusTextBox(TextBox tb)
        {
            tb?.Events()
                .MouseEnter
                .Take(1)
                .Subscribe(_ =>
                {
                    tb.Focus();
                    tb.CaretIndex = 0;
                });

        }

        private void SaveChanges_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is not Button { DataContext: TagController tagController, Parent: Panel panel })
                return;

            var tb = panel.Children.GetByType<TextBox>();

            tagController.Confirm();
            tb.SelectionLength = 0;
            Keyboard.ClearFocus();
        }
    }
}
