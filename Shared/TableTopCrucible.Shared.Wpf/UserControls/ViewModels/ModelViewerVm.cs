using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Media.Media3D;

using HelixToolkit.Wpf;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Helper;
using TableTopCrucible.Infrastructure.Repositories.Services;
using TableTopCrucible.Shared.Wpf.Services;

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
        private readonly IWpfThumbnailGenerationService _thumbnailService;

        private readonly ObservableAsPropertyHelper<bool> _isActivated;

        public ModelViewerVm(
            IWpfThumbnailGenerationService thumbnailService)
        {
            _thumbnailService = thumbnailService;
            this.WhenActivated(() => new[]
            {
                new ActOnLifecycle(()=>{},()=>{}),
                this.WhenAnyValue(
                        vm => vm.Model)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Do(x =>
                    {// set loading info
                        PlaceholderText =
                            PlaceholderMessage ??
                            (x is null
                                ? (Message)"No File selected"
                                : (Message)"Loading");
                        if (x is null ||
                            PlaceholderMessage is not null ||
                            x.GetSize().Value > SettingsHelper.FileMinLoadingScreenSize
                           )
                            IsLoading = true;

                        ViewportContent = null;
                    })
                    .Select(x => x)
                    .WhereNotNull()
                    .Select(file =>
                    {
                        return file is null
                            ? Observable.Return(null as Model3DGroup)
                            : Observable.Start(() =>
                            {
                                var model = file.Load(true);
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

            _isActivated = this.GetIsActivatedChanges()
                .Buffer(TimeSpan.FromMilliseconds(200))
                .Select(buffer => buffer.FirstOrDefault())
                .WhereNotNull()
                .DistinctUntilChanged()
                .Do(x => { })
                .Publish()
                .RefCount()
                .ToProperty(this, vm => vm. IsActivated);
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
            => _thumbnailService.Generate(Model, sourceName, Viewport);
    }
}