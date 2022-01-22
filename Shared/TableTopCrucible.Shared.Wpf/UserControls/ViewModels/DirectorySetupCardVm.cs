using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Engine.Services;
using TableTopCrucible.Core.Engine.ValueTypes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;
using TableTopCrucible.Core.Wpf.Helper;
using TableTopCrucible.Infrastructure.DataPersistence;
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
        private readonly IStorageController _storageController;
        private ObservableAsPropertyHelper<DirectorySetup> _directorySetup;

        private ObservableAsPropertyHelper<bool> _isDirty;

        public DirectorySetupCardVm(
            IDirectorySetupRepository directorySetupRepository,
            INotificationService notificationService,
            IStorageController storageController
        )
        {
            _directorySetupRepository = directorySetupRepository;
            _notificationService = notificationService;
            _storageController = storageController;

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
                            vm => vm.DirectorySetup.Name,
                            vm => vm.DirectorySetup.Path,
                            vm => vm.Name,
                            vm => vm.Path,
                            (savedName, savedPath, Name, path) =>
                                savedName?.Value != Name || savedPath?.Value != path)
                        .OutputObservable(out var isDirtyChanges)
                        .ToProperty(this, vm => vm.IsDirty, out _isDirty),

                    // Validation
                    vtName.RegisterValidator(this, vm => vm.Name),
                    //DirectorySetupEntity.RegisterValidator(this, vm=>vm.Path),

                    // Commands
                    // save changes
                    ReactiveCommandHelper.Create(() =>
                        {
                            try
                            {
                                DirectorySetup.Name = (vtName)Name;
                                DirectorySetup.Path = (DirectoryPath)Path;
                                _storageController.AutoSave();
                                _notificationService.AddNotification(
                                    (vtName)"Directory saved successfully",
                                    (Description)$"The directory '{Name}' has been saved successfully",
                                    NotificationType.Confirmation);
                            }
                            catch (Exception e)
                            {
                                _notificationService.AddNotification(
                                    (vtName)"Directory could not be saved",
                                    (Description)string.Join(Environment.NewLine,
                                        $"The directory '{Name}' could not be saved.", "Error:", e.ToString()),
                                    NotificationType.Error);
                                throw;
                            }
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
                            try
                            {
                                Name = DirectorySetup.Name.Value;
                                Path = DirectorySetup.Path.Value;

                                _notificationService.AddNotification(
                                    (vtName)"Directory undo successful",
                                    (Description)$"The changes in directory '{Name}' have been undone successfully",
                                    NotificationType.Confirmation);
                            }
                            catch (Exception e)
                            {
                                _notificationService.AddNotification(
                                    (vtName)"Directory undo failed",
                                    (Description)string.Join(Environment.NewLine,
                                        $"The directory changes for '{Name}' could not be undone.", "Error:",
                                        e.ToString()),
                                    NotificationType.Error);
                                throw;
                            }
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
                    ),

                    // change path
                    ReactiveCommandHelper.Create<DirectoryPath>(path =>
                    {
                        var takenDir = _directorySetupRepository[path];

                        if (takenDir is not null)
                        {
                            _notificationService.AddNotification((vtName)"Directory could not be changed",
                                (Description)
                                $"The Directory '{takenDir.Path}' has already been added as '{takenDir.Name}'.",
                                NotificationType.Error);
                            return;
                        }

                        Path = path.Value;
                        Name = path.GetDirectoryName().ToName().Value;
                        _storageController.AutoSave();
                    }, c => changePathCommand = c)
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
        public ReactiveCommandBase<DirectoryPath, Unit> changePathCommand { get; private set; }

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