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
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Services;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;
using TableTopCrucible.Core.WPF.Helper;
using TableTopCrucible.Infrastructure.Repositories.Services;
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
                ViewModel!.BringIntoView.RegisterHandler(context =>
                {
                    var bounds = ViewModel.ViewportContent.Bounds;
                    this.Viewport.Camera.ZoomExtents(Viewport.Viewport,bounds);
                    context.SetOutput(Unit.Default);
                }),
                ViewModel!.GenerateThumbnail.RegisterHandler(context =>
                    context.SetOutput(createThumbnail(context.Input))
                ),

                this.WhenAnyValue(v=>v.ViewModel.PlaceholderText.Value)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, v=>v.PlaceholderText.Text),

                this.WhenAnyValue(vm=>vm.ViewModel.IsLoading)
                    .Select(loading=>
                        loading
                            ? Visibility.Visible
                            : Visibility.Collapsed)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this,
                        v=>v.PlaceholderContainer.Visibility),

                this.WhenAnyValue(v=>v.ViewModel.ViewportContent)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, v=>v.ContainerVisual.Content),
            });
        }

        private ImageFilePath createThumbnail(ModelFilePath model)
        {
            var notificationService = Locator.Current.GetService<INotificationService>();

            try
            {
                if (model is null)
                    throw new InvalidOperationException("could not create a screen shot, the model is null");

                var directorySetupRepository = Locator.Current.GetService<IDirectorySetupRepository>();
                
                var directory = directorySetupRepository.ByFilepath(model.ToFilePath()).FirstOrDefault().Path + (DirectoryName)"Thumbnails";
                var fileName = model.GetFilenameWithoutExtension() +
                                BareFileName.TimeSuffix +
                                FileExtension.UncompressedImage;
                var path = ImageFilePath.From(directory +fileName);

                var source = Viewport.CreateBitmap(Viewport.ActualWidth, Viewport.ActualHeight);
                
                directory.Create();

                using var fileStream = path.OpenWrite();

                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(source));
                encoder.Save(fileStream);

                notificationService.AddNotification(
                    (Name)"Image creation successful",
                    (Description)$"The Image for the File {model} was generated successfully",
                    NotificationType.Confirmation);
                return path;
            }
            catch (Exception ex)
            {
                notificationService.AddNotification(
                    (Name)"Image creation failed",
                    (Description) string.Join(Environment.NewLine, 
                        $"The Image for the File {model} could not be created",
                        "Details:",
                        ex.ToString()
                        ),
                    NotificationType.Confirmation);
                throw;
            }
        }
    }
}
