using System;
using System.Collections.Generic;
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

using DynamicData;

using ReactiveUI;

using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views
{
    /// <summary>
    /// Interaction logic for ItemDataViewerV.xaml
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
                    v=>v.HashKey.Text),
                this.WhenAnyValue(v=>v.ViewModel.Item)
                    .Select(item=>item?.Tags?.Connect() ?? Observable.Return(ChangeSet<Tag>.Empty))
                    .Switch()
                    .ToCollection()
                    .BindTo(this, v=>v.Tags.ItemsSource),
            });
        }
    }
}
