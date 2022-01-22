using System.Reactive.Linq;
using ReactiveUI;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views
{
    /// <summary>
    ///     Interaction logic for GalleryV.xaml
    /// </summary>
    public partial class GalleryV : ReactiveUserControl<GalleryVm>
    {
        public GalleryV()
        {
            InitializeComponent();
            this.WhenActivated(() => new[]
            {
                this.OneWayBind(ViewModel,
                    vm => vm.Images,
                    v => v.Images.ItemsSource),
                this.OneWayBind(ViewModel,
                    vm => vm.SelectedImageViewer,
                    v => v.SelectedImage.ViewModel),
                this.WhenAnyValue(v => v.Images.SelectedItem)
                    .Cast<GalleryItem>()
                    .Select(galleryItem => galleryItem?.Image)
                    .BindTo(this, v => v.ViewModel.SelectedImage)
            });
        }
    }
}