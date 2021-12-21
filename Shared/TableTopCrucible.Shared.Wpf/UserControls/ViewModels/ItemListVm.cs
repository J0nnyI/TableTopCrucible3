using System;
using System.Windows.Input;
using DynamicData.Binding;
using ReactiveUI;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Infrastructure.Models.Entities;
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

        public ObservableCollectionExtended<FileData> Files = new();
        public ObservableCollectionExtended<Item> Items = new();

        public ItemListVm(
            IScannedFileRepository fileRepository,
            IFileSynchronizationService fileSynchronizationService,
            IItemRepository itemRepository)
        {
            _fileRepository = fileRepository;
            _fileSynchronizationService = fileSynchronizationService;

            throw new NotImplementedException("has to be rewritten");
            //this.WhenActivated(() => new[]{

            //fileRepository
            //    .Cache
            //    .Connect()
            //    .ObserveOn(RxApp.MainThreadScheduler)
            //    .Bind(Files)
            //    .Subscribe(),
            //itemRepository
            //    .Cache
            //    .Connect()
            //    .ObserveOn(RxApp.MainThreadScheduler)
            //    .Bind(Items)
            //    .Subscribe(),
            //});
        }

        public ICommand FileSyncCommand => _fileSynchronizationService.StartScanCommand;
        public ViewModelActivator Activator { get; } = new();
    }
}