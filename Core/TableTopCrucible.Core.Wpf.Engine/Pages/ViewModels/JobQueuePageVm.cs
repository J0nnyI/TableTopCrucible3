using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Jobs.ValueTypes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;

namespace TableTopCrucible.Core.Wpf.Engine.Pages.ViewModels;

[Transient]
public interface IJobQueuePage : ISidebarPage
{
}

public class JobQueuePagePageVm : IJobQueuePage
{
    public JobQueuePagePageVm(
        IJobQueue todoQueue,
        IJobQueue inProgressQueue,
        IJobQueue doneQueue)
    {
        TodoQueue = todoQueue;
        InProgressQueue = inProgressQueue;
        DoneQueue = doneQueue;

        todoQueue.JobFilter = JobFilter.FromState(JobState.ToDo);
        inProgressQueue.JobFilter = JobFilter.FromState(JobState.InProgress);
        doneQueue.JobFilter = JobFilter.FromState(JobState.Done);
    }

    [Reactive]
    public bool ToDoExpanded { get; set; }

    [Reactive]
    public bool InProgressExpanded { get; set; } = true;

    [Reactive]
    public bool DoneExpanded { get; set; }

    public IJobQueue TodoQueue { get; }
    public IJobQueue InProgressQueue { get; }
    public IJobQueue DoneQueue { get; }
    public Name Title => (Name)"Job Queue";
    public SidebarWidth Width => null;
}