using ReactiveUI;
using TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Domain.Library.Wpf.UserControls.Views
{
    /// <summary>
    ///     Interaction logic for ItemFileListVm.xaml
    /// </summary>
    public partial class ItemFileListV : ReactiveUserControl<ItemFileListVm>
    {
        public ItemFileListV()
        {
            InitializeComponent();
            this.WhenActivated(() => new[]
            {
                this.OneWayBind(ViewModel,
                    vm => vm.Files,
                    v => v.Files.ItemsSource),
                this.WhenAnyValue(v => v.ViewModel).BindTo(this, v => v.DataContext)
            });
        }
    }
}