using ReactiveUI;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views;

/// <summary>
///     Interaction logic for FilteredListHeaderV.xaml
/// </summary>
public partial class FilteredListHeaderV : ReactiveUserControl<FilteredListHeaderVm>
{
    public FilteredListHeaderV()
    {
        InitializeComponent();
    }
}