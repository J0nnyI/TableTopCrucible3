using DynamicData;
using DynamicData.Binding;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Reactive.Linq;
using System.Windows.Input;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Infrastructure.Repositories;
using TableTopCrucible.Infrastructure.Repositories.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Models.ValueTypes;

using vtName = TableTopCrucible.Core.ValueTypes.Name;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    [Transient(typeof(MasterDirectoryListVm))]
    public interface IMasterDirectoryList
    {

    }
    public class MasterDirectoryListVm : ReactiveObject, IActivatableViewModel, IMasterDirectoryList
    {
        [Reactive]
        public string Directory { get; set; }
        [Reactive]
        public string Name { get; set; }

        public ICommand CreateDirectory { get; }
        public MasterDirectoryListVm(IMasterDirectoryRepository masterDirectoryRepository)
        {
            CreateDirectory = ReactiveCommand.Create(() =>
                Observable.Start(() =>
                    masterDirectoryRepository.AddOrUpdate(new MasterDirectory()
                    {
                        Path = MasterDirectoryPath.From(Directory),
                        Name = vtName.From(Name),
                    })),
                this.WhenAnyValue(
                    vm => vm.Directory,
                    vm => vm.Name,
                    (path, name) =>
                        path != null && MasterDirectoryPath.From(path).Exists()
                ), RxApp.TaskpoolScheduler, RxApp.MainThreadScheduler
            );
            this.WhenActivated(() => new[]
            {
                masterDirectoryRepository
                    .Data
                    .Connect()
                    .Bind(Directories)
                    .Subscribe()
            });
        }
        public ObservableCollectionExtended<MasterDirectory> Directories { get; } = new();
        public ViewModelActivator Activator { get; } = new();
    }
}
