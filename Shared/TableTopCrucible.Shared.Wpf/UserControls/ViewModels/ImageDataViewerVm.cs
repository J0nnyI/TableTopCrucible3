using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Services;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    [Transient]
    public interface IImageDataViewer
    {
        ImageData Image { get; set; }
    }
    public class ImageDataViewerVm:ReactiveObject, IActivatableViewModel, IImageDataViewer
    {
        public IImageViewer ImageViewer { get; }
        public ViewModelActivator Activator { get; } = new();
        [Reactive]
        public ImageData Image { get; set; }
        [Reactive]
        public string Name { get; set; }

        public ImageDataViewerVm(
            IImageViewer imageViewer,
            IFileRepository fileRepository)
        {
            ImageViewer = imageViewer;
            this.WhenActivated(()=>new []
            {
                this.WhenAnyValue(vm=>vm.Image)
                    .OutputObservable(out var imageChanges)
                    .Select(img=>img.Name.Value)
                    .BindTo(this, vm=>vm.Name),
                
                imageChanges
                    .Select(img=>fileRepository.WatchSingle(img.HashKey))
                    .Switch()
                    .Select(file=>file?.Path?.ToImagePath())
                    .BindTo(this, vm=>vm.ImageViewer.ImageFile)
            });
        }
    }
}
