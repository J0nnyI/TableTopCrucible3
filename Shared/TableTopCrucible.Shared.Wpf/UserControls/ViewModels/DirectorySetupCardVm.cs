using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Helpers;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using Splat;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Wpf.Engine.Services;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;
using TableTopCrucible.Core.Wpf.Helper;
using TableTopCrucible.Infrastructure.Repositories;
using TableTopCrucible.Infrastructure.Repositories.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Models.EntityIds;
using TableTopCrucible.Infrastructure.Repositories.Models.ValueTypes;

using vtName = TableTopCrucible.Core.ValueTypes.Name;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    [Transient(typeof(DirectorySetupCardVm))]
    public interface IDirectorySetupCard : IComparable<IDirectorySetupCard>, IComparable
    {
        public DirectorySetupId DirectorySetupId { get; set; }
        public DirectorySetup DirectorySetup { get; }
        public bool ResetOnSave { get; set; }
    }
    public class DirectorySetupCardVm : ReactiveValidationObject, IActivatableViewModel, IDirectorySetupCard
    {
        private ObservableAsPropertyHelper<DirectorySetup> _directorySetup;
        public DirectorySetup DirectorySetup => _directorySetup?.Value;
        [Reactive]
        public DirectorySetupId DirectorySetupId { get; set; }

        [Reactive]
        public bool ResetOnSave { get; set; } = false;

        [Reactive]
        public string Name { get; set; }
        [Reactive]
        public string Path { get; set; }

        private ObservableAsPropertyHelper<bool> _isDirty;
        public bool IsDirty => _isDirty.Value;

        public Interaction<Unit, bool> ConfirmDeletionInteraction { get; }= new();
        
        private readonly IDirectorySetupRepository _directorySetupRepository;
        private readonly INotificationService _notificationService;

        public ICommand SaveChangesCommand { get; private set; }
        public ICommand UndoChangesCommand { get; private set; }
        public ICommand RemoveDirectoryCommand { get; private set; }

        public DirectorySetupCardVm()
        {
            this._directorySetupRepository = Locator.Current.GetService<IDirectorySetupRepository>();
            this._notificationService = Locator.Current.GetService<INotificationService>();

            this.WhenActivated(() => new[]
            {
                // Properties
                this.WhenAnyValue(
                        vm => vm.DirectorySetupId,
                        id => _directorySetupRepository.DataChanges.Watch(id))
                    .Switch()
                    .Select(change=>change.Current)
                    .Do(m =>
                    {
                        Name = m?.Name?.Value;
                        Path = m?.Path?.Value;
                    })
                    .ToProperty(this, vm=>vm.DirectorySetup, out _directorySetup),

                this.WhenAnyValue(
                        vm=>vm.DirectorySetup,
                        vm=>vm.Name,
                        vm=>vm.Path,
                        (dir, Name, path) =>
                            dir?.Name?.Value != Name || dir?.Path?.Value != path)
                    .OutputObservable(out var isDirtyChanges)
                    .ToProperty(this, vm=>vm.IsDirty,out _isDirty),

                // Validation
                vtName.RegisterValidator(this, vm=>vm.Name),
                DirectorySetupPath.RegisterValidator(this, vm=>vm.Path),

                // Commands
                ReactiveCommandHelper.Create(() =>
                    {
                        _directorySetupRepository.AddOrUpdate(new(Name, Path, DirectorySetup.Id));
                        _notificationService.AddNotification(
                            "Directory saved successfully",
                            $"The directory '{Name}' has been saved successfully",
                            NotificationType.Confirmation);
                    },
                    this.WhenAnyValue(
                        vm => vm.IsDirty,
                        vm => vm.HasErrors,
                        (isDirty, hasErrors) => isDirty && !hasErrors),
                    c => SaveChangesCommand = c
                ),
                ReactiveCommandHelper.Create(() =>
                    {
                        Name = DirectorySetup.Name.Value;
                        Path = DirectorySetup.Path.Value;
                        _notificationService.AddNotification(
                            "Directory undo successful",
                            $"The changes in directory '{Name}' have been undone successfully",
                            NotificationType.Confirmation);
                    },
                    isDirtyChanges,
                    c=>UndoChangesCommand =c
                ),
                ReactiveCommandHelper.Create(() =>
                    {
                        this.ConfirmDeletionInteraction.Handle(Unit.Default)
                            .Take(1)
                            .Where(confirmed => confirmed)
                            .Subscribe(_ =>
                            {
                                _directorySetupRepository.Delete(DirectorySetup.Id);
                                _notificationService.AddNotification(
                                    "Remove successful",
                                    $"The Directory '{Name}' has been removed from this list",
                                    NotificationType.Confirmation);
                            });
                    },
                    c=>RemoveDirectoryCommand = c
                ),
            },
                vm=>vm.DirectorySetup
            );
        }

        public int CompareTo(IDirectorySetupCard other)
            => this.DirectorySetup?.CompareTo(other?.DirectorySetup) ?? 1;

        public int CompareTo(object obj)
            => CompareTo(obj as IDirectorySetupCard);

        public ViewModelActivator Activator { get; } = new();
    }
}
