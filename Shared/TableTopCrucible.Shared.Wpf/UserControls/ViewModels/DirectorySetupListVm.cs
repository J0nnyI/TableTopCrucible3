using DynamicData;
using DynamicData.Binding;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Helpers;

using Splat;

using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Forms;
using System.Windows.Input;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Wpf.Engine.Services;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;
using TableTopCrucible.Infrastructure.Repositories;
using TableTopCrucible.Infrastructure.Repositories.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Models.ValueTypes;

using vtName = TableTopCrucible.Core.ValueTypes.Name;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    [Transient(typeof(DirectorySetupListVm))]
    public interface IDirectorySetupList
    {

    }
    public class DirectorySetupListVm : ReactiveValidationObject, IActivatableViewModel, IDirectorySetupList
    {
        private readonly IDirectorySetupRepository _directorySetupRepository;
        private readonly INotificationService _notificationService;

        public Interaction<Unit, DirectorySetupPath> GetDirectoryDialog { get; } = new();

        public ICommand CreateDirectory { get; private set; }
        public DirectorySetupListVm(IDirectorySetupRepository directorySetupRepository, INotificationService notificationService)
        {
            _directorySetupRepository = directorySetupRepository;
            _notificationService = notificationService;


            this.WhenActivated(() => new[]
            {
                _directorySetupRepository
                    .DataChanges
                    .Connect()
                    .Transform(m=>m.Id)
                    .IgnoreUpdateWhen((m1, m2)=>m1==m2)
                    .Transform(m=>
                    {
                        var card = Locator.Current.GetService<IDirectorySetupCard>();
                        card.DirectorySetupId = m;
                        return card;
                    })
                    .Sort()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(Directories)
                    .Subscribe(),

                _initCommands(),
            });
        }

        private IDisposable _initCommands()
        {
            var disposables = new CompositeDisposable();
            CreateDirectory =
                ReactiveCommand.Create(async () =>
                {
                    var path = await GetDirectoryDialog.Handle(Unit.Default);
                    if (path == null)
                        return;
                    var takenItem = _directorySetupRepository.Data.Items.FirstOrDefault(e => e.Path == path);
                    if (takenItem == null)
                    {
                        var directorySetup = new DirectorySetup(path.GetDirectoryName().ToName(), path);
                        _directorySetupRepository.AddOrUpdate(directorySetup);
                        _notificationService.AddNotification(
                            "Directory added successfully",
                            $"The directory '{directorySetup.Path}' has been added as '{directorySetup.Name}'",
                            NotificationType.Confirmation);
                    }
                    else
                    {
                        _notificationService.AddNotification(
                            "Directory has already been added",
                            $"The directory '{takenItem.Path.Value}' has already been added as '{takenItem.Name.Value}'",
                            NotificationType.Info);
                    }
                });
            return disposables;
        }
        public ObservableCollectionExtended<IDirectorySetupCard> Directories { get; } = new();
        public ViewModelActivator Activator { get; } = new();
    }
}
