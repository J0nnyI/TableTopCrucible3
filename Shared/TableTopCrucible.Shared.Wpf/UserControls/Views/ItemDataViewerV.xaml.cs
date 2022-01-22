using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views
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
                this.OneWayBind(ViewModel,
                    vm => vm.Item.FileKey3d.Value,
                    v => v.HashKey.Text),
                this.WhenAnyValue(v => v.ViewModel.Item)
                    .Select(item => item?.Tags?.Connect() ?? Observable.Return(ChangeSet<Tag>.Empty))
                    .Switch()
                    .ToCollection()
                    .BindTo(this, v => v.Tags.ItemsSource)
            });
        }
    }
}