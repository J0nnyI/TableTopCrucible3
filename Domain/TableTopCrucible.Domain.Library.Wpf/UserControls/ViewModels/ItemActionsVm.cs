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
using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;
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
        public ViewModelActivator Activator { get; } = new();
        public ICommand StartSyncCommand { get; }
        public ICommand DeleteAllDataCommand { get; }
        public Interaction<Unit, YesNoDialogResult> DeletionConfirmation { get; } = new();

        public ItemActionsVm(IFileSynchronizationService fileSynchronizationService,
            IFileRepository fileRepository,
            IItemRepository itemRepository)
        {
            this.StartSyncCommand = fileSynchronizationService.StartScanCommand;


            DeleteAllDataCommand = ReactiveCommand.Create(() =>
            {
                DeletionConfirmation.Handle(Unit.Default)
                    .Where(res => res == YesNoDialogResult.Yes)
                    .Subscribe(res =>
                        {
                            fileRepository.RemoveRange(fileRepository.Data);
                            itemRepository.RemoveRange(itemRepository.Data);
                        }
                    );
            });
        }

    }
}
