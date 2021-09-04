using DynamicData;
using DynamicData.Binding;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Helpers;

using Splat;

using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
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

        [Reactive]
        public string Directory { get; set; }
        [Reactive]
        public string Name { get; set; }

        public Interaction<Unit, FileArchivePath> GetDirectoryDialog { get; } = new();

        public ICommand CreateDirectory { get; private set; }
        public FileArchiveListVm(IFileArchiveRepository fileArchiveRepository)
        {
            _fileArchiveRepository = fileArchiveRepository;


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
                    _fileArchiveRepository.AddOrUpdate(new FileArchive()
                    {
                        Path = path,
                        Name = path.GetDirectoryName().ToName(),
                    });
                });
            return disposables;
        }
        public ObservableCollectionExtended<IFileArchiveCard> Directories { get; } = new();
        public ViewModelActivator Activator { get; } = new();
    }
}
