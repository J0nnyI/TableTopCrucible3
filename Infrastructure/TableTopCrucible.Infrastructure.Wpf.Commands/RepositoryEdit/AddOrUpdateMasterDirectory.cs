using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Input;

using ReactiveUI;

using TableTopCrucible.Infrastructure.Repositories;
using TableTopCrucible.Infrastructure.Repositories.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Wpf.Commands.RepositoryEdit
{
    public class MasterDirectoryCommandBuilder
    {
        private readonly IMasterDirectoryRepository _repository;

        public MasterDirectoryCommandBuilder(IMasterDirectoryRepository repository)
        {
            _repository = repository;
        }
        public ICommand AddOrUpdate([NotNull] IObservable<MasterDirectory> masterDirChanges)
        {
            return ReactiveCommand.Create<MasterDirectory>(
                masterDir => _repository.AddOrUpdate(masterDir),
                masterDirChanges.Select(md => md != null),
                RxApp.TaskpoolScheduler,
                RxApp.MainThreadScheduler);
        }

        public ICommand DeleteCommand([NotNull] IObservable<MasterDirectoryId> masterDirId)
        {
            return ReactiveCommand.Create<MasterDirectoryId>(
                id => _repository.Delete(id),
                masterDirId.Select(id => id != null),
                RxApp.TaskpoolScheduler,
                RxApp.MainThreadScheduler);
        }
    }
}
