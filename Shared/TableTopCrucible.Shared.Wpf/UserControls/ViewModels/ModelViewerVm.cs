using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

using HelixToolkit.Wpf;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Services;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    [Transient]
    public interface IModelViewer
    {
        public ModelFilePath Model { get; set; }
        // will be shown instead of the model if set
        public Message PlaceholderMessage { get; set; }
    }
    public class ModelViewerVm : ReactiveObject, IModelViewer, IActivatableViewModel
    {
        private readonly IDirectorySetupRepository _directorySetupRepository;
        public ReactiveCommand<Unit,  ImageData> GenerateThumbnailCommand { get; }
        public ViewModelActivator Activator { get; } = new();

        [Reactive]
        public ModelFilePath Model { get; set; }
        [Reactive]
        public Message PlaceholderMessage { get; set; }
        [Reactive]
        public bool IsLoading { get; set; }
        [Reactive]
        public Message PlaceholderText { get; set; }
        [Reactive]
        public Model3DGroup ViewportContent { get; set; }

        public Interaction<Unit, Unit> BringIntoView { get; } = new();
        public Interaction<ModelFilePath, ImageFilePath> GenerateThumbnail = new();
        public ModelViewerVm(IDirectorySetupRepository directorySetupRepository, IFileRepository fileRepository, IImageDataRepository imageDataRepository)
        {
            _directorySetupRepository = directorySetupRepository;
            GenerateThumbnailCommand = ReactiveCommand.Create<Unit, ImageData>( _ =>
            {
                var imagePath =  GenerateThumbnail.Handle(Model).Wait();
                var newFile = new FileData(
                    imagePath.ToFilePath(), 
                    FileHashKey.Create(imagePath.ToFilePath()),
                    imagePath.GetLastWriteTime());
                fileRepository.Add(newFile);

                var image = imageDataRepository.Data.SingleOrDefault(file => file.HashKey == newFile.HashKey);

                if(image is null)
                {
                    image = new ImageData(Model.GetFilenameWithoutExtension().ToName(), newFile.HashKey);
                    imageDataRepository.Add(image);
                }

                return image;
            });
            this.WhenActivated(() => new[]
                {
                    this.WhenAnyValue(
                            v => v.Model, 
                            v=>v.PlaceholderMessage,
                            (model, message)=>new{model, message})
                        .Do((x) =>
                        {
                            PlaceholderText =
                                x.message ??
                                (x.model is null
                                    ? (Message)"No File selected"
                                    : (Message)"Loading");
                            if(x.model is null || 
                               x.message is not null ||
                               x.model.GetSize().Value > SettingsHelper.FileMinLoadingScreenSize
                               )
                                IsLoading = true;

                            ViewportContent = null;
                        })
                        .Select(x=>x.model)
                        .WhereNotNull()
                        .Select(file =>
                        {
                            return file is null
                                ? Observable.Return(null as Model3DGroup)
                                : Observable.Start(() =>
                                {
                                    var material = Materials.LightGray;
                                    var model = new ModelImporter()
                                    {
                                        DefaultMaterial = material
                                    }.Load(file.Value);
                                    model.Freeze();
                                    return model;
                                }, RxApp.TaskpoolScheduler);
                        })
                        .Switch()
                        .Subscribe(model =>
                        {
                            if (model is null)
                                return;

                            RxApp.MainThreadScheduler.Schedule(model, (_, model) =>
                            {
                                if (model is null)// happens when a new item has been loaded while the scheduler was queued
                                    return null;
                                ViewportContent = model;
                                BringIntoView.Handle(Unit.Default).Subscribe();
                                IsLoading = false;
                                return null;
                            });
                        }),
            });
        }


    }
}
