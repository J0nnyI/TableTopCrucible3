using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Engine.Services;
using TableTopCrucible.Core.Engine.ValueTypes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Services;
using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;
using TableTopCrucible.Domain.Library.Services;
using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Services;
using TableTopCrucible.Shared.ItemSync.Services;
using TableTopCrucible.Shared.Services;
using TableTopCrucible.Shared.ValueTypes;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels
{
    [Transient]
    public interface IItemActions
    {
        public Item Item { get; set; }
        ICommand GenerateThumbnailsCommand { get; set; }

    }
    public class ItemActionsVm:ReactiveObject, IItemActions, IActivatableViewModel
    {
        public ViewModelActivator Activator { get; } = new();
        public ICommand StartSyncCommand { get; }
        public ICommand DeleteAllDataCommand { get; }
        public ICommand GenerateThumbnailsCommand { get; set; }
        public Interaction<Unit, YesNoDialogResult> DeletionConfirmation { get; } = new();
        [Reactive]
        public Item Item { get; set; }

        [Reactive]
        public IItemModelViewer ItemModelViewer { get; set; }

        /// <summary>
        /// <inheritdoc cref="IModelViewer.GetCameraView"/>
        /// </summary>
        /// <returns></returns>
        public IObservable<CameraView> GetCurrentCameraView { get; }

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
                                    (Description)string.Join(Environment.NewLine,"all Files, Items and or Images could not be removed","Error:",e.ToString()),
                                    NotificationType.Error);
                            }
                        }
                    );
            });
        }

    }
}
