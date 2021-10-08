using TableTopCrucible.Core.Jobs.Progression.ValueTypes;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Jobs.Progression.Models
{
    // interface for managing a tracking collection
    public interface ICompositeTracker : ITrackingViewer
    {
        ICompositeTracker AddComposite(Name name = null, JobWeight weight = null);
        ISourceTracker AddSingle(Name name = null, TargetProgress targetProgress = null, JobWeight weight = null);
    }
}
