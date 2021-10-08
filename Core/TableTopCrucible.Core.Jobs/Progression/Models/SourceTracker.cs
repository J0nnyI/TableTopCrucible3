using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using TableTopCrucible.Core.Jobs.Progression.ValueTypes;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Jobs.Progression.Models
{
    // tracks a single progress
    internal class SourceTracker : ISourceTracker, ITrackingViewer
    {

        public SourceTracker(Name title, TargetProgress trackingTarget)
        {
            this.Title = title;
            if (trackingTarget != null)
                SetTarget(trackingTarget);

            _accumulatedProgressChanges =
                _increments
                    .StartWith((ProgressIncrement)0)
                    .Scan((CurrentProgress)0, (acc, inc) => acc + inc);

            CurrentProgressChanges =
                Observable.CombineLatest(
                    _accumulatedProgressChanges,
                    TargetProgressChanges,
                    _completedChanges,
                    (current, target, completed) => completed || current > target ? (CurrentProgress)target : current
                );
        }

        public Name Title { get; }
        private bool disposed = false;
        public void Dispose()
        {
            if (disposed)
                return;
            disposed = true;
            OnCompleted();
            _completedChanges.Dispose();
            _targetProgressChanges.Dispose();
            lock (_increments)
                _increments.Dispose();
        }

        public void OnCompleted()
        {
            if (_completedChanges.Value)
                return;
            _completedChanges.OnNext(true);
            _completedChanges.OnCompleted();
            _targetProgressChanges.OnCompleted();
            lock (_increments)
                _increments.OnCompleted();
        }
        private readonly BehaviorSubject<bool> _completedChanges = new(false);

        public IObservable<JobState> JobStateChanges => Observable.CombineLatest(
            _accumulatedProgressChanges.Do(x => { }),
            TargetProgressChanges.Do(x => { }),
            _completedChanges.Do(x => { }),
            (current, target, completed) =>
            {
                if (completed)
                    return JobState.Done;
                if (current == (TargetProgress)0)
                    return JobState.ToDo;
                if (current >= target)
                    return JobState.Done;
                return JobState.InProgress;
            })
            .DistinctUntilChanged();

        private readonly ReplaySubject<ProgressIncrement> _increments = new();
        private readonly IObservable<CurrentProgress> _accumulatedProgressChanges;

        public IObservable<CurrentProgress> CurrentProgressChanges { get; }

        private readonly BehaviorSubject<TargetProgress> _targetProgressChanges = new((TargetProgress)1);
        public IObservable<TargetProgress> TargetProgressChanges => _targetProgressChanges.AsObservable();

        public void SetTarget(TargetProgress trackingTarget)
            => _targetProgressChanges.OnNext(trackingTarget);

        public void Increment(ProgressIncrement increment)
        {
            lock (_increments)
                _increments.OnNext(increment ?? (ProgressIncrement)1);
        }
        public override string ToString()
            => $"S {Title}";
    }

    internal class WeightedSourceTracker : SourceTracker, IWeightedTrackingViewer
    {
        public WeightedSourceTracker(Name title, TargetProgress trackingTarget, JobWeight weight) : base(title, trackingTarget)
        {
            this.Weight = weight ?? JobWeight.Default;
        }
        public JobWeight Weight { get; }
        public override string ToString()
            => $"WS {Title} - {Weight}";
    }
}
