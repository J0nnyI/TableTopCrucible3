using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;

using ReactiveUI;

// ReSharper disable once CheckNamespace
namespace ReactiveUI
{
    public static class ReactiveCommandHelper
    {
        public static ReactiveCommand<Unit, Unit> Create(
            Action execute,
            out ReactiveCommand<Unit, Unit> result,
            IScheduler outputScheduler = null,
            IScheduler canExecuteScheduler = null)
            => Create(execute, null, out result, outputScheduler, canExecuteScheduler);
        public static ReactiveCommand<Unit, Unit> Create(
            Action execute,
            [AllowNull] IObservable<bool> canExecute,
            out ReactiveCommand<Unit, Unit> result,
            IScheduler outputScheduler = null,
            IScheduler canExecuteScheduler = null)
            => result = ReactiveCommand.Create(execute, canExecute, outputScheduler, canExecuteScheduler);
        public static ReactiveCommand<Unit, Unit> Create(
            Action execute,
            IObservable<bool> canExecute = null,
            Action<ReactiveCommand<Unit, Unit>> resultWriter = null,
            IScheduler outputScheduler = null,
            IScheduler canExecuteScheduler = null)
        {
            var result = ReactiveCommand.Create(execute, canExecute, outputScheduler, canExecuteScheduler);
            resultWriter(result);
            return result;
        }
        public static ReactiveCommand<Unit, Unit> Create(
            Action execute,
            Action<ReactiveCommand<Unit, Unit>> resultWriter = null,
            IScheduler outputScheduler = null,
            IScheduler canExecuteScheduler = null)
            => Create(execute, null, resultWriter, outputScheduler, canExecuteScheduler);
    }
}
