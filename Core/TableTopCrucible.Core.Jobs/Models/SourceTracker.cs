using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using TableTopCrucible.Core.Jobs.ValueTypes;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Jobs.Models
{
    // tracks a single progress
    internal class SourceTracker : ISourceTrackerController, ITrackingViewer
    {

        public SourceTracker(Name title, TrackingTarget trackingTarget)
        {
            this.Title = title;
            if (trackingTarget != null)
                SetTarget(trackingTarget);

            _accumulatedProgressChanges =
                _increments
                    .Scan((CurrentProgress)0, (acc, inc) => acc + inc)
                    .StartWith((CurrentProgress)0);

            CurrentProgressChanges =
                Observable.CombineLatest(
                    _accumulatedProgressChanges,
                    TargetProgressChanges,
                    _completedChanges,
                    (current, target, completed) => completed ? (CurrentProgress)target : current
                );
        }

        public Name Title { get; }

        public void Dispose()
        {
            if (!_completedChanges.Value)
                OnCompleted();
            _completedChanges.Dispose();
            _targetProgressChanges.Dispose();
            _increments.Dispose();
        }

        public void OnCompleted()
        {
            if (_completedChanges.Value)
                return;
            _completedChanges.OnNext(true);
            _completedChanges.OnCompleted();
            _targetProgressChanges.OnCompleted();
            _increments.OnCompleted();
        }
        private readonly BehaviorSubject<bool> _completedChanges = new(false);

        public IObservable<JobState> JobStateChanges => Observable.CombineLatest(
            _accumulatedProgressChanges,
            TargetProgressChanges,
            _completedChanges,
            (current, target, completed) =>
            {
                if (current == (TrackingTarget) 0)
                    return JobState.ToDo;
                if (current >= target || completed)
                    return JobState.Done;
                return JobState.InProgress;
            })
            .DistinctUntilChanged();

        private readonly ReplaySubject<ProgressIncrement> _increments = new();
        private IObservable<CurrentProgress> _accumulatedProgressChanges { get; }

        public IObservable<CurrentProgress> CurrentProgressChanges { get; }

        private readonly BehaviorSubject<TrackingTarget> _targetProgressChanges = new((TrackingTarget)1);
        public IObservable<TrackingTarget> TargetProgressChanges => _targetProgressChanges.AsObservable();

        public void SetTarget(TrackingTarget trackingTarget)
            => _targetProgressChanges.OnNext(trackingTarget);

        public void Increment(ProgressIncrement increment)
            => _increments.OnNext(increment ?? (ProgressIncrement)1);
        public override string ToString()
            => $"S {Title}";
    }

    internal class WeightedSourceTracker : SourceTracker, IWeightedTrackingViewer
    {
        public WeightedSourceTracker(Name title, TrackingTarget trackingTarget, TrackingWeight weight) : base(title, trackingTarget)
        {
            this.Weight = weight ?? TrackingWeight.Default;
        }
        public TrackingWeight Weight { get; }
        public override string ToString()
            => $"WS {Title} - {Weight}";
    }
}
