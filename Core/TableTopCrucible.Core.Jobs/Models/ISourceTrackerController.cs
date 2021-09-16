using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableTopCrucible.Core.Jobs.ValueTypes;

namespace TableTopCrucible.Core.Jobs.Models
{
    // controls a single source tracker
    // dispose completes the tracking, regardless of the current progress
    public interface ISourceTrackerController:IDisposable, ITrackingViewer
    {
        public void SetTarget(TargetProgress target);
        public void Set(CurrentProgress target);
        public void Increment();
        public void Complete();
    }
}
