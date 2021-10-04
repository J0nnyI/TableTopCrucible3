﻿using System;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Jobs.Helper;
using TableTopCrucible.Core.Jobs.Progression.Models;
using TableTopCrucible.Core.Jobs.Progression.ValueTypes;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Jobs.Progression.Services
{
    [Singleton]
    public interface IProgressTrackingService
    {
        // creates a new tracker and adds it to the collection
        ICompositeTrackerController CreateCompositeTracker(Name title);
        // creates a new tracker and adds it to the collection
        ISourceTrackerController CreateSourceTracker(Name title = null, TargetProgress target = null);
        IObservableList<ITrackingViewer> TrackerList { get; }

        IObservable<CurrentProgressPercent> TotalProgress { get; }
    }
    internal class ProgressTrackingService : IProgressTrackingService
    {
        public ProgressTrackingService()
        {
            trackerList
                .Connect()
                .DisposeMany()
                .Transform(tracker=>tracker
                    .JobStateChanges
                    .Where(state=>state == JobState.Done)
                    .Delay(SettingsHelper.NotificationDelay)
                    .Subscribe(_=>this.trackerList.Remove(tracker))
                )
                .DisposeMany()
                .Subscribe();

            TotalProgress = trackerList.Connect()
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
                    .Select(progress=> progress < (CurrentProgressPercent) 100 ? progress : (CurrentProgressPercent)0)
                )
                .Switch()
                .DistinctUntilChanged();
        }
        public ICompositeTrackerController CreateCompositeTracker(Name title = null)
        {
            var tracker = new CompositeTracker(title);
            trackerList.Add(tracker);
            return tracker;
        }
        public ISourceTrackerController CreateSourceTracker(Name title = null, TargetProgress target = null)
        {
            var tracker = new SourceTracker(title, target);
            trackerList.Add(tracker);
            return tracker;
        }

        private readonly SourceList<ITrackingViewer> trackerList = new();
        public IObservableList<ITrackingViewer> TrackerList => trackerList.AsObservableList();
        public IObservable<CurrentProgressPercent> TotalProgress { get; }
    }
}