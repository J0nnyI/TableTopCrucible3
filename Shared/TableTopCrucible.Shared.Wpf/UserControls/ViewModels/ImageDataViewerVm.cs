using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Services;
using TableTopCrucible.Shared.Wpf.ValueTypes;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

[Transient]
public interface IImageDataViewer
{
    ImageData Image { get; set; }
    RenderSize RenderSize { get; set; }
}

public class ImageDataViewerVm : ReactiveObject, IActivatableViewModel, IImageDataViewer
{
    public IImageViewer ImageViewer { get; }
    public ViewModelActivator Activator { get; } = new();

    [Reactive]
    public ImageData Image { get; set; }

    public RenderSize RenderSize { get; set; }

    [Reactive]
    public string Name { get; set; }

    public ImageDataViewerVm(
        IImageViewer imageViewer,
        IFileRepository fileRepository)
    {
        ImageViewer = imageViewer;
        this.WhenActivated(() => new[]
        {
            this.WhenAnyValue(vm => vm.Image.Name)
                .Select(img => img.Value)
                .BindTo(this, vm => vm.Name),

            this.WhenAnyValue(vm => vm.RenderSize)
                .BindTo(this, vm => vm.ImageViewer.RenderSize),

            this.WhenAnyValue(vm => vm.Image)
                .Select(img => fileRepository.WatchSingle(img.HashKey))
                .Switch()
                .Select(file => file?.Path?.ToImagePath())
                .BindTo(this, vm => vm.ImageViewer.ImageFile)
        });
    }
}