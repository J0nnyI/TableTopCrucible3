using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Services;

namespace TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels
{
    [Transient]
    public interface IItemFileList: IDisposable
    {
        Item Item { get; set; }
    }
    public class ItemFileListVm : ReactiveObject, IItemFileList, IActivatableViewModel
    {
        public ViewModelActivator Activator { get; } = new(); 
        private readonly CompositeDisposable _disposables = new();
        [Reactive]
        public Item Item { get; set; }

        public ObservableCollection<FileData> Files { get; } = new();

        public ItemFileListVm(IFileRepository fileRepository)
        {
            this.WhenActivated(()=>new []
            {
                this.WhenAnyValue(vm => vm.Item)
                    .Select(item=>item?.FileKey3d)
                    .Select(fileRepository.Watch)
                    .Switch()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .OnItemAdded(file =>
                    {
                        Files.Clear();
                        Files.Add(file);

                    })
                    .Subscribe()
            });
        }
        
        public void Dispose()
            => _disposables.Dispose();
    }
}
