using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Jobs.ValueTypes;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Jobs.Progression.Models
{
    // tracks a composite progress and returns the weighted total progress
    internal class CompositeTracker : ICompositeTracker, ITrackingViewer, IDisposable
    {
        private CompositeDisposable _disposables = new();
        private readonly IObservable<Unit> _onDestroy;
        public static readonly WeightedTargetProgress Target = (WeightedTargetProgress)100;

        private readonly SourceList<IWeightedTrackingViewer> _trackerStack = new();

        public CompositeTracker(Name title)
        {
            Title = title;
            _onDestroy = _disposables.AsSubject();


            var trackerListChanges = _trackerStack
                .Connect()
                .ToCollection()
                .Publish()
                .RefCount();

            CurrentProgressChanges = trackerListChanges
                .Select(trackerList =>
                {
                    var totalTrackerWeight = (JobWeight)trackerList
                        .Sum(tracker => tracker.Weight.Value);

                    return // get the weight of all trackers parallel as list
                        trackerList.Select(tracker =>
                            tracker.CurrentProgressChanges.CombineLatest(tracker.TargetProgressChanges,
                                tracker.JobStateChanges,
                                (current, target, state) =>
                                {
                                    // target value relative to the others
                                    var targetFraction = tracker.Weight.Value / totalTrackerWeight.Value;
                                    var targetPercent =
                                        Target.Value *
                                        targetFraction; // how much of the composite progress is from this tracker?

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
                                .Sum()
                        );
                })
                .Switch()
                .StartWith((WeightedCurrentProgress)0)
                .DistinctUntilChanged()
                .TakeUntil(_onDestroy)
                .Replay()
                .ConnectUntil(_disposables);

            // todo compositeTracker: child-updates have to be in percent
            JobStateChanges = trackerListChanges.Select(trackerList =>
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
                .DistinctUntilChanged()
                .Replay(1)
                .ConnectUntil(_disposables);
        }

        public IObservable<WeightedCurrentProgress> CurrentProgressChanges { get; }

        public IObservable<WeightedTargetProgress> TargetProgressChanges { get; } = Observable.Return(Target);

        IObservable<CurrentProgress> ITrackingViewer.CurrentProgressChanges =>
            CurrentProgressChanges.Select(x => (CurrentProgress)x);

        IObservable<TargetProgress> ITrackingViewer.TargetProgressChanges =>
            TargetProgressChanges.Select(x => (TargetProgress)x);

        public Name Title { get; }
        public IObservable<JobState> JobStateChanges { get; }

        public ICompositeTracker AddComposite(Name name = null, JobWeight weight = null)
        {
            var tracker = new WeightedCompositeTracker(name, weight);
            _trackerStack.Add(tracker);
            return tracker;
        }

        public ISourceTracker AddSingle(Name name = null, TargetProgress targetProgress = null, JobWeight weight = null)
        {
            var tracker = new WeightedSourceTracker(name, targetProgress, weight);
            _trackerStack.Add(tracker);
            return tracker;
        }

        public override string ToString() => $"C {Title}";

        public void Dispose()
            => _disposables.Dispose();
    }

    internal class WeightedCompositeTracker : CompositeTracker, IWeightedTrackingViewer
    {
        public WeightedCompositeTracker(Name title, JobWeight weight) : base(title)
        {
            Weight = weight ?? JobWeight.Default;
        }

        public JobWeight Weight { get; }

        public override string ToString() => $"WC {Title} - {Weight}";
    }
}