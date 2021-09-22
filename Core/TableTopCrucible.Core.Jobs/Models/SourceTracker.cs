using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

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
            _currentProgressChanges,
            _targetProgressChanges,
            _completedChanges,
            (current, target, completed) =>
            {
                if (current == (TrackingTarget) 0)
                    return JobState.ToDo;
                if (current == target || completed)
                    return JobState.Done;
                return JobState.InProgress;
            });

        private readonly ReplaySubject<ProgressIncrement> _increments = new();
        private IObservable<CurrentProgress> _currentProgressChanges =>
            _increments
                .Scan((CurrentProgress)0, (acc, inc) => acc + inc)
                .StartWith((CurrentProgress)0);

        public IObservable<CurrentProgress> CurrentProgressChanges => // set progress = target if disposed
            Observable.CombineLatest(
                _currentProgressChanges,
                TargetProgressChanges,
                _completedChanges,
                (current, target, completed) => completed ? (CurrentProgress)target : current);

        private readonly BehaviorSubject<TrackingTarget> _targetProgressChanges = new((TrackingTarget)1);
        public IObservable<TrackingTarget> TargetProgressChanges => _targetProgressChanges.AsObservable();

        public void SetTarget(TrackingTarget trackingTarget)
            => _targetProgressChanges.OnNext(trackingTarget);

        public void Increment(ProgressIncrement increment)
            => _increments.OnNext(increment ?? (ProgressIncrement)1);
    }

    internal class WeightedSourceTracker : SourceTracker, IWeightedTrackingViewer
    {
        public WeightedSourceTracker(Name title, TrackingTarget trackingTarget, TrackingWeight weight) : base(title, trackingTarget)
        {
            this.Weight = weight;
        }
        public TrackingWeight Weight { get; }
    }
}
