using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Shared.Services;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels.ItemControls;

namespace TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels
{
    [Transient]
    public interface ISingleItemViewer:IDisposable
    {
        public Item Item { get; set; }
        ReactiveCommand<Unit, Unit> GenerateThumbnailCommand { get; }
    }
    public class SingleItemViewerVm:ReactiveObject, IActivatableViewModel, ISingleItemViewer
    {
        private readonly IGalleryService _galleryService;
        private readonly CompositeDisposable _disposables = new();
        public IItemActions Actions { get; }
        public IItemFileList FileList { get; }
        public IGallery Gallery { get; }
        public IItemDataViewer DataViewer { get; }
        public IItemModelViewer ModelViewer { get; }
        public IItemViewerHeader ViewerHeader { get; }
        public ViewModelActivator Activator { get; } = new();

        public SingleItemViewerVm(
            IItemActions actions,
            IItemFileList fileList,
            IGallery gallery,
            IItemDataViewer dataViewer,
            IItemModelViewer modelViewer,
            IItemViewerHeader viewerHeader,
            IGalleryService galleryService)
        {
            _galleryService = galleryService;
            Actions = actions;
            FileList = fileList.DisposeWith(_disposables);
            Gallery = gallery;
            DataViewer = dataViewer;
            ModelViewer = modelViewer;
            ViewerHeader = viewerHeader;
            Actions.GenerateThumbnailsByViewportCommand = ModelViewer.GenerateThumbnailCommand;

            var itemChanges = this.WhenAnyValue(vm => vm.Item)
                .Replay(1)// must not be publish since the update happens before the view is activated -> no value pushed in whenActivated
                .RefCount();
            this.WhenActivated(()=>new []
            {
                itemChanges
                    .BindTo(this, vm => vm.ModelViewer.Item),
                itemChanges
                    .BindTo(this, vm => vm.DataViewer.Item),
                itemChanges
                    .BindTo(this, vm => vm.ViewerHeader.Item),
                itemChanges
                    .BindTo(this, vm => vm.FileList.Item),
                itemChanges
                    .BindTo(this, vm => vm.Gallery.Item),
                itemChanges
                    .BindTo(this, vm => vm.Actions.Item)
            });
        }
        [Reactive]
        public Item Item { get; set; }

        public ReactiveCommand<Unit, Unit> GenerateThumbnailCommand => ModelViewer.GenerateThumbnailCommand;

        public void Dispose()
            => _disposables.Dispose();
        public void HandleFileDrop(FilePath[] filePaths)
        {
            var images =
                filePaths
                    .Where(file => file.IsImage())
                    .Select(img => img.ToImagePath())
                    .ToArray();
            _galleryService.AddImagesToItem(Item, images);
        }
    }

}
