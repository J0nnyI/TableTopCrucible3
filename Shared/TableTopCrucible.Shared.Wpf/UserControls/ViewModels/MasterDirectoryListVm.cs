﻿using DynamicData;
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
using Splat;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Infrastructure.Repositories;
using TableTopCrucible.Infrastructure.Repositories.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Models.ValueTypes;

using vtName = TableTopCrucible.Core.ValueTypes.Name;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    [Transient(typeof(FileArchiveListVm))]
    public interface IFileArchiveList
    {

    }
    public class FileArchiveListVm : ReactiveValidationObject, IActivatableViewModel, IFileArchiveList
    {
        private readonly IFileArchiveRepository _fileArchiveRepository;

        [Reactive]
        public string Directory { get; set; }
        [Reactive]
        public string Name { get; set; }

        public ICommand CreateDirectory { get; private set; }
        public FileArchiveListVm(IFileArchiveRepository fileArchiveRepository)
        {
            _fileArchiveRepository = fileArchiveRepository;


            this.WhenActivated(() => new[]
            {
                _fileArchiveRepository
                    .DataChanges
                    .Connect()
                    .Transform(m=>
                    {
                        var card = Locator.Current.GetService<IFileArchiveCard>();
                        card.FileArchive = m;
                        return card;
                    })
                    .Sort()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(Directories)
                    .Subscribe(),
                _initCommands(),

                FileArchivePath.RegisterValidator(this, 
                    vm => vm.Directory,
                    true,
                    _fileArchiveRepository.TakenDirectoriesChanges
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
                            _fileArchiveRepository.AddOrUpdate(new FileArchive()
                            {
                                Path = FileArchivePath.From(Directory),
                                Name = vtName.From(Name),
                            })), this.WhenAnyValue(v=>v.HasErrors).Do(_=>{}).Select(x=>!x)

                    , RxApp.TaskpoolScheduler, RxApp.MainThreadScheduler
                ).DisposeWith(disposables);
            return disposables;
        }
        public ObservableCollectionExtended<IFileArchiveCard> Directories { get; } = new();
        public ViewModelActivator Activator { get; } = new();
    }
}
