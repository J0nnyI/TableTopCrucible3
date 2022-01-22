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
    public interface IItemThumbnailViewer
    {
        Item Item { get; set; }
    }
    public class ItemThumbnailViewerVm:ReactiveObject,IActivatableViewModel, IItemThumbnailViewer
    {
        public IImageViewer ImageViewer { get; }
        [Reactive]
        public Item Item { get; set; }
        [Reactive]
        public string Name { get; set; }

        public ViewModelActivator Activator { get; } = new();

        public ItemThumbnailViewerVm(
            IImageViewer imageViewer,
            IImageRepository imageRepository)
        {
            ImageViewer = imageViewer;
            this.WhenActivated(() => new[]
            {
                this.WhenAnyValue(vm=>vm.Item.Name)
                    .Select(img=>img.Value)
                    .BindTo(this, vm=>vm.Name),

                this.WhenAnyValue(vm=>vm.Item)
                    .Select(item=>imageRepository.WatchThumbnail(item.Id))
                    .Switch()
                    .Select(file=>file?.Path?.ToImagePath())
                    .BindTo(this, vm=>vm.ImageViewer.ImageFile)
            });
        }
    }
}
