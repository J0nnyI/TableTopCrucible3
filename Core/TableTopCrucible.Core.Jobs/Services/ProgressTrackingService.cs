using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Jobs.Models;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Jobs.Services
{
    [Singleton(typeof(ProgressTrackingService))]
    public interface IProgressTrackingService
    {
        // creates a new tracker and adds it to the collection
        public ICompositeTrackerController CreateNewComposite(Name title);
        // creates a new tracker and adds it to the collection
        public ISourceTrackerController CreateNewSourceTracker(Name title);
    }
    internal class ProgressTrackingService: IProgressTrackingService
    {

    }
}
