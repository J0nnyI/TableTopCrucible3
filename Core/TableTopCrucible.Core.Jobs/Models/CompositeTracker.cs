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
        public static readonly WeightedTrackingTarget Target = (WeightedTrackingTarget)100;

        IObservable<CurrentProgress> ITrackingViewer.CurrentProgressChanges =>
            CurrentProgressChanges.Select(prog => (CurrentProgress)prog);
        public IObservable<WeightedCurrentProgress> CurrentProgressChanges { get; }

        IObservable<TrackingTarget> ITrackingViewer.TargetProgressChanges =>
            TargetProgressChanges.Select(target => (TrackingTarget)target);

        public IObservable<WeightedTrackingTarget> TargetProgressChanges { get; } = Observable.Return(Target);

        public Name Title { get; }
        public IObservable<JobState> JobStateChanges { get; }

        public ICompositeTrackerController AddComposite(Name name = null, TrackingWeight weight = null)
        {
            var tracker = new WeightedCompositeTracker(name, weight);
            _trackerStack.OnNext(tracker);
            return tracker;
        }

        public ISourceTrackerController AddSingle(Name name = null, TrackingTarget trackingTarget = null, TrackingWeight weight = null)
        {
            var tracker = new WeightedSourceTracker(name, trackingTarget, weight);
            _trackerStack.OnNext(tracker);
            return tracker;
        }
        // each pushed item represents one tracker
        private readonly ReplaySubject<IWeightedTrackingViewer> _trackerStack = new();
        public IObservable<IWeightedTrackingViewer> TrackerStack => _trackerStack;

        private IObservable<IEnumerable<IWeightedTrackingViewer>> trackerListChanges { get; }

        public CompositeTracker(Name title)
        {
            this.Title = title;


            trackerListChanges = _trackerStack
                .Scan(
                    Enumerable.Empty<IWeightedTrackingViewer>(),
                    (list, tracker) =>
                        list.Append(tracker)
                    );

            CurrentProgressChanges = _trackerStack.SelectMany(src =>
                    src.CurrentProgressChanges.Select(current => current * src.Weight))
                .Scan((WeightedCurrentProgress)0, (acc, cur) => acc + cur);



            CurrentProgressChanges = trackerListChanges.Select(trackerList =>
                    {
                        var totalTrackerWeight = (TrackingWeight)trackerList.Sum(tracker => tracker.Weight.Value);

                        return Observable.CombineLatest( // get the weight of all trackers parallel as list
                            trackerList.Select(tracker =>
                                Observable.CombineLatest(
                                    tracker.CurrentProgressChanges,
                                    tracker.TargetProgressChanges,
                                    tracker.JobStateChanges,
                                    (current, target, state) =>
                                    {
                                        // target value relative to the others
                                        var targetFraction = tracker.Weight.Value / totalTrackerWeight.Value;
                                        var targetPercent = Target.Value * targetFraction; // how much of the composite progress is from this tracker?

                                        // catch over / under / miss fill
                                        switch (state)
                                        {
                                            case JobState.ToDo:
                                                return (WeightedCurrentProgress)0;
                                            case JobState.Done:
                                                return (WeightedCurrentProgress)(Target.Value * targetFraction);
                                        }

                                        // current value
                                        var filledPercent = current.Value / target.Value;
                                        if (filledPercent > 1) // catch overfill
                                            filledPercent = 1;

                                        return (WeightedCurrentProgress)(targetPercent * filledPercent);
                                    }
                                )
                            ), // sum the content of the list
                            targets => (WeightedCurrentProgress)targets
                                .Select(target => target.Value)
                                .Sum());
                    }
                ).Switch()
                    .StartWith((WeightedCurrentProgress)0);

            // todo compositetracker: child-updates müssen prozentual betrachtet werden
            this.JobStateChanges = trackerListChanges.Select(trackerList =>
                Observable.CombineLatest(
                     trackerList.Select(
                        tracker => tracker.JobStateChanges),
                     jobStates =>
                     {
                         // for more performance and maybe reliability compare current to target value instead
                         // no children? todo
                         if (!jobStates.Any())
                             return JobState.ToDo;

                         // any job in progress? => InProgress
                         if (jobStates.Contains(JobState.InProgress))
                             return JobState.InProgress;

                         // all jobs done? => Done
                         if (jobStates.All(state => state == JobState.Done))
                             return JobState.Done;

                         // all jobs todo? => todo
                         if (jobStates.All(state => state == JobState.ToDo))
                             return JobState.ToDo;

                         // remainder: (some todo, some done) => inProgress
                         return JobState.InProgress;
                     }
                )
            )
                .Switch()
            .StartWith(JobState.ToDo)
                .DistinctUntilChanged();
        }
        public override string ToString()
            => $"C {Title}";
    }

    internal class WeightedCompositeTracker : CompositeTracker, IWeightedTrackingViewer
    {
        public WeightedCompositeTracker(Name title, TrackingWeight weight) : base(title)
        {
            this.Weight = weight ?? TrackingWeight.Default;
        }

        public TrackingWeight Weight { get; }

        public override string ToString()
            => $"WC {Title} - {Weight}";
    }
}
