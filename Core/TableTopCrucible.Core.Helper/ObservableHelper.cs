using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData.Kernel;

namespace TableTopCrucible.Core.Helper;

public struct PreviousAndCurrentValue<T>
{
    internal PreviousAndCurrentValue(Optional<T> previous, Optional<T> current,
        ObservableHelper.PcValueType previousType)
    {
        Previous = previous;
        Current = current;
        Type = ObservableHelper.PcValueType.Full;
        if (previousType == ObservableHelper.PcValueType.Seed)
            Type = ObservableHelper.PcValueType.Initial;
        // seed => initial
        // initial => full
        // full => full
    }

    public Optional<T> Previous { get; }
    public Optional<T> Current { get; }
    internal ObservableHelper.PcValueType Type { get; init; }
}

public static class ObservableHelper
{
    // returns the previous and then current value, waiting for the 2nd update to push values
    public static IObservable<PreviousAndCurrentValue<T>> Pairwise<T>(this IObservable<T> src,
        bool skipInitial = true)
    {
        return src.Scan(new PreviousAndCurrentValue<T> { Type = PcValueType.Seed },
                (previous, current) =>
                    new PreviousAndCurrentValue<T>(previous.Current, current, previous.Type))
            .Where(x =>
                x.Type == PcValueType.Full
                || !skipInitial && x.Type == PcValueType.Initial);
    }

    public static IObservable<T> OutputObservable<T>(this IObservable<T> src, out IObservable<T> newObservable)
    {
        newObservable = src;
        return src;
    }

    /// <summary>
    /// starts a countdown timer for the given duration which updates in said resolution until it reaches 0.
    /// <br/>
    /// uses replay and concat to always instantly provide the current value, even for subscriptions after completion
    /// <br/> <br/>
    /// <b>!!! removing all subscribers from the resulting observable resets the countdown !!!</b>
    /// </summary>
    /// <param name="forTime"></param>
    /// <param name="isRunningChanges">default is Observable.Return(true)</param>
    /// <param name="cancel">updating this observable cancels the countdown and the next value will be <see cref="TimeSpan.Zero"/>. default is observable.never</param>
    /// <param name="resolution">default is <see cref="SettingsHelper.AnimationResolution"/></param>
    /// <param name="scheduler">default is <see cref="Scheduler.CurrentThread"/></param>
    /// <returns>an observable which contains the time until the timer is completed</returns>
    public static IObservable<TimeSpan> StartTimer(this TimeSpan forTime, IObservable<bool> isRunningChanges = null,
        IObservable<Unit> cancel = null, TimeSpan? resolution = null, IScheduler scheduler = null)
    {
        var incrementSize = resolution ?? SettingsHelper.AnimationResolution;
        isRunningChanges ??= Observable.Return(true);
        cancel ??= Observable.Never<Unit>();
        return Observable.Interval(incrementSize, scheduler ?? Scheduler.CurrentThread)
            .CombineLatest(
                isRunningChanges, (_, isRunning) => isRunning)
            .Scan(forTime, (acc, isRunning)
                => isRunning
                    ? acc - incrementSize
                    : acc)
            .TakeWhile(time => time > TimeSpan.Zero)
            .TakeUntil(cancel)
            .Concat(Observable.Return(TimeSpan.Zero))
            .Replay(1)
            .RefCount();
    }

    public static IObservable<double> AnimateValue(double from, double to, TimeSpan? duration = null,
        IScheduler scheduler = null)
    {
        var frameCount = (duration ?? SettingsHelper.AnimationTimeSpan) /
                         SettingsHelper.AnimationResolution;
        var stepSize = (to - from) / frameCount;

        return AnimateValue(from, v => v + stepSize, duration, scheduler);
    }

    public static IObservable<T> AnimateValue<T>(T seed, Func<T, T> accumulator, TimeSpan? duration = null,
        IScheduler scheduler = null)
    {
        return Observable.Interval(SettingsHelper.AnimationResolution, scheduler ?? Scheduler.CurrentThread)
            .Take(MathHelper.CeilingInt(
                (duration ?? SettingsHelper.AnimationTimeSpan) /
                SettingsHelper.AnimationResolution
            ))
            .Scan(seed, (acc, _) => accumulator(acc));
    }

    /// <summary>
    ///     connects to the source until the compositeDisposable is disposed of
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="disposeWith"></param>
    /// <returns></returns>
    public static IConnectableObservable<T> ConnectUntil<T>(this IConnectableObservable<T> source,
        CompositeDisposable disposeWith)
    {
        source.Connect().DisposeWith(disposeWith);
        return source;
    }

    public static void OnNext(this ISubject<Unit> subject)
        => subject.OnNext(Unit.Default);

    internal enum PcValueType : byte
    {
        Seed = 1,
        Initial = 2,
        Full = 3
    }

    public static IObservable<Unit> SelectUnit<T>(this IObservable<T> obs)
        => obs.Select(_ => Unit.Default);
}