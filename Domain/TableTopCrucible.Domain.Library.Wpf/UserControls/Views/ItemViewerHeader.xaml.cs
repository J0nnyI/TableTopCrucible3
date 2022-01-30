using System.Reactive.Linq;
using ReactiveUI;
using TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Domain.Library.Wpf.UserControls.Views
{
    /// <summary>
    ///     Interaction logic for ModelHeaderV.xaml
    /// </summary>
    public partial class ItemViewerHeaderV : ReactiveUserControl<ItemViewerHeaderVm>
    {
        public ItemViewerHeaderV()
        {
            InitializeComponent();
            this.WhenActivated(() => new[]
            {
                this.WhenAnyValue(v => v.ViewModel.Item.Name)
                    .Select(name => name.Value)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, v => v.Name.Text),
                this.Bind(ViewModel,
                    vm=>vm.TabStrip,
                    v=>v.TabStrip.ViewModel)
            });
        }
    }
}