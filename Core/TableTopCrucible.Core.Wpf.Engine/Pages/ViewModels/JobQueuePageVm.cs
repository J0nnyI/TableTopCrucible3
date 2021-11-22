using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Jobs.Progression.ValueTypes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;

namespace TableTopCrucible.Core.Wpf.Engine.Pages.ViewModels
{
    [Transient]
    public interface IJobQueuePage: ISidebarPage
    {

    }
    public class JobQueuePagePageVm:IJobQueuePage
    {
        public IJobQueue AllQueue { get; }
        public IJobQueue TodoQueue { get; }
        public IJobQueue InProgressQueue { get; }
        public IJobQueue DoneQueue { get; }
        public Name Title => (Name) "Job Queue";
        public SidebarWidth Width => null;

        public JobQueuePagePageVm(
            IJobQueue allQueue, 
            IJobQueue todoQueue,
            IJobQueue inProgressQueue,
            IJobQueue doneQueue)
        {
            AllQueue = allQueue;
            TodoQueue = todoQueue;
            InProgressQueue = inProgressQueue;
            DoneQueue = doneQueue;

            allQueue.JobFilter = JobFilter.All;
            todoQueue.JobFilter = JobFilter.FromState(JobState.ToDo);
            inProgressQueue.JobFilter = JobFilter.FromState(JobState.InProgress);
            doneQueue.JobFilter = JobFilter.FromState(JobState.Done);
        }
    }
}
