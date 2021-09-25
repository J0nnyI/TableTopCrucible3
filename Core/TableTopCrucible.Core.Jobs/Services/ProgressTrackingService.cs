using DynamicData;

using System;
using System.Linq;
using System.Reactive.Linq;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Jobs.Helper;
using TableTopCrucible.Core.Jobs.Models;
using TableTopCrucible.Core.Jobs.ValueTypes;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Jobs.Services
{
    [Singleton(typeof(ProgressTrackingService))]
    public interface IProgressTrackingService
    {
        // creates a new tracker and adds it to the collection
        public ICompositeTrackerController CreateCompositeTracker(Name title = null);
        // creates a new tracker and adds it to the collection
        public ISourceTrackerController CreateSourceTracker(Name title = null, TrackingTarget target = null);
        public IObservableList<ITrackingViewer> TrackerList { get; }

        public IObservable<CurrentProgressPercent> CurrentProgress { get; }
    }
    internal class ProgressTrackingService : IProgressTrackingService
    {
        public ProgressTrackingService()
        {
            CurrentProgress = trackerList.Connect()
                .ToCollection()
                .Select(trackers => trackers
                    .Select(tracker => tracker.GetCurrentProgressInPercent())
                    .CombineLatest(
                    progresses =>
                        !progresses.Any() ? CurrentProgressPercent.Min :
                        (CurrentProgressPercent)(
                            progresses.Sum(progressPercent => progressPercent.Value)
                            / progresses.Count
                        )
                    )
                )
                .Switch();
        }
        public ICompositeTrackerController CreateCompositeTracker(Name title = null)
        {
            var tracker = new CompositeTracker(title);
            trackerList.Add(tracker);
            return tracker;
        }
        public ISourceTrackerController CreateSourceTracker(Name title = null, TrackingTarget target = null)
        {
            var tracker = new SourceTracker(title, target);
            trackerList.Add(tracker);
            return tracker;
        }

        private readonly SourceList<ITrackingViewer> trackerList = new();
        public IObservableList<ITrackingViewer> TrackerList => trackerList.AsObservableList();
        public IObservable<CurrentProgressPercent> CurrentProgress { get; }
    }
}
