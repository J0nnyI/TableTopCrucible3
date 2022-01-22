using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Engine.Services;
using TableTopCrucible.Core.Engine.ValueTypes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;
using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels
{
    [Transient]
    public interface IItemActions
    {
        public Item Item { get; set; }
        ICommand GenerateThumbnailsCommand { get; set; }
    }

    public class ItemActionsVm : ReactiveObject, IItemActions, IActivatableViewModel
    {
        public ItemActionsVm(
            IFileSynchronizationService fileSynchronizationService,
            IStorageController storageController,
            INotificationService notificationService)
        {
            var notificationService1 = notificationService;
            StartSyncCommand = fileSynchronizationService.StartScanCommand;
            DeleteAllDataCommand = ReactiveCommand.Create(() =>
            {
                DeletionConfirmation.Handle(Unit.Default)
                    .Where(res => res == YesNoDialogResult.Yes)
                    .Subscribe(res =>
                        {
                            try
                            {
                                storageController.Files.Clear();
                                storageController.Images.Clear();
                                storageController.Items.Clear();
                                notificationService1.AddNotification((Name)"Removal sucessfull",
                                    (Description)"all Files, Items & Images have been removed successfully",
                                    NotificationType.Confirmation);
                            }
                            catch (Exception e)
                            {
                                notificationService1.AddNotification((Name)"Removal failed",
                                    (Description)string.Join(Environment.NewLine,
                                        "all Files, Items and or Images could not be removed", "Error:", e.ToString()),
                                    NotificationType.Error);
                            }
                        }
                    );
            });
        }

        public ICommand StartSyncCommand { get; }
        public ICommand DeleteAllDataCommand { get; }
        public Interaction<Unit, YesNoDialogResult> DeletionConfirmation { get; } = new();

        [Reactive]
        public IItemModelViewer ItemModelViewer { get; set; }

        /// <summary>
        ///     <inheritdoc cref="IModelViewer.GetCameraView" />
        /// </summary>
        /// <returns></returns>
        public IObservable<CameraView> GetCurrentCameraView { get; }

        public ViewModelActivator Activator { get; } = new();
        public ICommand GenerateThumbnailsCommand { get; set; }

        [Reactive]
        public Item Item { get; set; }
    }
}