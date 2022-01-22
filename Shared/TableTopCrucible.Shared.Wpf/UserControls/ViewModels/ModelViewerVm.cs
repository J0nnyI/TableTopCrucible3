using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Media.Media3D;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Repositories.Services;
using TableTopCrucible.Shared.Wpf.Services;
using ReactiveObjectHelper = TableTopCrucible.Core.Wpf.Helper.ReactiveObjectHelper;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    [Transient]
    public interface IModelViewer
    {
        ModelFilePath Model { get; set; }

        /// <summary>
        ///     will be shown instead of the model if set
        /// </summary>
        Message PlaceholderMessage { get; set; }

        public bool IsActivated { get; }

        ImageFilePath GenerateThumbnail(Name sourceName);
    }

    public class ModelViewerVm : ReactiveObject, IModelViewer, IActivatableViewModel
    {
        private readonly IDirectorySetupRepository _directorySetupRepository;
        private readonly IWpfThumbnailGenerationService _thumbnailService;

        private readonly ObservableAsPropertyHelper<bool> _isActivated;

        public ModelViewerVm(
            IDirectorySetupRepository directorySetupRepository,
            IFileRepository fileRepository,
            IImageDataRepository imageDataRepository,
            IWpfThumbnailGenerationService thumbnailService)
        {
            _directorySetupRepository = directorySetupRepository;
            _thumbnailService = thumbnailService;
            this.WhenActivated(() => new[]
            {
                this.WhenAnyValue(
                        v => v.Model,
                        v => v.PlaceholderMessage,
                        (model, message) => new { model, message })
                    .Do(x =>
                    {
                        PlaceholderText =
                            x.message ??
                            (x.model is null
                                ? (Message)"No File selected"
                                : (Message)"Loading");
                        if (x.model is null ||
                            x.message is not null ||
                            x.model.GetSize().Value > SettingsHelper.FileMinLoadingScreenSize
                           )
                            IsLoading = true;

                        ViewportContent = null;
                    })
                    .Select(x => x.model)
                    .WhereNotNull()
                    .Select(file =>
                    {
                        return file is null
                            ? Observable.Return(null as Model3DGroup)
                            : Observable.Start(() =>
                            {
                                var material = Materials.LightGray;
                                var model = new ModelImporter
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
                            if (model is null) // happens when a new item has been loaded while the scheduler was queued
                                return null;
                            ViewportContent = model;
                            BringIntoView.Handle(Unit.Default).Subscribe();
                            IsLoading = false;
                            return null;
                        });
                    })
            });

            _isActivated = ReactiveObjectHelper.GetIsActivatedChanges(this).ToProperty(this, vm => vm.IsActivated);
        }

        [Reactive]
        public bool IsLoading { get; set; }

        [Reactive]
        public Message PlaceholderText { get; set; }

        [Reactive]
        public Model3DGroup ViewportContent { get; set; }

        public Interaction<Unit, Unit> BringIntoView { get; } = new();
        public HelixViewport3D Viewport { get; set; } //dirty but works
        public ViewModelActivator Activator { get; } = new();

        [Reactive]
        public ModelFilePath Model { get; set; }

        [Reactive]
        public Message PlaceholderMessage { get; set; }

        public bool IsActivated => _isActivated.Value;

        /// <summary>
        /// </summary>
        /// <param name="sourceName">i.e. ItemName</param>
        public ImageFilePath GenerateThumbnail(Name sourceName)
            => _thumbnailService.GenerateThumbnail(Model, sourceName, Viewport);
    }
}