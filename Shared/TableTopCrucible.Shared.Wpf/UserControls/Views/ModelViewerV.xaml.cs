using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

using HelixToolkit.Wpf;

using Microsoft.EntityFrameworkCore.Diagnostics;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Splat;

using TableTopCrucible.Core.Engine.Services;
using TableTopCrucible.Core.Engine.ValueTypes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Services;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;
using TableTopCrucible.Core.Wpf.Helper;
using TableTopCrucible.Infrastructure.Repositories.Services;
using TableTopCrucible.Shared.Helper;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Shared.Wpf.UserControls.Views
{
    /// <summary>
    /// Interaction logic for ModelViewerV.xaml
    /// </summary>
    public partial class ModelViewerV : ReactiveUserControl<ModelViewerVm>
    {
        public ModelViewerV()
        {
            InitializeComponent();
            this.WhenActivated(() => new[]
            {
                // interactions
                ViewModel!.BringIntoView.RegisterHandler(context =>
                {
                    var bounds = ViewModel.ViewportContent.Bounds;
                    this.Viewport.Camera.ZoomExtents(Viewport.Viewport, bounds);
                    context.SetOutput(Unit.Default);
                }),

                // bindings
                this.WhenAnyValue(v => v.ViewModel.PlaceholderText.Value)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, v => v.PlaceholderText.Text),

                this.WhenAnyValue(vm => vm.ViewModel.IsLoading)
                    .Select(loading =>
                        loading
                            ? Visibility.Visible
                            : Visibility.Collapsed)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this,
                        v => v.PlaceholderContainer.Visibility),

                this.WhenAnyValue(v => v.ViewModel.ViewportContent)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, v => v.ContainerVisual.Content),
                new ActOnDispose(
                    ()=>ViewModel.Viewport=Viewport,
                    () => ViewModel.Viewport = null)

            });

        }

        private ImageFilePath createThumbnail()
        {
            var notificationService = Locator.Current.GetService<INotificationService>();

            if (ViewModel?.Model is null)
                throw new InvalidOperationException("could not create a screen shot, the model is null");
            if (!ViewModel.Model.Exists())
                throw new InvalidOperationException("could not create a screen shot, the model has been deleted");


            if (!ViewModel.Model.Exists())
                throw new InvalidOperationException("could not create a screen shot, the model has been deleted");

            try
            {

                var directorySetupRepository = Locator.Current.GetService<IDirectorySetupRepository>();

                var directory = directorySetupRepository.ByFilepath(ViewModel.Model.ToFilePath()).FirstOrDefault().Path + (DirectoryName)"Thumbnails";
                var fileName = ViewModel.Model.GetFilenameWithoutExtension() +
                                BareFileName.TimeSuffix +
                                FileExtension.UncompressedImage;
                var path = ImageFilePath.From(directory + fileName);

                var source = Viewport.CreateBitmap();

                directory.Create();

                using var fileStream = path.OpenWrite();

                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(source));
                encoder.Save(fileStream);

                notificationService.AddNotification(
                    (Name)"Image creation successful",
                    (Description)$"The Image for the File {ViewModel.Model} was generated successfully",
                    NotificationType.Confirmation);
                return path;
            }
            catch (Exception ex)
            {
                notificationService.AddNotification(
                    (Name)"Image creation failed",
                    (Description)string.Join(Environment.NewLine,
                        $"The Image for the File {ViewModel.Model} could not be created",
                        "Details:",
                        ex.ToString()
                        ),
                    NotificationType.Confirmation);
                throw;
            }
        }
    }
}
