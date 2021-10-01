using ReactiveUI;

using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using TableTopCrucible.Core.Jobs.Progression.Models;
using TableTopCrucible.Core.Jobs.Progression.ValueTypes;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Jobs.Helper
{
    public interface ISubscribedTrackingViewer : IDisposable, ITrackingViewer
    {
        public CurrentProgress CurrentProgress { get; }
        public TargetProgress TargetProgress { get; }
        public JobState JobState { get; }
    }


    internal class SubscribedTrackingViewer : ReactiveObject, ISubscribedTrackingViewer
    {
        public ITrackingViewer Source { get; }

        public IObservable<CurrentProgress> CurrentProgressChanges => Source.CurrentProgressChanges;
        private ObservableAsPropertyHelper<CurrentProgress> _currentProgress;
        public CurrentProgress CurrentProgress => _currentProgress.Value;

        public IObservable<TargetProgress> TargetProgressChanges => Source.TargetProgressChanges;
        private ObservableAsPropertyHelper<TargetProgress> _targetProgress;
        public TargetProgress TargetProgress => _targetProgress.Value;

        public IObservable<JobState> JobStateChanges => Source.JobStateChanges;
        private ObservableAsPropertyHelper<JobState> _jobState;
        public JobState JobState => _jobState.Value;

        public Name Title => Source.Title;
        public ViewModelActivator Activator { get; }
        private CompositeDisposable _disposables;
        private readonly CompositeDisposable _permanentDisposables = new();

        internal SubscribedTrackingViewer(ITrackingViewer source, ViewModelActivator parentActivator = null, IScheduler scheduler = null)
        {
            Source = source;
            this.Activator = parentActivator;
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

        public void _bindValues(IScheduler scheduler)
        {
            _disposables = new CompositeDisposable(new IDisposable[]
            {
                CurrentProgressChanges.ToProperty(
                    this,
                    vm=>vm.CurrentProgress,
                    out _currentProgress,
                    false,
                    scheduler ?? Scheduler.CurrentThread),
                TargetProgressChanges.ToProperty(
                    this,
                    vm=>vm.TargetProgress,
                    out _targetProgress,
                    false,
                    scheduler ?? Scheduler.CurrentThread),
                JobStateChanges.ToProperty(
                    this,
                    vm=>vm.JobState,
                    out _jobState,
                    false,
                    scheduler ?? Scheduler.CurrentThread)
            }).DisposeWith(_permanentDisposables);
        }


        public void Dispose()
        {
            if (_permanentDisposables.IsDisposed)
                return;
            _permanentDisposables.Dispose();
        }

        public override string ToString()
        {
            string type = "";
            if (Source is WeightedCompositeTracker)
                type = "Weighted Composite";
            if (Source is CompositeTracker)
                type = "Composite";
            if (Source is WeightedSourceTracker)
                type = "Weighted Source";
            if (Source is SourceTracker)
                type = "Source";
            return $"{CurrentProgress} / {TargetProgress} ({CurrentProgress.Value / TargetProgress.Value * 100}%) - " + type;
        }
    }
    public static class TrackingViewerHelper
    {
        // provides properties for all observables of the tracking viewer
        public static ISubscribedTrackingViewer Subscribe(
            this ITrackingViewer viewer,
            ViewModelActivator parentActivator = null,
            IScheduler scheduler = null)
            => new SubscribedTrackingViewer(viewer, parentActivator, scheduler);

        public static IObservable<CurrentProgressPercent> GetCurrentProgressInPercent(
            this ITrackingViewer viewer)
            => Observable.CombineLatest(
                viewer.CurrentProgressChanges,
                viewer.TargetProgressChanges,
                CurrentProgressPercent.From);

        public static IObservable<Unit> OnDone(this ITrackingViewer viewer)
            => viewer.JobStateChanges.Where(x => x == JobState.Done).Select(_ => Unit.Default);

    }
}
