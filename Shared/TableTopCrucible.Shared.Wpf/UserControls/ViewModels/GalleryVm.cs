using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Services;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    public class GalleryItem:ReactiveObject, IDisposable
    {
        public ImageData Image { get; }

        [Reactive]
        public Uri FilePath { get; private set; }
        [Reactive]
        public string Name { get; private set; }
        [Reactive]
        public Visibility NoFileErrorVisibility { get; private set; }

        private readonly CompositeDisposable _disposables = new();
        public void Dispose() => _disposables.Dispose();
        public GalleryItem(ImageData image, IFileRepository fileRepository)
        {
            Image = image;

            _disposables.Add(
                fileRepository
                    .Watch(this.WhenAnyValue(vm => vm.Image.HashKey))
                    .ToCollection()
                    .Select(files=>files.FirstOrDefault())
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

        public override string ToString() => Name;
    }

    [Singleton]
    public interface IGallery
    {
        Item Item { get; set; }
    }
    public class GalleryVm : ReactiveObject, IGallery, IActivatableViewModel
    {
        [Reactive] public Item Item { get; set; }
        public ViewModelActivator Activator { get; } = new();
        private ReadOnlyObservableCollection<GalleryItem> _images;
        public ReadOnlyObservableCollection<GalleryItem> Images => _images;
        public GalleryVm(IFileRepository fileRepository, IImageDataRepository imageDataRepository)
        {
            this.WhenActivated(()=>new []
            {
                this.WhenAnyValue(vm=>vm.Item)
                    .Select(item=>imageDataRepository.ByItemId(item.Id))
                    .Switch()
                    .Transform(image=>new GalleryItem(image, fileRepository))
                    .Bind(out _images)
                    .Subscribe()
            });
        }
    }
}
