using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels.ItemControls;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views.ItemControls
{
    /// <summary>
    ///     Interaction logic for ItemDataViewerV.xaml
    /// </summary>
    public partial class ItemDataViewerV : ReactiveUserControl<ItemDataViewerVm>
    {
        public ItemDataViewerV()
        {
            InitializeComponent();
            this.WhenActivated(() => new[]
            {
                this.WhenAnyValue(v=>v.ViewModel.Item.FileKey3d)
                    .Select(key=>key.Value)
                    .BindTo(this, v=>v.HashKey.Text),
                this.WhenAnyValue(v=>v.ViewModel.Item.Id)
                    .Select(key=>key.Value)
                    .BindTo(this, v=>v.ItemId.Text),
                this.Bind(ViewModel,
                    vm=>vm.TagEditor,
                    v=>v.TagEditor.ViewModel)
            });
        }
    }
}