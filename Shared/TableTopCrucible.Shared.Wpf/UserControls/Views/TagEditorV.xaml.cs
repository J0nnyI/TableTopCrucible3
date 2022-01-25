using System;
using System.Globalization;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using System.Windows.Threading;

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
            return value is true // true = editMode
                ? PackIconKind.Undo
                : PackIconKind.Close;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
    public class EditModeToTextBoxWidthConverter : IValueConverter
    {
        public static EditModeToTextBoxWidthConverter Instance = new();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is true // true = editMode
                ? 100
                : 0;

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
                    v=>v.TagList.ItemsSource),
            });
        }

        private void TextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is not ComboBox { DataContext: TagController tagController } tb)
                return;

            var actionSuccess = true;
            switch (e.Key)
            {
                case Key.Enter:
                    actionSuccess = tagController.Confirm();
                    break;
                case Key.Escape:
                    tagController.Revert();
                    break;
                default:
                    return;
            }

            if (!actionSuccess)
                return;

            Keyboard.ClearFocus();
            tb.MoveFocus(new TraversalRequest(FocusNavigationDirection.Up));
        }

        private void Chip_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Chip { DataContext: TagController tagController, Content: Panel panel })
                return;

            tagController.EditModeEnabled = true;

            focusTextBox(panel.Children.GetByType<StackPanel>());
        }

        private void NewTag_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is not Button { DataContext: TagController tagController, Parent: Panel panel })
                return;

            tagController.AddMode = false;
            tagController.EditModeEnabled = true;

            focusTextBox(panel.Children.GetByType<StackPanel>());
        }

        private void SaveChanges_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is not Button { DataContext: TagController tagController, Parent: Panel panel })
                return;

            var tb = panel.Children.GetByType<ComboBox>();

            tagController.Confirm();
            tb.MoveFocus(new TraversalRequest(FocusNavigationDirection.Up));
        }

        private void focusTextBox(Panel comboBoxContainer)
        {
            var cb = comboBoxContainer
                .Children.GetByType<ComboBox>();


            Dispatcher.BeginInvoke(() =>
            {
                cb.Focus();
                cb.IsDropDownOpen = true;
            }, (DispatcherPriority)5);

        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is not Panel { DataContext: TagController { EditModeEnabled: true } } panel)
                return;

            focusTextBox(panel);
        }
    }
}
