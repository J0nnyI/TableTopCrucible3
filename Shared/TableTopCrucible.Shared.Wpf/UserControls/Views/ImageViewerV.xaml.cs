using System.Reactive.Linq;
using System.Windows;
using ReactiveUI;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views;

/// <summary>
///     Interaction logic for ImageViewerV.xaml
/// </summary>
public partial class ImageViewerV : ReactiveUserControl<ImageViewerVm>
{
    public ImageViewerV()
    {
        InitializeComponent();

        this.WhenActivated(() => new[]
        {
            this.OneWayBind(ViewModel,
                vm => vm.ImageSource,
                v => v.Image.Source),

            this.WhenAnyValue(v => v.ViewModel.ImageSource)
                .Select(img => img is null
                    ? Visibility.Visible
                    : Visibility.Collapsed)
                .ObserveOn(RxApp.MainThreadScheduler)
                .BindTo(this, vm => vm.Placeholder.Visibility),

            this.WhenAnyValue(v => v.Width)
                .BindTo(this, v => v.Placeholder.Width),
            this.WhenAnyValue(v => v.Height)
                .BindTo(this, v => v.Placeholder.Height),

            this.WhenAnyValue(v => v.ViewModel.ImageSource)
                .Select(img => img is not null
                    ? Visibility.Visible
                    : Visibility.Collapsed)
                .ObserveOn(RxApp.MainThreadScheduler)
                .BindTo(this, vm => vm.Image.Visibility),

            this.Bind(ViewModel,
                vm => vm.PlaceholderIcon,
                v => v.Placeholder.Kind),
            this.Bind(ViewModel,
                vm => vm.ErrorText,
                v => v.Placeholder.ToolTip),
        });
    }
}