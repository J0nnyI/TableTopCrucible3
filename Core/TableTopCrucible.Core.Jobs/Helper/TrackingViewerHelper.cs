using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using Splat;
using TableTopCrucible.Core.Engine.Services;
using TableTopCrucible.Core.Engine.ValueTypes;
using TableTopCrucible.Core.Jobs.Progression.Models;
using TableTopCrucible.Core.Jobs.Progression.Services;
using TableTopCrucible.Core.Jobs.ValueTypes;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Jobs.Helper;

public interface ISubscribedTrackingViewer : IDisposable, ITrackingViewer
{
    public CurrentProgress CurrentProgress { get; }
    public TargetProgress TargetProgress { get; }
    public JobState JobState { get; }
}

internal class SubscribedTrackingViewer : ReactiveObject, ISubscribedTrackingViewer
{
    private readonly CompositeDisposable _permanentDisposables = new();
    private ObservableAsPropertyHelper<CurrentProgress> _currentProgress;
    private CompositeDisposable _disposables;
    private ObservableAsPropertyHelper<JobState> _jobState;
    private ObservableAsPropertyHelper<TargetProgress> _targetProgress;

    internal SubscribedTrackingViewer(ITrackingViewer source, ViewModelActivator parentActivator = null,
        IScheduler scheduler = null)
    {
        Source = source;
        Activator = parentActivator;
        if (parentActivator == null)
        {
            _bindValues(scheduler);
        }
        else
        {
            parentActivator.Activated
                .Subscribe(_ => _bindValues(scheduler))
                .DisposeWith(_permanentDisposables);
            parentActivator.Deactivated
                .Subscribe(_ => _disposables.Dispose())
                .DisposeWith(_permanentDisposables);
        }
    }

    public ITrackingViewer Source { get; }
    public ViewModelActivator Activator { get; }

    public IObservable<CurrentProgress> CurrentProgressChanges => Source.CurrentProgressChanges;
    public CurrentProgress CurrentProgress => _currentProgress.Value;

    public IObservable<TargetProgress> TargetProgressChanges => Source.TargetProgressChanges;
    public TargetProgress TargetProgress => _targetProgress.Value;

    public IObservable<JobState> JobStateChanges => Source.JobStateChanges;
    public JobState JobState => _jobState.Value;

    public Name Title => Source.Title;


    public void Dispose()
    {
        if (_permanentDisposables.IsDisposed)
            return;
        _permanentDisposables.Dispose();
    }

    public void _bindValues(IScheduler scheduler)
    {
        _disposables = new CompositeDisposable(
            CurrentProgressChanges.ToProperty(
                this,
                vm => vm.CurrentProgress,
                out _currentProgress,
                false,
                scheduler)
            ,
            TargetProgressChanges.Do(x => { }).ToProperty(
                this,
                vm => vm.TargetProgress,
                out _targetProgress,
                false,
                scheduler)
            ,
            JobStateChanges.ToProperty(
                this,
                vm => vm.JobState,
                out _jobState,
                false,
                scheduler)
        ).DisposeWith(_permanentDisposables);
    }

    public override string ToString()
    {
        var type = "";
        if (Source is WeightedCompositeTracker)
            type = "Weighted Composite";
        if (Source is CompositeTracker)
            type = "Composite";
        if (Source is WeightedSourceTracker)
            type = "Weighted Source";
        if (Source is SourceTracker)
            type = "Source";

        var progPercent = string.Empty;
        if (CurrentProgress != null && TargetProgress != null)
            progPercent = $" : ({CurrentProgress.Value / TargetProgress.Value * 100}%)";

        return $"{Title} - {type} - {JobState}: {CurrentProgress} / {TargetProgress} {progPercent}";
    }
}

public static class TrackingViewerHelper
{
    // provides properties for all observables of the tracking viewer
    public static ISubscribedTrackingViewer Subscribe(
        this ITrackingViewer viewer,
        ViewModelActivator parentActivator = null,
        IScheduler scheduler = null) =>
        new SubscribedTrackingViewer(viewer, parentActivator, scheduler);

    public static IObservable<CurrentProgressPercent> GetCurrentProgressInPercent(
        this ITrackingViewer viewer) =>
        viewer.CurrentProgressChanges.CombineLatest(viewer.TargetProgressChanges,
            CurrentProgressPercent.From);

    public static IObservable<Unit> OnDone(this ITrackingViewer viewer)
    {
        return viewer.JobStateChanges.Where(x => x == JobState.Done).Select(_ => Unit.Default);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="list"></param>
    /// <param name="action"></param>
    /// <param name="filter">when false, the action will not be applied to this item</param>
    /// <param name="trackerName"></param>
    /// <param name="errorTitle"></param>
    /// <param name="scheduler">default = taskpool</param>
    /// <param name="successTitle"></param>
    /// <param name="successDescription"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ITrackingViewer TrackedForEachAsync<T>(
        this IEnumerable<T> list,
        Func<T,bool> filter,
        Action<T> action,
        Name trackerName,
        Name successTitle = null,
        Description successDescription = null,
        Name errorTitle = null,
        IScheduler scheduler = null)
    {
        var enumeratedList = list.ToArray();
        var tracker = Locator.Current.GetService<IProgressTrackingService>()
            !.CreateSourceTracker(trackerName, (TargetProgress)enumeratedList.Length);
        var notificationService = Locator.Current.GetService<INotificationService>()!;
        Observable.Start(() =>
            {
                try
                {
                    foreach (var entry in enumeratedList)
                    {
                        if(filter(entry))
                            action(entry);
                        tracker.Increment();
                    }

                    tracker.Complete();
                    if (successTitle is not null)
                        notificationService.AddConfirmation(successTitle, successDescription);
                }
                catch (Exception e)
                {
                    if (errorTitle is not null)
                        notificationService.AddError(e, errorTitle);
                    else
                        throw;
                }
            }, scheduler ?? RxApp.TaskpoolScheduler)
            .Take(1).Subscribe();
        return tracker;
    }
}