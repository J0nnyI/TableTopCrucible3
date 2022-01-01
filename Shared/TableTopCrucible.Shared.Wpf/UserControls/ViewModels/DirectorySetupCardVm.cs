using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Helpers;
using Splat;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Wpf.Engine.Services;
using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;
using TableTopCrucible.Core.Wpf.Helper;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;
using TableTopCrucible.Infrastructure.Repositories.Services;
using vtName = TableTopCrucible.Core.ValueTypes.Name;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    [Transient]
    public interface IDirectorySetupCard : IComparable<IDirectorySetupCard>, IComparable
    {
        public DirectorySetupId DirectorySetupId { get; set; }
        public DirectorySetup DirectorySetup { get; }
        public bool ResetOnSave { get; set; }
    }

    public class DirectorySetupCardVm : ReactiveValidationObject, IActivatableViewModel, IDirectorySetupCard
    {
        private readonly IDirectorySetupRepository _directorySetupRepository;
        private readonly INotificationService _notificationService;
        private ObservableAsPropertyHelper<DirectorySetup> _directorySetup;

        private ObservableAsPropertyHelper<bool> _isDirty;

        public DirectorySetupCardVm()
        {
            _directorySetupRepository = Locator.Current.GetService<IDirectorySetupRepository>();
            _notificationService = Locator.Current.GetService<INotificationService>();

            this.WhenActivated(() => new[]
                {
                    // Properties
                    this.WhenAnyValue(
                            vm => vm.DirectorySetupId,
                            id => _directorySetupRepository.Watch(id))
                        .Switch()
                        .Select(change => change)
                        .Do(m =>
                        {
                            Name = m?.Name?.Value;
                            Path = m?.Path?.Value;
                        })
                        .ToProperty(this, vm => vm.DirectorySetup, out _directorySetup),

                    this.WhenAnyValue(
                            vm => vm.DirectorySetup,
                            vm => vm.Name,
                            vm => vm.Path,
                            (dir, Name, path) =>
                                dir?.Name?.Value != Name || dir?.Path?.Value != path)
                        .OutputObservable(out var isDirtyChanges)
                        .ToProperty(this, vm => vm.IsDirty, out _isDirty),

                    // Validation
                    vtName.RegisterValidator(this, vm => vm.Name),
                    //DirectorySetupEntity.RegisterValidator(this, vm=>vm.Path),

                    // Commands
                    // save changes
                    ReactiveCommandHelper.Create(() =>
                        {
                            //_directorySetupRepository.Update(new DirectorySetupChangeSet(
                            //    (vtName)Name,
                            //    (DirectorySetupEntity)Path,
                            //    DirectorySetup.Id));
                            _notificationService.AddNotification(
                                (vtName)"Directory saved successfully",
                                (Description)$"The directory '{Name}' has been saved successfully",
                                NotificationType.Confirmation);
                        },
                        this.WhenAnyValue(
                            vm => vm.IsDirty,
                            vm => vm.HasErrors,
                            (isDirty, hasErrors) => isDirty && !hasErrors),
                        c => SaveChangesCommand = c
                    ),

                    // undo changes
                    ReactiveCommandHelper.Create(() =>
                        {
                            Name = DirectorySetup.Name.Value;
                            Path = DirectorySetup.Path.Value;
                            _notificationService.AddNotification(
                                (vtName)"Directory undo successful",
                                (Description)$"The changes in directory '{Name}' have been undone successfully",
                                NotificationType.Confirmation);
                        },
                        isDirtyChanges,
                        c => UndoChangesCommand = c
                    ),

                    // remove directory
                    ReactiveCommandHelper.Create(() =>
                        {
                            ConfirmDeletionInteraction.Handle(Unit.Default)
                                .Take(1)
                                .Where(confirmed => confirmed == YesNoDialogResult.Yes)
                                .Subscribe(_ =>
                                {
                                    _directorySetupRepository.Remove(DirectorySetup.Id);
                                    _notificationService.AddNotification(
                                        (vtName)"Remove successful",
                                        (Description)$"The Directory '{Name}' has been removed from this list",
                                        NotificationType.Confirmation);
                                });
                        },
                        c => RemoveDirectoryCommand = c
                    )
                },
                vm => vm.DirectorySetup
            );
        }

        [Reactive]
        public string Name { get; set; }

        [Reactive]
        public string Path { get; set; }

        public bool IsDirty => _isDirty.Value;

        public Interaction<Unit, YesNoDialogResult> ConfirmDeletionInteraction { get; } = new();

        public ICommand SaveChangesCommand { get; private set; }
        public ICommand UndoChangesCommand { get; private set; }
        public ICommand RemoveDirectoryCommand { get; private set; }

        public ViewModelActivator Activator { get; } = new();
        public DirectorySetup DirectorySetup => _directorySetup?.Value;

        [Reactive]
        public DirectorySetupId DirectorySetupId { get; set; }

        [Reactive]
        public bool ResetOnSave { get; set; } = false;

        public int CompareTo(IDirectorySetupCard other)
            => DirectorySetup?.Name?.CompareTo(other?.DirectorySetup?.Name) ?? 1;

        public int CompareTo(object obj)
            => CompareTo(obj as IDirectorySetupCard);
    }
}