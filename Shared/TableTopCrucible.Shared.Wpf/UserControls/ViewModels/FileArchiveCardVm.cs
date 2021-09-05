using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Helpers;

using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using Splat;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Infrastructure.Repositories;
using TableTopCrucible.Infrastructure.Repositories.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Models.ValueTypes;

using vtName = TableTopCrucible.Core.ValueTypes.Name;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    [Transient(typeof(FileArchiveCardVm))]
    public interface IFileArchiveCard : IComparable<IFileArchiveCard>, IComparable
    {
        public FileArchive FileArchive { get; set; }
        public bool ResetOnSave { get; set; }
    }
    public class FileArchiveCardVm : ReactiveValidationObject, IActivatableViewModel, IFileArchiveCard
    {
        [Reactive]
        public FileArchive FileArchive { get; set; }

        [Reactive]
        public bool ResetOnSave { get; set; } = false;

        [Reactive]
        public string Name { get; set; }
        [Reactive]
        public string Path { get; set; }

        private ObservableAsPropertyHelper<bool> _isDirty;
        private readonly IFileArchiveRepository? _fileArchiveRepository;
        public bool IsDirty => _isDirty.Value;

        public ICommand SaveChangesCommand { get; private set; }
        public ICommand UndoChangesCommand { get; private set; }
        public ICommand RemoveDirectoryCommand { get; private set; }

        public FileArchiveCardVm()
        {
            this._fileArchiveRepository = Locator.Current.GetService<IFileArchiveRepository>();

            this.WhenActivated(() => new[]
            {
                this.WhenAnyValue(
                        vm=>vm.FileArchive,
                        vm=>vm.Name,
                        vm=>vm.Path,
                        (dir, Name, path) =>
                            dir?.Name?.Value != Name || dir?.Path?.Value != path)
                    .ToProperty(this, vm=>vm.IsDirty,out _isDirty),

                // todo: undo / save / delete implementations
                _initializeCommands(),
                this.WhenAnyValue(vm=>vm.FileArchive)
                    .Subscribe(m =>
                    {
                        Name = m.Name.Value;
                        Path = m.Path.Value;
                    }),


                vtName.RegisterValidator(this, vm=>vm.Name),
                FileArchivePath.RegisterValidator(this, vm=>vm.Path),
            });
        }

        private IDisposable _initializeCommands()
        {
            var saveChanges = ReactiveCommand.Create(() =>
                {
                    _fileArchiveRepository.AddOrUpdate(new(Name, Path, FileArchive.Id));
                },
                this.WhenAnyValue(
                    vm => vm.IsDirty,
                    vm => vm.HasErrors,
                    (isDirty, hasErrors) => isDirty && !hasErrors)
            );
            var undoChanges = ReactiveCommand.Create(() =>
                {
                    this.Name = FileArchive.Name.Value;
                    this.Path = FileArchive.Path.Value;
                },
                this.WhenAnyValue(vm=>vm.IsDirty)
            );
            var removeDirectory = ReactiveCommand.Create(() =>
                {
                    _fileArchiveRepository.Delete(FileArchive.Id);
                }
            );

            this.SaveChangesCommand = saveChanges;
            this.UndoChangesCommand = undoChanges;
            this.RemoveDirectoryCommand = removeDirectory;

            return new CompositeDisposable(new IDisposable[]
            {
                saveChanges,
                undoChanges,
                removeDirectory,
            });
        }

        public int CompareTo(IFileArchiveCard other)
        {
            return this.FileArchive.CompareTo(other?.FileArchive);
        }

        public int CompareTo(object? obj)
        {
            return CompareTo(obj as IFileArchiveCard);
        }

        public ViewModelActivator Activator { get; } = new();
    }
}
