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
        public ICompositeTrackerController CreateNewCompositeTracker(Name title);
        public ICompositeTrackerController CreateNewCompositeTracker(string title);
        // creates a new tracker and adds it to the collection
        public ISourceTrackerController CreateNewSourceTracker(Name title);
        public ISourceTrackerController CreateNewSourceTracker(string title);
    }
    internal class ProgressTrackingService: IProgressTrackingService
    {
        public ICompositeTrackerController CreateNewCompositeTracker(Name title)
        {
            throw new NotImplementedException();
        }

        public ICompositeTrackerController CreateNewCompositeTracker(string title)
            => CreateNewCompositeTracker(Name.From(title));

        public ISourceTrackerController CreateNewSourceTracker(Name title)
        {
            throw new NotImplementedException();
        }

        public ISourceTrackerController CreateNewSourceTracker(string title)
            => CreateNewSourceTracker(Name.From(title));
    }
}
