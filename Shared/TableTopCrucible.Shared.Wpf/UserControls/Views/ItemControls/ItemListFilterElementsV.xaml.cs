using System;
using ReactiveUI;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels.ItemControls;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views.ItemControls;

/// <summary>
/// Interaction logic for ItemListFilterElementsVm.xaml
/// </summary>
public partial class ItemListFilterElementsV : ReactiveUserControl<ItemListFilterElementsVm>
{
    public ItemListFilterElementsV()
    {
        InitializeComponent();
        this.WhenActivated(() => new IDisposable[]
        {
            this.OneWayBind(ViewModel,
                vm => vm.FilterMode,
                v => v.Header.Text,
                mode => mode == FilterMode.Include
                    ? "Include"
                    : "Exclude"),
            this.Bind(ViewModel,
                vm => vm.NameFilter,
                v => v.Name.Text),
            this.Bind(ViewModel,
                vm => vm.TagEditor,
                v => v.TagEditor.ViewModel),
        });
    }
}