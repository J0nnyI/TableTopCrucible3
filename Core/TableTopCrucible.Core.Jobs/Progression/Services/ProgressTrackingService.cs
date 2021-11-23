using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using DynamicData;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Jobs.Helper;
using TableTopCrucible.Core.Jobs.Progression.Models;
using TableTopCrucible.Core.Jobs.ValueTypes;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Jobs.Progression.Services
{
    [Singleton]
    public interface IProgressTrackingService
    {
        IObservableList<ITrackingViewer> TrackerList { get; }

        IObservable<CurrentProgressPercent> TotalProgress { get; }

        // creates a new tracker and adds it to the collection
        ICompositeTracker CreateCompositeTracker(Name title);

        // creates a new tracker and adds it to the collection
        ISourceTracker CreateSourceTracker(Name title = null, TargetProgress target = null);
    }

    internal class ProgressTrackingService : IProgressTrackingService, IDisposable
    {
        private CompositeDisposable _disposables = new();
        private readonly SourceList<ITrackingViewer> trackerList = new();

        public ProgressTrackingService()
        {
            TotalProgress = trackerList.Connect()
                .FilterOnObservable(tracker=>tracker.JobStateChanges.Select(state=>state == JobState.InProgress))
                .ToCollection()
                .Select(trackers => trackers
                    .Select(tracker => tracker.GetCurrentProgressInPercent())
                    .CombineLatest(
                        progresses =>
                            !progresses.Any()
                                ? CurrentProgressPercent.Min
                                : (CurrentProgressPercent)(
                                    progresses.Sum(progressPercent => progressPercent.Value)
                                    / progresses.Count
                                )
                    )
                    .Select(progress => progress < (CurrentProgressPercent)100 ? progress : (CurrentProgressPercent)0)
                )
                .Switch()
                .DistinctUntilChanged()
                .StartWith(CurrentProgressPercent.Min)
                .Replay(1)
                .ConnectUntil(_disposables);
        }

        public ICompositeTracker CreateCompositeTracker(Name title = null)
        {
            var tracker = new CompositeTracker(title);
            trackerList.Add(tracker);
            return tracker;
        }

        public ISourceTracker CreateSourceTracker(Name title = null, TargetProgress target = null)
        {
            var tracker = new SourceTracker(title, target);
            trackerList.Add(tracker);
            return tracker;
        }

        public IObservableList<ITrackingViewer> TrackerList => trackerList.AsObservableList();
        public IObservable<CurrentProgressPercent> TotalProgress { get; }
        public void Dispose()
           => _disposables.Dispose();
    }
}