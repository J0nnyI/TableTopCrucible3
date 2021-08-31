using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Text;
using ReactiveUI;
using TableTopCrucible.Infrastructure.Repositories.Models.Entities;

namespace TableTopCrucible.Infrastructure.Wpf.Commands.RepositoryEdit
{
    public class AddOrUpdateMasterDirectory:ReactiveCommand<MasterDirectory, Unit>
    {
        protected internal AddOrUpdateMasterDirectory(
            Func<MasterDirectory,
            IObservable<Unit>> execute, 
            IObservable<bool>? canExecute, 
            IScheduler? outputScheduler,
            IScheduler? canExecuteScheduler) : base(execute, canExecute, outputScheduler, canExecuteScheduler)
        {
        }
    }
}
