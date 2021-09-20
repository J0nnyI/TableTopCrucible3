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

        public SourceTracker(Name name, TrackingTarget trackingTarget)
        {
            this.Name = name;
            if (trackingTarget != null)
                SetTarget(trackingTarget);
        }

        public Name Name { get; }

        public void Dispose()
        {
            _targetProgressChanges.Dispose();
            _increments.Dispose();
        }

        private readonly ReplaySubject<ProgressIncrement> _increments = new();
        public IObservable<CurrentProgress> CurrentProgressChanges =>
            _increments
                .Scan((CurrentProgress)0, (acc, inc) => acc + inc);

        private readonly BehaviorSubject<TrackingTarget> _targetProgressChanges = new((TrackingTarget)1);
        public IObservable<TrackingTarget> TargetProgressChanges => _targetProgressChanges.AsObservable();
        public Name Title { get; }

        public void SetTarget(TrackingTarget trackingTarget)
            => _targetProgressChanges.OnNext(trackingTarget);

        public void Increment(ProgressIncrement increment)
            => _increments.OnNext(increment);
    }

    internal class WeightedSourceTracker : SourceTracker, IWeightedTrackingViewer
    {
        public WeightedSourceTracker(Name name, TrackingTarget trackingTarget, TrackingWeight weight):base(name, trackingTarget)
        {
            this.Weight = weight;
        }
        public TrackingWeight Weight { get; }
    }
}
