using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;

using DynamicData;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Jobs.JobQueue.Models;
using TableTopCrucible.Core.Jobs.Progression.Models;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Jobs.JobQueue.Services
{
    [Singleton]
    public interface IJobQueueService
    {
        IObservableList<IJobViewer> JobQueue { get; }
    }
    public class JobQueueService : IJobQueueService
    {
        private readonly SourceList<IJobRunner> _jobQueue = new();
        public IObservableList<IJobViewer> JobQueue { get; }

        public JobQueueService()
        {
            JobQueue = _jobQueue
                .Cast(x => x as IJobViewer)
                .AsObservableList();
            throw new NotImplementedException(nameof(IJobQueueService));
        }

        public void QueueJob(Name title, Action<ICompositeTracker> action, IScheduler scheduler)
            => QueueJob(new ActionJob(title, action, scheduler));
        public void QueueJob(IJobRunner job)
            => _jobQueue.Add(job);
    }
}
