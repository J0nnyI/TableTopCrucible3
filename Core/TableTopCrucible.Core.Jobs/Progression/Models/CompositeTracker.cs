﻿using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using TableTopCrucible.Core.Jobs.Progression.ValueTypes;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Jobs.Progression.Models
{
    // tracks a composite progress and returns the weighted total progress
    internal class CompositeTracker : ICompositeTrackerController, ITrackingViewer
    {
        public static readonly WeightedTargetProgress Target = (WeightedTargetProgress)100;

        IObservable<CurrentProgress> ITrackingViewer.CurrentProgressChanges =>
            CurrentProgressChanges.Select(prog => (CurrentProgress)prog);
        public IObservable<WeightedCurrentProgress> CurrentProgressChanges { get; }

        IObservable<TargetProgress> ITrackingViewer.TargetProgressChanges =>
            TargetProgressChanges.Select(target => (TargetProgress)target);

        public IObservable<WeightedTargetProgress> TargetProgressChanges { get; } = Observable.Return(Target);

        public Name Title { get; }
        public IObservable<JobState> JobStateChanges { get; }

        public ICompositeTrackerController AddComposite(Name name = null, JobWeight weight = null)
        {
            var tracker = new WeightedCompositeTracker(name, weight);
            _trackerStack.OnNext(tracker);
            return tracker;
        }

        public ISourceTrackerController AddSingle(Name name = null, TargetProgress targetProgress = null, JobWeight weight = null)
        {
            var tracker = new WeightedSourceTracker(name, targetProgress, weight);
            _trackerStack.OnNext(tracker);
            return tracker;
        }
        // each pushed item represents one tracker
        private readonly ReplaySubject<IWeightedTrackingViewer> _trackerStack = new();
        public IObservable<IWeightedTrackingViewer> TrackerStack => _trackerStack;

        private IObservable<IWeightedTrackingViewer[]> trackerListChanges { get; }

        public CompositeTracker(Name title)
        {
            this.Title = title;


            trackerListChanges = _trackerStack
                .Scan(
                    Array.Empty<IWeightedTrackingViewer>(),
                    (list, tracker) =>
                        list.Append(tracker).ToArray()
                    );

            CurrentProgressChanges = _trackerStack.SelectMany(src =>
                    src.CurrentProgressChanges.Select(current => current * src.Weight))
                .Scan((WeightedCurrentProgress)0, (acc, cur) => acc + cur);



            CurrentProgressChanges = trackerListChanges.Select(trackerList =>
                    {
                        var totalTrackerWeight = (JobWeight)trackerList.Sum(tracker => tracker.Weight.Value);

                        return // get the weight of all trackers parallel as list
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
                            ).CombineLatest( // sum the content of the list
                            targets => (WeightedCurrentProgress)targets
                                .Select(target => target.Value)
                                .Sum());
                    }
                ).Switch()
                    .StartWith((WeightedCurrentProgress)0);

            // todo compositetracker: child-updates müssen prozentual betrachtet werden
            this.JobStateChanges = trackerListChanges.Select(trackerList =>
                trackerList.Select(tracker => tracker.JobStateChanges)
                    .CombineLatest(jobStates =>
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
        public WeightedCompositeTracker(Name title, JobWeight weight) : base(title)
        {
            this.Weight = weight ?? JobWeight.Default;
        }

        public JobWeight Weight { get; }

        public override string ToString()
            => $"WC {Title} - {Weight}";
    }
}
