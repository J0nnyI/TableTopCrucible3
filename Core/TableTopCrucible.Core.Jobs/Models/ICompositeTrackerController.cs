using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using TableTopCrucible.Core.Jobs.ValueTypes;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Jobs.Models
{
    // interface for managing a tracking collection
    public interface ICompositeTrackerController: ITrackingViewer
    {
        ICompositeTrackerController AddComposite(Name name, TrackingWeight weight = null);
        ISourceTrackerController AddSingle(Name name, TrackingTarget trackingTarget = null, TrackingWeight weight = null);
        IObservable<IWeightedTrackingViewer> TrackerStack { get; }
    }
}
