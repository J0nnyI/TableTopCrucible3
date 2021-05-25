
using DynamicData;

using ReactiveUI;

using System;
using System.ComponentModel;
using System.Reactive.Linq;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Core.Jobs.Services;
using TableTopCrucible.Core.Jobs.WPF.Views;

namespace TableTopCrucible.Core.Jobs.WPF.ViewModels
{

    [Transient(typeof(JobOverviewVM))]
    public interface IJobOverview
    {

    }
    public class JobOverviewVM : IJobOverview
    {
        public BindingList<JobViewerVM> Jobs { get; } = new BindingList<JobViewerVM>();
        public JobOverviewVM(IJobService jobService)
        {
            jobService.Jobs
                .Connect()
                .Transform(job => new JobViewerVM(job))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(Jobs)
                .Subscribe();
        }
    }
}
