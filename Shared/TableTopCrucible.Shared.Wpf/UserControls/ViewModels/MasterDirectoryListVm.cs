using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using TableTopCrucible.Infrastructure.Repositories;
using TableTopCrucible.Infrastructure.Repositories.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Models.EntityIds;
using TableTopCrucible.Infrastructure.Repositories.Models.ValueTypes;
using TableTopCrucible.Infrastructure.Wpf.Commands.RepositoryEdit;
using TableTopCtucible.Core.DependencyInjection.Attributes;
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
                ),RxApp.TaskpoolScheduler, RxApp.MainThreadScheduler
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
