using ReactiveUI;
using TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Domain.Library.Wpf.UserControls.Views
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
                this.Bind(ViewModel,
                    vm=>vm.TagEditor,
                    v=>v.TagEditor.ViewModel)
            });
        }
    }
}