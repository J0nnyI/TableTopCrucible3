using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableTopCrucible.Core.Jobs.ValueTypes;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Jobs.Models
{
    // tracks a composite progress and returns the weighted total progress
    internal class CompositeTracker:ICompositeTrackerController, ITrackingViewer
    {
        public IObservable<CurrentProgress> CurrentProgressChanges { get; }
        public IObservable<TargetProgress> TargetProgressChanges { get; }
        public Name Title { get; }
        public ITrackingViewer AddTracker(ITrackingViewer tracker, TrackingWeight weight)
        {
            throw new NotImplementedException();
        }

        public CompositeTracker(Name title)
        {
            this.Title = title;
        }
    }
}
