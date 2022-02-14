using System;
using ReactiveUI;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views;

/// <summary>
/// Interaction logic for ImageDataViewerV.xaml
/// </summary>
public partial class ImageDataViewerV : ReactiveUserControl<ImageDataViewerVm>
{
    public ImageDataViewerV()
    {
        InitializeComponent();

        this.WhenActivated(() => new IDisposable[]
        {
            this.OneWayBind(ViewModel,
                vm => vm.ImageViewer,
                v => v.ImageViewer.ViewModel),
            this.OneWayBind(ViewModel,
                vm => vm.Name,
                v => v.Name.Text)
        });
    }
}