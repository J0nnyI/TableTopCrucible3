using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Helpers;

using System;
using System.Windows.Input;

using TableTopCrucible.Core.DependencyInjection.Attributes;
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
        public bool IsDirty => _isDirty.Value;

        public ICommand SaveChangesCommand { get; }
        public ICommand UndoChangesCommand { get; }
        public ICommand RemoveDirectoryCommand { get; }

        public FileArchiveCardVm()
        {
            this.WhenActivated(() => new[]
            {
                this.WhenAnyValue(vm=>vm.FileArchive)
                    .Subscribe(m =>
                    {
                        Name = m.Name.Value;
                        Path = m.Path.Value;
                    }),

                this.WhenAnyValue(
                    vm=>vm.FileArchive,
                    vm=>vm.Name,
                    vm=>vm.Path,
                    (dir, Name, path) =>
                        dir.Name.Value != Name || dir.Path.Value != path)
                    .ToProperty(this, vm=>vm.IsDirty,out _isDirty),

                vtName.RegisterValidator(this, vm=>vm.Name),
                FileArchivePath.RegisterValidator(this, vm=>vm.Path),
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
