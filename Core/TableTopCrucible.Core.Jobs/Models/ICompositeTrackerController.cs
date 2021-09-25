using TableTopCrucible.Core.Jobs.ValueTypes;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Jobs.Models
{
    // interface for managing a tracking collection
    public interface ICompositeTrackerController : ITrackingViewer
    {
        ICompositeTrackerController AddComposite(Name name = null, TrackingWeight weight = null);
        ISourceTrackerController AddSingle(Name name = null, TrackingTarget trackingTarget = null, TrackingWeight weight = null);
    }
}
