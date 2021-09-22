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
            CurrentProgressChanges.Select(prog => (CurrentProgress)prog);
        public IObservable<WeightedCurrentProgress> CurrentProgressChanges { get; }

        IObservable<TrackingTarget> ITrackingViewer.TargetProgressChanges =>
            TargetProgressChanges.Select(target => (TrackingTarget)target);
        public IObservable<WeightedTrackingTarget> TargetProgressChanges { get; }

        public Name Title { get; }
        public IObservable<JobState> JobStateChanges { get; }

        public ICompositeTrackerController AddComposite(Name name, TrackingWeight weight = null)
        {
            var tracker = new WeightedCompositeTracker(name, weight);
            _trackerStack.OnNext(tracker);
            return tracker;
        }

        public ISourceTrackerController AddSingle(Name name, TrackingTarget trackingTarget, TrackingWeight weight = null)
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
                    new List<IWeightedTrackingViewer>(),
                    (list, tracker) =>
                    {
                        list.Add(tracker);
                        return list;
                    });

            CurrentProgressChanges = _trackerStack.SelectMany(src =>
                    src.CurrentProgressChanges.Select(current => current * src.Weight))
                .Scan((WeightedCurrentProgress)0, (acc, cur) => acc + cur);

            TargetProgressChanges = trackerListChanges.Select(trackerList =>
                Observable.CombineLatest(// get the weight of all trackers parallel as list
                    trackerList.Select(tracker =>
                        tracker.TargetProgressChanges.Select(target =>
                            target * tracker.Weight)
                    ),// sum the content of the list
                    targets => (WeightedTrackingTarget)targets
                        .Select(target => target.Value)
                        .Sum())
                ).Switch();// and flatten the observables so that it is updated when there is a new tracker and when a target changes

            trackerListChanges.Select(trackerList =>
                Observable.CombineLatest(
                     trackerList.Select(
                        tracker => tracker.JobStateChanges),
                     jobStates =>
                     {
                         // any job in progress? => InProgress
                         if(jobStates.Contains(JobState.InProgress))
                            return JobState.InProgress;

                         // all jobs done? => Done
                         if(jobStates.All(state=>state==JobState.Done))
                             return JobState.Done;

                         // all jobs todo? => todo
                         if(jobStates.All(state=>state==JobState.ToDo))
                            return JobState.ToDo;

                         //remainder: (some todo, some done) => inProgress
                         return JobState.InProgress;
                     }
                )
            );
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
