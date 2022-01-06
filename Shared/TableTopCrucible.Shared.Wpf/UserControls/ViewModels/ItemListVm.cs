using System;
using System.Collections.Generic;
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
using System.Linq;
using System.Reactive.Disposables;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    [Transient]
    public interface IItemList : IDisposable
    {
        IObservableList<Item> SelectedItems { get; }
    }

    public class ItemListVm : ReactiveObject, IItemList, IActivatableViewModel
    {
        private readonly IFileRepository _fileRepository;
        private readonly IFileSynchronizationService _fileSynchronizationService;

        public ObservableCollectionExtended<FileData> Files = new();
        public ObservableCollectionExtended<Item> Items = new();
        private SourceList<Item> _selectedItems = new();
        public IObservableList<Item> SelectedItems => _selectedItems.AsObservableList();
        private readonly CompositeDisposable _disposables = new();

        public ItemListVm(
            IFileRepository fileRepository,
            IFileSynchronizationService fileSynchronizationService,
            IItemRepository itemRepository)
        {
            _fileRepository = fileRepository;
            _fileSynchronizationService = fileSynchronizationService;
            _selectedItems.DisposeWith(_disposables);

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

        public void UpdateSelection(IEnumerable<Item> selectedItems, IEnumerable<Item> deselectedItems)
        {
            _selectedItems.Edit(list =>
            {
                list.AddRange(selectedItems);
                list.RemoveMany(deselectedItems);
            });
        }
        public ICommand FileSyncCommand => _fileSynchronizationService.StartScanCommand;
        public ViewModelActivator Activator { get; } = new();
        public void Dispose()
            => _disposables.Dispose();
    }
}