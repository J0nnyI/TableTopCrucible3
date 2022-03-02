using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;
using ReactiveUI;
using TableTopCrucible.Core.Wpf.Helper;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views;

public class EditModeToCloseIconConverter : IMultiValueConverter
{
    public static EditModeToCloseIconConverter Instance = new();

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length != 2 ||
            values[0] is not bool editMode ||
            values[1] is not bool addMode)
            return PackIconKind.Error;

        return editMode && !addMode
            ? PackIconKind.Undo
            : PackIconKind.Close;
    }


    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
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
                vm => vm.TagList,
                v => v.TagList.ItemsSource),
        });
    }
}