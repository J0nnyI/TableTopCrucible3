using System;
using System.Reactive.Concurrency;
using DynamicData;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Jobs.JobQueue.Models;
using TableTopCrucible.Core.Jobs.Progression.Models;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Jobs.JobQueue.Services;

[Singleton]
public interface IJobQueueService
{
    IObservableList<IJobViewer> JobQueue { get; }
}

public class JobQueueService : IJobQueueService
{
    private readonly SourceList<IJobRunner> _jobQueue = new();

    public JobQueueService()
    {
        JobQueue = _jobQueue
            .Cast(x => x as IJobViewer)
            .AsObservableList();
        throw new NotImplementedException(nameof(IJobQueueService));
    }

    public IObservableList<IJobViewer> JobQueue { get; }

    public void QueueJob(Name title, Action<ICompositeTracker> action, IScheduler scheduler)
    {
        QueueJob(new ActionJob(title, action, scheduler));
    }

    public void QueueJob(IJobRunner job)
    {
        _jobQueue.Add(job);
    }
}