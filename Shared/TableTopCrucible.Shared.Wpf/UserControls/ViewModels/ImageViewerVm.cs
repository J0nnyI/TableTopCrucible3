using System;
using System.Reactive.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.DependencyInjection.Attributes;
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
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(img =>
                    {
                        if (img is null || !img.Exists())
                        {
                            ImageSource = null;
                        }
                        else
                        {
                            var src = new BitmapImage();
                            src.BeginInit();
                            src.UriSource = img.ToUri();
                            src.CacheOption = BitmapCacheOption.OnLoad;
                            src.EndInit();
                            ImageSource = src;
                        }
                    })
            });
        }

        [Reactive]
        public ImageSource ImageSource { get; private set; }

        public ViewModelActivator Activator { get; } = new();

        [Reactive]
        public ImageFilePath ImageFile { get; set; }
    }
}