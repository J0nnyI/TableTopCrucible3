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
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Services;
using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Services;
using TableTopCrucible.Shared.ItemSync.Services;

namespace TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels
{
    [Transient]
    public interface IItemActions
    {

    }
    public class ItemActionsVm:ReactiveObject, IItemActions, IActivatableViewModel
    {
        private readonly IImageDataRepository _imageRepository;
        private readonly INotificationService _notificationService;
        public ViewModelActivator Activator { get; } = new();
        public ICommand StartSyncCommand { get; }
        public ICommand DeleteAllDataCommand { get; }
        public Interaction<Unit, YesNoDialogResult> DeletionConfirmation { get; } = new();

        public ItemActionsVm(IFileSynchronizationService fileSynchronizationService,
            IFileRepository fileRepository,
            IItemRepository itemRepository,
            IImageDataRepository imageRepository,
            INotificationService notificationService)
        {
            _imageRepository = imageRepository;
            _notificationService = notificationService;
            this.StartSyncCommand = fileSynchronizationService.StartScanCommand;


            DeleteAllDataCommand = ReactiveCommand.Create(() =>
            {
                DeletionConfirmation.Handle(Unit.Default)
                    .Where(res => res == YesNoDialogResult.Yes)
                    .Subscribe(res =>
                        {
                            try
                            {
                                imageRepository.RemoveRange(imageRepository.Data);
                                itemRepository.RemoveRange(itemRepository.Data);
                                fileRepository.RemoveRange(fileRepository.Data);
                                _notificationService.AddNotification((Name)"Removal sucessfull",
                                    (Description)"all Files, Items & Images have been removed successfully",
                                    NotificationType.Confirmation);
                            }
                            catch (Exception e)
                            {
                                _notificationService.AddNotification((Name)"Removal failed",
                                    (Description)string.Join(Environment.NewLine,"all Files, Items and or Images could not be removed","Error:",e.ToString()),
                                    NotificationType.Error);
                            }
                        }
                    );
            });
        }

    }
}
