using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Concurrency;

// ReSharper disable once CheckNamespace
namespace ReactiveUI;

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
        [NotNull] Action<ReactiveCommand<Unit, Unit>> resultWriter = null,
        IScheduler outputScheduler = null,
        IScheduler canExecuteScheduler = null)
    {
        var result = ReactiveCommand.Create(execute, canExecute, outputScheduler, canExecuteScheduler);
        resultWriter!.Invoke(result);
        return result;
    }

    public static ReactiveCommand<Unit, Unit> Create(
        Action execute,
        [NotNull] Action<ReactiveCommand<Unit, Unit>> resultWriter = null,
        IScheduler outputScheduler = null,
        IScheduler canExecuteScheduler = null)
        => Create(execute, null, resultWriter, outputScheduler, canExecuteScheduler);

    public static ReactiveCommand<T, Unit> Create<T>(
        Action<T> execute,
        [NotNull] Action<ReactiveCommand<T, Unit>> resultWriter = null,
        IScheduler outputScheduler = null,
        IScheduler canExecuteScheduler = null)
    {
        var res = ReactiveCommand.Create(execute, null, outputScheduler, canExecuteScheduler);
        resultWriter!.Invoke(res);
        return res;
    }

    public static ReactiveCommand<T, Unit> ToCommand<T>(Action<T> action, IObservable<bool> canExecute, IScheduler outputScheduler = null, IScheduler canExecuteScheduler = null)
        => ReactiveCommand.Create(action,canExecute,canExecuteScheduler, outputScheduler);
}