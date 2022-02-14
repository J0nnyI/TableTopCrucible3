using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using MaterialDesignThemes.Wpf;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;
using TableTopCrucible.Domain.Library.Wpf.Services;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;
using TableTopCrucible.Infrastructure.Repositories.Services;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;
using TableTopCrucible.Shared.Wpf.ValueTypes;

namespace TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels
{
    [Singleton]
    public interface IItemGallery:ITabPage
    {
    }

    public class ItemItemGalleryVm : ReactiveObject, IItemGallery, IActivatableViewModel
    {

        public ItemItemGalleryVm(IImageViewer selectedImageViewer, IFileRepository fileRepository,
            IImageRepository imageRepository,
            ILibraryService libraryService)
        {
            SelectedImageViewer = selectedImageViewer;

            _isSelectable = libraryService
                .SingleSelectedItemChanges
                .Select(item => item is not null)
                .ToProperty(this, vm => vm.IsSelectable);

            this.WhenActivated(() => new[]
            {
                libraryService.SingleSelectedItemChanges
                    .Select(item =>
                        item is not null
                            ? imageRepository.WatchMany(item.Id)
                            : Observable.Return(ChangeSet<ImageData, ImageDataId>.Empty))
                    .Switch()
                    .SortBy(x => x.Name.Value)
                    .OutputObservable(out var images)
                    .Transform(image =>
                    {
                        var viewer = Locator.Current.GetService<IImageDataViewer>();
                        viewer.RenderSize = (RenderSize)(SettingsHelper.ThumbnailSize.Width / 3);
                        viewer!.Image = image;
                        return viewer;
                    })
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(out _images)
                    .Subscribe(),

                images
                    .WatchFirstOrDefault()
                    .Select(img =>
                        fileRepository[img?.HashKey].FirstOrDefault()?.Path?.ToImagePath()
                    )
                    .BindTo(this, vm => vm.SelectedImageViewer.ImageFile),

                this.WhenAnyValue(vm => vm.SelectedImage)
                    .Select(img => fileRepository.Watch(img?.HashKey))
                    .Switch()
                    .WatchFirstOrDefault()
                    .Select(img =>
                        fileRepository[img?.HashKey].FirstOrDefault()?.Path?.ToImagePath()
                    )
                    .BindTo(this, vm => vm.SelectedImageViewer.ImageFile)
            });
        }

        public IImageViewer SelectedImageViewer { get; }

        [Reactive]
        public ImageData SelectedImage { get; set; }

        private ReadOnlyObservableCollection<IImageDataViewer> _images;
        public ReadOnlyObservableCollection<IImageDataViewer> Images => _images;

        public ViewModelActivator Activator { get; } = new();
        

        #region ITabPage

        public Name Title => (Name)"Images";
        public PackIconKind SelectedIcon => PackIconKind.ViewGallery;
        public PackIconKind UnselectedIcon => PackIconKind.ViewGalleryOutline;


        private readonly ObservableAsPropertyHelper<bool> _isSelectable;
        public bool IsSelectable => _isSelectable.Value;

        public SortingOrder Position => (SortingOrder)3;

        public void BeforeClose() { }
        #endregion
    }
}