using System;
using TableTopCrucible.Core.Jobs.Progression.ValueTypes;

namespace TableTopCrucible.Core.Jobs.Progression.Models
{
    // controls a single source tracker
    // dispose completes the tracking, regardless of the current progress
    public interface ISourceTracker : IDisposable, ITrackingViewer
    {
        public void SetTarget(TargetProgress trackingTarget);

        // default = 1
        public void Increment(ProgressIncrement increment = null);

        // should be called when the task is done, dispose should be called when the tracker will be no longer needed (reading included)
        public void OnCompleted();
    }
}