using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

using DynamicData;
using DynamicData.Aggregation;

using TableTopCrucible.Core.Jobs.ValueTypes;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Jobs.Models
{
    // tracks a composite progress and returns the weighted total progress
    internal class CompositeTracker : ICompositeTrackerController, ITrackingViewer
    {
        IObservable<CurrentProgress> ITrackingViewer.CurrentProgressChanges =>
            CurrentProgressChanges.Select(prog => (CurrentProgress) prog);
        public IObservable<WeightedCurrentProgress> CurrentProgressChanges { get; }

        IObservable<TrackingTarget> ITrackingViewer.TargetProgressChanges =>
            TargetProgressChanges.Select(target => (TrackingTarget)target);
        public IObservable<WeightedTrackingTarget> TargetProgressChanges { get; }

        public Name Title { get; }

        public ICompositeTrackerController AddComposite(Name name, TrackingWeight weight = null)
        {
            var tracker = new WeightedCompositeTracker(name, weight);
            trackerSource.OnNext(tracker);
            return tracker;
        }

        public ISourceTrackerController AddSingle(Name name, TrackingTarget trackingTarget, TrackingWeight weight = null)
        {
            var tracker = new WeightedSourceTracker(name, trackingTarget, weight);
            trackerSource.OnNext(tracker);
            return tracker;
        }
        // each pushed item represents one tracker
        private readonly ReplaySubject<IWeightedTrackingViewer> trackerSource = new();
        public IObservable<IWeightedTrackingViewer> Tracker => trackerSource;

        public CompositeTracker(Name title)
        {
            this.Title = title;

            CurrentProgressChanges = trackerSource.Select(tracker =>
                tracker
                    .CurrentProgressChanges
                    .Select(progress => progress * tracker.Weight))
                .Merge()
                .Scan((WeightedCurrentProgress)0, (current, acc) => current + acc);

            TargetProgressChanges = trackerSource
                // collect all pushed observables in a flat list
                .Scan(
                    new List<IObservable<WeightedTrackingTarget>>(),
                    (list, tracker) =>
                    {
                        list.Add(tracker.TargetProgressChanges.Select(total => total * tracker.Weight));
                        return list;
                    })
                // combine that list into a observable which pushes updates for that list
                .Select(trackers => Observable.CombineLatest(trackers.ToArray()))
                .Switch()
                // summiere alle sub-tracker
                .Select(trackers =>
                    (WeightedTrackingTarget)trackers.Select(total => total.Value)
                        .Sum());

        }
    }

    internal class WeightedCompositeTracker : CompositeTracker, IWeightedTrackingViewer
    {
        public WeightedCompositeTracker(Name title, TrackingWeight weight) : base(title)
        {
            this.Weight = weight;
        }

        public TrackingWeight Weight { get; }
    }
}
