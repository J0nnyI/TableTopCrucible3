using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using TableTopCrucible.Core.Jobs.ValueTypes;

namespace TableTopCrucible.Core.Jobs.Models
{
    // interface for managing a tracking collection
    public interface ICompositeTrackerController: ITrackingViewer
    {
        ITrackingViewer AddTracker(ITrackingViewer tracker, TrackingWeight weight);
        IObservableList<ITrackingViewer> Children { get; }
    }
}
