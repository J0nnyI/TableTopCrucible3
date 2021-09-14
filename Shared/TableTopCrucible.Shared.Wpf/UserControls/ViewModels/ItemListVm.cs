using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Infrastructure.Repositories;
using TableTopCrucible.Infrastructure.Repositories.Models.Entities;
using TableTopCrucible.Shared.ItemSync.Services;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    [Transient(typeof(ItemListVm))]
    public interface IItemList
    {

    }
    public class ItemListVm : ReactiveObject, IItemList, IActivatableViewModel
    {
        private readonly IScannedFileRepository _fileRepository;
        private readonly IFileSynchronizationService _fileSynchronizationService;
        public ViewModelActivator Activator { get; } = new();

        public ObservableCollectionExtended<ScannedFileData> Files = new();
        public ICommand FileSyncCommand => _fileSynchronizationService.StartScanCommand;

        public ItemListVm(IScannedFileRepository fileRepository,
            IFileSynchronizationService fileSynchronizationService)
        {
            _fileRepository = fileRepository;
            _fileSynchronizationService = fileSynchronizationService;

            this.WhenActivated(()=>new []{

                fileRepository
                    .DataChanges
                    .Connect()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(Files)
                    .Subscribe()
            });
        }
    }
}
