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
    [Transient(typeof(FileArchiveListVm))]
    public interface IFileArchiveList
    {

    }
    public class FileArchiveListVm : ReactiveValidationObject, IActivatableViewModel, IFileArchiveList
    {
        private readonly IFileArchiveRepository _fileArchiveRepository;
        private readonly INotificationService _notificationService;

        [Reactive]
        public string Directory { get; set; }
        [Reactive]
        public string Name { get; set; }

        public Interaction<Unit, FileArchivePath> GetDirectoryDialog { get; } = new();

        public ICommand CreateDirectory { get; private set; }
        public FileArchiveListVm(IFileArchiveRepository fileArchiveRepository, INotificationService notificationService)
        {
            _fileArchiveRepository = fileArchiveRepository;
            _notificationService = notificationService;


            this.WhenActivated(() => new[]
            {
                _fileArchiveRepository
                    .DataChanges
                    .Connect()
                    .Transform(m=>
                    {
                        var card = Locator.Current.GetService<IFileArchiveCard>();
                        card.FileArchive = m;
                        return card;
                    })
                    .Sort()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(Directories)
                    .Subscribe(),

                _initCommands(),

                FileArchivePath.RegisterValidator(this,
                    vm => vm.Directory,
                    true,
                    _fileArchiveRepository.TakenDirectoriesChanges
                    ),
                vtName.RegisterValidator(this, vm => vm.Name),
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
                    var takenItem = _fileArchiveRepository.Data.Items.FirstOrDefault(e => e.Path == path);
                    if (takenItem == null)
                    {
                        var newArchive = new FileArchive()
                        {
                            Path = path,
                            Name = path.GetDirectoryName().ToName(),
                        };
                        _fileArchiveRepository.AddOrUpdate(newArchive);
                        _notificationService.AddNotification(
                            "Archive added successfully", 
                            $"The Directory '{newArchive.Path}' has been added as Archive '{newArchive.Name}'",
                            NotificationType.Confirmation);
                    }
                    else
                    {
                        _notificationService.AddNotification(
                            "Archive has already been added",
                            $"This directory has already been registered as archive under the name {takenItem.Name.Value}",
                            NotificationType.Info);
                    }
                });
            return disposables;
        }
        public ObservableCollectionExtended<IFileArchiveCard> Directories { get; } = new();
        public ViewModelActivator Activator { get; } = new();
    }
}
