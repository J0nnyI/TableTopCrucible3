using ReactiveUI;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;
using System.Windows.Input;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Infrastructure.Repositories;
using TableTopCrucible.Infrastructure.Repositories.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Wpf.Commands.RepositoryEdit
{
    [Singleton(typeof(MasterDirectoryCommandBuilder))]
    public interface IMasterDirectoryCommandBuilder
    {
        ICommand AddOrUpdate([NotNull] IObservable<MasterDirectory> masterDirChanges);
        ICommand DeleteCommand([NotNull] IObservable<MasterDirectoryId> masterDirId);
    }
    public class MasterDirectoryCommandBuilder : IMasterDirectoryCommandBuilder
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
