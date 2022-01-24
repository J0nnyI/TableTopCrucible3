using System;
using System.IO;
using System.Reactive.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using MaterialDesignThemes.Wpf;

using Microsoft.AspNetCore.Components;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    [Transient]
    public interface IImageViewer
    {
        ImageFilePath ImageFile { get; set; }
    }

    public class ImageViewerVm : ReactiveObject, IActivatableViewModel, IImageViewer
    {
        public ImageViewerVm()
        {
            this.WhenActivated(() => new[]
            {
                this.WhenAnyValue(vm => vm.ImageFile)
                    .ObserveOn(RxApp.TaskpoolScheduler)
                    .Select(file =>
                    {
                        if (file is null || !file.Exists())
                        {
                            return new
                            {
                                imageSource = null as BitmapImage,
                                errorText = (Message)"No file selected",
                                placeholderIcon = PackIconKind.ImageOutline
                            };
                        }

                        try
                        {
                            using var stream = file.OpenRead();
                            var src = new BitmapImage();
                            src.BeginInit();
                            src.CacheOption = BitmapCacheOption.OnLoad;
                            src.StreamSource = stream;
                            src.EndInit();
                            src.DecodePixelWidth =Convert.ToInt32(SettingsHelper.ThumbnailWidth);
                            src.Freeze();
                            return new
                            {
                                imageSource = src,
                                errorText = null as Message,
                                placeholderIcon = PackIconKind.ImageOffOutline,
                            };
                        }
                        catch (Exception e)
                        {
                            return new
                            {
                                imageSource = null as BitmapImage,
                                errorText = (Message)"File could not be loaded",
                                placeholderIcon = PackIconKind.ImageOffOutline,
                            };
                        }
                    })
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(x =>
                    {
                        ImageSource = x.imageSource;
                        ErrorText = x.errorText;
                        PlaceholderIcon = x.placeholderIcon;
                    })
            });
        }

        [Reactive]
        public ImageSource ImageSource { get; private set; }

        public ViewModelActivator Activator { get; } = new();

        [Reactive]
        public ImageFilePath ImageFile { get; set; }

        [Reactive]
        public Message ErrorText { get; set; }

        [Reactive]
        public PackIconKind PlaceholderIcon { get; set; }
    }
}