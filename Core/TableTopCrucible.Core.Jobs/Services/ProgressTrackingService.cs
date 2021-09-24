using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Jobs.Models;
using TableTopCrucible.Core.Jobs.ValueTypes;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Jobs.Services
{
    [Singleton(typeof(ProgressTrackingService))]
    public interface IProgressTrackingService
    {
        // creates a new tracker and adds it to the collection
        public ICompositeTrackerController CreateNewCompositeTracker(Name title);
        // creates a new tracker and adds it to the collection
        public ISourceTrackerController CreateSourceTracker(Name title, TrackingTarget target = null);
        public IObservableList<ITrackingViewer> TrackerList { get; }
    }
    internal class ProgressTrackingService: IProgressTrackingService
    {
        public ICompositeTrackerController CreateNewCompositeTracker(Name title)
        {
            var tracker = new CompositeTracker(title);
            trackerList.Add(tracker);
            return tracker;
        }
        public ISourceTrackerController CreateSourceTracker(Name title, TrackingTarget target = null)
        {
            var tracker = new SourceTracker(title, target);
            trackerList.Add(tracker);
            return tracker;
        }

        private readonly SourceList<ITrackingViewer> trackerList = new();
        public IObservableList<ITrackingViewer> TrackerList => trackerList.AsObservableList();
    }
}
