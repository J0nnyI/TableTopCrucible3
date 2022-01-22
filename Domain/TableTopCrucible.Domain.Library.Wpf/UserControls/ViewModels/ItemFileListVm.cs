using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Services;

namespace TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels
{
    [Transient]
    public interface IItemFileList : IDisposable
    {
        Item Item { get; set; }
    }

    public class ItemFileListVm : ReactiveObject, IItemFileList, IActivatableViewModel
    {
        private readonly CompositeDisposable _disposables = new();

        public ItemFileListVm(IFileRepository fileRepository)
        {
            this.WhenActivated(() => new[]
            {
                this.WhenAnyValue(vm => vm.Item)
                    .Select(item => item?.FileKey3d)
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

        public ObservableCollection<FileData> Files { get; } = new();
        public ViewModelActivator Activator { get; } = new();

        [Reactive]
        public Item Item { get; set; }

        public void Dispose()
            => _disposables.Dispose();
    }
}