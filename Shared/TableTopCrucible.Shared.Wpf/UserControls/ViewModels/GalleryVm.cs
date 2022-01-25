using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;
using TableTopCrucible.Infrastructure.Repositories.Services;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    [Singleton]
    public interface IGallery
    {
        Item Item { get; set; }
    }

    public class GalleryVm : ReactiveObject, IGallery, IActivatableViewModel
    {

        public GalleryVm(IImageViewer selectedImageViewer, IFileRepository fileRepository,
            IImageRepository imageRepository)
        {
            SelectedImageViewer = selectedImageViewer;

            this.WhenActivated(() => new[]
            {
                this.WhenAnyValue(vm => vm.Item)
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

        [Reactive]
        public Item Item { get; set; }
    }
}