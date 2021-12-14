using DynamicData;
using DynamicData.Binding;

using ReactiveUI;

using System;
using System.Reactive.Linq;
using System.Windows.Input;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Infrastructure.Models.Models;
using TableTopCrucible.Infrastructure.Repositories;
using TableTopCrucible.Infrastructure.Repositories.Services;
using TableTopCrucible.Shared.ItemSync.Services;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    [Transient]
    public interface IItemList
    {

    }
    public class ItemListVm : ReactiveObject, IItemList, IActivatableViewModel
    {
        private readonly IScannedFileRepository _fileRepository;
        private readonly IFileSynchronizationService _fileSynchronizationService;
        public ViewModelActivator Activator { get; } = new();

        public ObservableCollectionExtended<ScannedFileDataEntity> Files = new();
        public ObservableCollectionExtended<ItemModel> Items= new();
        public ICommand FileSyncCommand => _fileSynchronizationService.StartScanCommand;

        public ItemListVm(
            IScannedFileRepository fileRepository,
            IFileSynchronizationService fileSynchronizationService,
            IItemRepository itemRepository)
        {
            _fileRepository = fileRepository;
            _fileSynchronizationService = fileSynchronizationService;

            this.WhenActivated(() => new[]{

                fileRepository
                    .Cache
                    .Connect()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(Files)
                    .Subscribe(),
                itemRepository
                    .Cache
                    .Connect()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(Items)
                    .Subscribe(),
            });
        }
    }
}
