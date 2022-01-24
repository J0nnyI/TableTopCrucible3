using System;
using ReactiveUI;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels.ItemControls;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views.ItemControls
{
    /// <summary>
    /// Interaction logic for ItemThumbnailViewerV.xaml
    /// </summary>
    public partial class ItemThumbnailViewerV : ReactiveUserControl<ItemThumbnailViewerVm>
    {
        public ItemThumbnailViewerV()
        {
            InitializeComponent();

            this.WhenActivated(() => new IDisposable[]
            {
                this.OneWayBind(ViewModel,
                    vm => vm.ImageViewer,
                    v => v.ImageViewer.ViewModel),
                this.OneWayBind(ViewModel,
                    vm=>vm.Name,
                    v=>v.Name.Text)
            });
        }
    }
}
