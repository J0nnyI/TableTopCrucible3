using DynamicData;

using Microsoft.Extensions.Logging;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Core.Jobs.Managers;

namespace TableTopCrucible.Core.Jobs.Services
{
    [Singleton(typeof(JobService))]
    public interface IJobService
    {
        IJobHandler TrackJob();
        IObservableList<IJobViewer> Jobs { get; }
    }
    internal class JobService : IJobService
    {
        private readonly ILoggerFactory _loggerFactory;

        public IObservableList<IJobViewer> Jobs => _jobs;
        private SourceList<IJobViewer> _jobs { get; } = new SourceList<IJobViewer>();
        public JobService(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }
        public IJobHandler TrackJob()
        {
            var newJob = new JobManager(_loggerFactory);
            this._jobs.Add(newJob);

            return newJob;
        }
    }
}
