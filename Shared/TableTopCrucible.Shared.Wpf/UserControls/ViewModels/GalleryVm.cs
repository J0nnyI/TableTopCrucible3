using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;
using TableTopCrucible.Infrastructure.Repositories.Services;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    public class GalleryItem : ReactiveObject, IDisposable
    {
        private readonly CompositeDisposable _disposables = new();

        public GalleryItem(ImageData image, IFileRepository fileRepository)
        {
            Image = image;

            _disposables.Add(
                fileRepository
                    .Watch(this.WhenAnyValue(vm => vm.Image.HashKey))
                    .ToCollection()
                    .Select(files => files.FirstOrDefault())
                    .Subscribe(fileData =>
                    {
                        FilePath = fileData?.Path?.ToUri();
                        NoFileErrorVisibility =
                            fileData is null
                                ? Visibility.Visible
                                : Visibility.Collapsed;
                    }),
                this.WhenAnyValue(vm => vm.Image.Name.Value)
                    .BindTo(this, vm => vm.Name)
            );
        }

        public ImageData Image { get; }

        [Reactive]
        public Uri FilePath { get; private set; }

        [Reactive]
        public string Name { get; private set; }

        [Reactive]
        public Visibility NoFileErrorVisibility { get; private set; }

        public void Dispose() => _disposables.Dispose();

        public override string ToString() => Name;
    }

    [Singleton]
    public interface IGallery
    {
        Item Item { get; set; }
    }

    public class GalleryVm : ReactiveObject, IGallery, IActivatableViewModel
    {
        private ReadOnlyObservableCollection<GalleryItem> _images;

        public GalleryVm(IImageViewer selectedImageViewer, IFileRepository fileRepository,
            IImageDataRepository imageDataRepository)
        {
            SelectedImageViewer = selectedImageViewer;

            this.WhenActivated(() => new[]
            {
                this.WhenAnyValue(vm => vm.Item)
                    .Select(item =>
                        item is not null
                            ? imageDataRepository.WatchMany(item.Id)
                            : Observable.Return(ChangeSet<ImageData, ImageDataId>.Empty))
                    .Switch()
                    .SortBy(x => x.Name.Value)
                    .OutputObservable(out var images)
                    .Transform(image => new GalleryItem(image, fileRepository))
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

        public ReadOnlyObservableCollection<GalleryItem> Images => _images;

        public ViewModelActivator Activator { get; } = new();

        [Reactive]
        public Item Item { get; set; }
    }
}