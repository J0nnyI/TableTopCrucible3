using System.Reactive.Linq;
using ReactiveUI;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views
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
                Observable.ObserveOn<>(this.WhenAnyValue(v => v.ViewModel.Item.Name)
                        .Select(name => name.Value), RxApp.MainThreadScheduler)
                    .BindTo(this, v => v.Name.Text)
            });
        }
    }
}