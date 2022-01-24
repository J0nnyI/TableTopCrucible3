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
                this.WhenAnyValue(v=>v.ViewModel.Item.FileKey3d)
                    .Select(key=>key.Value)
                    .BindTo(this, v=>v.HashKey.Text),
                this.WhenAnyValue(v=>v.ViewModel.Item.Id)
                    .Select(key=>key.Value)
                    .BindTo(this, v=>v.ItemId.Text),
                this.WhenAnyValue(v => v.ViewModel.Item)
                    .Select(item => item?.Tags?.Connect() ?? Observable.Return(ChangeSet<Tag>.Empty))
                    .Switch()
                    .ToCollection()
                    .BindTo(this, v => v.TagList.ItemsSource),
                this.OneWayBind(ViewModel,
                    vm=>ViewModel.TagEditor,
                    v=>v.TagEditor.ViewModel)
            });
        }
    }
}