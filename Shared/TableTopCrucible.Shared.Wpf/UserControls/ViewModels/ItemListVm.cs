using System;
using System.Reactive.Linq;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Wpf.Helper;
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
        private readonly IFileRepository _fileRepository;
        private readonly IFileSynchronizationService _fileSynchronizationService;

        public ObservableCollectionExtended<FileData> Files = new();
        public ObservableCollectionExtended<Item> Items = new();

        public ItemListVm(
            IFileRepository fileRepository,
            IFileSynchronizationService fileSynchronizationService,
            IItemRepository itemRepository)
        {
            _fileRepository = fileRepository;
            _fileSynchronizationService = fileSynchronizationService;

            this.WhenActivated(disposable => new[]{

            fileRepository
                .Updates
                .ToObservableCache(disposable)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(Files)
                .Subscribe(),
            itemRepository
                .Updates
                .ToObservableCache(disposable)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(Items)
                .Subscribe(),
            });
        }

        public ICommand FileSyncCommand => _fileSynchronizationService.StartScanCommand;
        public ViewModelActivator Activator { get; } = new();
    }
}