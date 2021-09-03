using DynamicData;
using DynamicData.Binding;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using HelixToolkit.Wpf;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
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
    public class MasterDirectoryListVm : ReactiveValidationObject, IActivatableViewModel, IMasterDirectoryList
    {
        private readonly IMasterDirectoryRepository _masterDirectoryRepository;

        [Reactive]
        public string Directory { get; set; }
        [Reactive]
        public string Name { get; set; }

        public ICommand CreateDirectory { get; private set; }
        public MasterDirectoryListVm(IMasterDirectoryRepository masterDirectoryRepository)
        {
            _masterDirectoryRepository = masterDirectoryRepository;


            this.WhenActivated(() => new[]
            {
                _masterDirectoryRepository
                    .DataChanges
                    .Connect()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(Directories)
                    .Subscribe(),
                _initCommands(),
                // bug: button has error update does not work, tb error detection after path-clear not working
                MasterDirectoryPath.RegisterValidator(this, 
                    vm => vm.Directory,
                    true,
                    _masterDirectoryRepository
                        .DataChanges
                        .Connect()
                        .Transform(m=>m.Path)
                        .ToCollection()
                    ),
                vtName.RegisterValidator(this, vm => vm.Name),
            });
        }

        private IDisposable _initCommands()
        {
            var disposables = new CompositeDisposable();
            CreateDirectory =
                ReactiveCommand.Create(() =>
                        Observable.Start(() =>
                            _masterDirectoryRepository.AddOrUpdate(new MasterDirectory()
                            {
                                Path = MasterDirectoryPath.From(Directory),
                                Name = vtName.From(Name),
                            })), this.WhenAnyValue(v=>v.HasErrors).Do(_=>{}).Select(x=>!x)

                    , RxApp.TaskpoolScheduler, RxApp.MainThreadScheduler
                ).DisposeWith(disposables);
            return disposables;
        }
        public ObservableCollectionExtended<MasterDirectory> Directories { get; } = new();
        public ViewModelActivator Activator { get; } = new();
    }
}
