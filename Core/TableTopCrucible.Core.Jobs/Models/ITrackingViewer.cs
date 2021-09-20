using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableTopCrucible.Core.Jobs.ValueTypes;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Jobs.Models
{
    // interface for getting the progress of a source tracker or tracker collection
    public interface ITrackingViewer
    {
        IObservable<CurrentProgress> CurrentProgressChanges { get; }
        IObservable<TrackingTarget> TargetProgressChanges { get; }
        Name Title { get; }
    }
    public interface IWeightedTrackingViewer:ITrackingViewer
    {
        TrackingWeight Weight { get; }
    }
}
