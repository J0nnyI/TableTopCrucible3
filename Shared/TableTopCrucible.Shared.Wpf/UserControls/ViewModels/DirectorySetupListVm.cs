using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Validation.Helpers;
using Splat;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Engine.Services;
using TableTopCrucible.Core.Engine.ValueTypes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Helper;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Services;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    [Transient]
    public interface IDirectorySetupList
    {
    }

    public class DirectorySetupListVm : ReactiveValidationObject, IActivatableViewModel, IDirectorySetupList
    {
        private readonly IDirectorySetupRepository _directorySetupRepository;
        private readonly INotificationService _notificationService;

        public DirectorySetupListVm(IDirectorySetupRepository directorySetupRepository,
            INotificationService notificationService)
        {
            _directorySetupRepository = directorySetupRepository;
            _notificationService = notificationService;

            HintOpacity =
                this.WhenAnyValue(vm => vm.Directories.Count)
                    .Select(c => c == 0)
                    .DistinctUntilChanged()
                    // causes ui lags
                    //.Select(show =>
                    //    ObservableHelper.AnimateValue(0, 1)
                    //        .Select(opacity => show ? opacity : 1 - opacity) // invert animation direction depending on the toggle
                    //)
                    //.Switch()
                    .Select(show => show
                        ? 1.0
                        : 0.0);

            this.WhenActivated(disposables => new[]
            {
                _directorySetupRepository
                    .Data
                    .Connect()
                    .Transform(dir =>
                    {
                        var card = Locator.Current.GetService<IDirectorySetupCard>();
                        card.DirectorySetupId = dir.Id;
                        return card;
                    })
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(Directories)
                    .Subscribe(),

                _initCommands()
            });
        }

        public Interaction<Unit, DirectoryPath> GetDirectoryDialog { get; } = new();

        public ICommand CreateDirectory { get; private set; }
        public IObservable<double> HintOpacity { get; }

        public ObservableCollectionExtended<IDirectorySetupCard> Directories { get; } = new();
        public ViewModelActivator Activator { get; } = new();

        private IDisposable _initCommands()
        {
            var disposables = new CompositeDisposable();
            CreateDirectory =
                ReactiveCommand.Create(async () =>
                {
                    try
                    {
                        var path = await GetDirectoryDialog.Handle(Unit.Default);
                        if (path == null)
                            return;
                        var takenItem = _directorySetupRepository[path];
                        if (takenItem == null)
                        {
                            var directorySetup = new DirectorySetup(path);
                            _directorySetupRepository.Add(directorySetup);
                            _notificationService.AddNotification(
                                (Name)"Directory added successfully",
                                (Description)
                                $"The directory '{directorySetup.Path}' has been added as '{directorySetup.Name}'",
                                NotificationType.Confirmation);
                        }
                        else
                        {
                            _notificationService.AddNotification(
                                (Name)"Directory has already been added",
                                (Description)
                                $"The directory '{takenItem.Path.Value}' has already been added as '{takenItem.Name.Value}'",
                                NotificationType.Info);
                        }
                    }
                    catch (Exception e)
                    {
                        _notificationService.AddNotification(
                            (Name)"Directory could not be added",
                            (Description)("The Directory could not be added:" + Environment.NewLine + e),
                            NotificationType.Error);
                    }
                });
            return disposables;
        }
    }
}