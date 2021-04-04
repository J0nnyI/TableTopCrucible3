using DynamicData;

using System;
using System.Collections.Generic;
using System.Text;

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
    internal class JobService:IJobService
    {
        public IObservableList<IJobViewer> Jobs => _jobs;
        private SourceList<IJobViewer> _jobs { get; } = new SourceList<IJobViewer>();

        public IJobHandler TrackJob()
        {
            var newJob = new JobManager();
            this._jobs.Add(newJob);

            return newJob;
        }
    }
}
