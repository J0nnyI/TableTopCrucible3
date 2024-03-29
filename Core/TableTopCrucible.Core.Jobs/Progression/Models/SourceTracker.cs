﻿using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Jobs.ValueTypes;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Jobs.Progression.Models
{
    // tracks a single progress
    internal class SourceTracker : ISourceTracker, ITrackingViewer
    {
        private readonly IObservable<CurrentProgress> _accumulatedProgressChanges;
        private readonly BehaviorSubject<bool> _completedChanges = new(false);
        private readonly CompositeDisposable _disposables = new();

        private readonly ReplaySubject<ProgressIncrement> _increments = new();

        private readonly BehaviorSubject<TargetProgress> _targetProgressChanges = new((TargetProgress)1);

        public SourceTracker(Name title, TargetProgress trackingTarget)
        {
            _completedChanges.DisposeWith(_disposables);
            _increments.DisposeWith(_disposables);
            _targetProgressChanges.DisposeWith(_disposables);

            Title = title;
            if (trackingTarget != null)
                SetTarget(trackingTarget);

            _accumulatedProgressChanges =
                _increments
                    .StartWith((ProgressIncrement)0)
                    .Scan((CurrentProgress)0, (acc, inc) => acc + inc);

            CurrentProgressChanges =
                _accumulatedProgressChanges.CombineLatest(TargetProgressChanges,
                        _completedChanges,
                        (current, target, completed) =>
                            completed || current > target
                                ? (CurrentProgress)target
                                : current
                    )
                    .Replay(1)
                    .ConnectUntil(_disposables);
        }

        public Name Title { get; }

        public void Complete()
        {
            if (_completedChanges.Value)
                return;
            _completedChanges.OnNext(true);
        }

        public IObservable<JobState> JobStateChanges =>
            _accumulatedProgressChanges.Do(x => { }).CombineLatest(_targetProgressChanges.Do(x => { }),
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
                .DistinctUntilChanged()
                .Replay(1)
                .ConnectUntil(_disposables);

        public IObservable<CurrentProgress> CurrentProgressChanges { get; }

        public IObservable<TargetProgress> TargetProgressChanges =>
            _targetProgressChanges
                .AsObservable();

        public void SetTarget(TargetProgress trackingTarget)
            => _targetProgressChanges.OnNext(trackingTarget);

        public void Increment(ProgressIncrement increment)
        {
            lock (_increments)
            {
                _increments.OnNext(increment ?? (ProgressIncrement)1);
            }
        }

        public void Dispose()
        {
            // the completion has to be delayed since the b-subjects seem to push no longer values once they are completed (runtime only?)
            _completedChanges.OnCompleted();
            _targetProgressChanges.OnCompleted();
            lock (_increments)
            {
                _increments.OnCompleted();
            }

            _disposables.Dispose();
        }

        public override string ToString() => $"S {Title}";
    }

    internal class WeightedSourceTracker : SourceTracker, IWeightedTrackingViewer
    {
        public WeightedSourceTracker(Name title, TargetProgress trackingTarget, JobWeight weight)
            : base(title, trackingTarget)
        {
            Weight = weight ?? JobWeight.Default;
        }

        public JobWeight Weight { get; }

        public override string ToString() => $"WS {Title} - {Weight}";
    }
}