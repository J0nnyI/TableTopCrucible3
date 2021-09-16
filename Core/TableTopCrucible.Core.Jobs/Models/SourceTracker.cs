using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableTopCrucible.Core.Jobs.ValueTypes;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Jobs.Models
{
    // tracks a single progress
    internal class SourceTracker:ISourceTrackerController, ITrackingViewer
    {
        public void Dispose()
        {
        }

        public IObservable<CurrentProgress> CurrentProgressChanges { get; }
        public IObservable<TargetProgress> TargetProgressChanges { get; }
        public Name Title { get; }
        public void SetTarget(TargetProgress target)
        {
            throw new NotImplementedException();
        }

        public void Set(CurrentProgress target)
        {
            throw new NotImplementedException();
        }

        public void Increment()
        {
            throw new NotImplementedException();
        }

        public void Complete()
        {
            throw new NotImplementedException();
        }
    }
}
