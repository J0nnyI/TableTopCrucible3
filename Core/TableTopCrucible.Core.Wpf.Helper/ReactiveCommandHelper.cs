using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

// ReSharper disable once CheckNamespace
namespace ReactiveUI;

public static class ReactiveCommandHelper
{
    public static ReactiveCommand<Unit, Unit> Create(
        Action execute,
        out ReactiveCommand<Unit, Unit> result,
        IScheduler outputScheduler = null)
        => Create(execute, null, out result, outputScheduler);

    public static ReactiveCommand<Unit, Unit> Create(
        Action execute,
        [AllowNull] IObservable<bool> canExecute,
        out ReactiveCommand<Unit, Unit> result,
        IScheduler outputScheduler = null)
        => result = ReactiveCommand.Create(execute, canExecute?.ObserveOn(RxApp.MainThreadScheduler), outputScheduler);

    public static ReactiveCommand<Unit, Unit> Create(
        Action execute,
        IObservable<bool> canExecute = null,
        [NotNull] Action<ReactiveCommand<Unit, Unit>> resultWriter = null,
        IScheduler outputScheduler = null)
    {
        var result = ReactiveCommand.Create(execute, canExecute?.ObserveOn(RxApp.MainThreadScheduler), outputScheduler);
        resultWriter!.Invoke(result);
        return result;
    }

    public static ReactiveCommand<Unit, Unit> Create(
        Action execute,
        [NotNull] Action<ReactiveCommand<Unit, Unit>> resultWriter = null,
        IScheduler outputScheduler = null)
        => Create(execute, null, resultWriter, outputScheduler);

    public static ReactiveCommand<T, Unit> Create<T>(
        Action<T> execute,
        [NotNull] Action<ReactiveCommand<T, Unit>> resultWriter = null,
        IScheduler outputScheduler = null)
    {
        var res = ReactiveCommand.Create(execute, null, outputScheduler);
        resultWriter!.Invoke(res);
        return res;
    }

    public static ReactiveCommand<T, Unit> ToCommand<T>(Action<T> action, IObservable<bool> canExecute, IScheduler outputScheduler = null)
        => ReactiveCommand.Create(action,canExecute?.ObserveOn(RxApp.MainThreadScheduler),outputScheduler);
}            


