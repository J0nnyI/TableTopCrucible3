using System.Reactive.Linq;
using ReactiveUI;
using TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Domain.Library.Wpf.UserControls.Views;

/// <summary>
///     Interaction logic for GalleryV.xaml
/// </summary>
public partial class ItemGalleryV : ReactiveUserControl<ItemItemGalleryVm>
{
    public ItemGalleryV()
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
                .Cast<IImageDataViewer>()
                .Select(viewer => viewer?.Image)
                .BindTo(this, v => v.ViewModel.SelectedImage)
        });
    }
}