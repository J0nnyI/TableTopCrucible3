using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Jobs.Progression.Services;
using TableTopCrucible.Core.Jobs.Progression.ValueTypes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels
{
    [Singleton(typeof(JobQueueVm))]
    public interface IJobQueue : ISidebarPage
    {}
    public class JobQueueVm:ReactiveObject, IActivatableViewModel, IJobQueue
    {
        private readonly IProgressTrackingService _progressTrackingService;
        public ViewModelActivator Activator { get; } = new();
        public Name Title => (Name) "Job Queue";
        public SidebarWidth Width => null;

        //public ObservableCollectionEx

        public JobQueueVm(IProgressTrackingService progressTrackingService)
        {
            _progressTrackingService = progressTrackingService;

            //this.WhenActivated(()=>new []
            //{
            //    _progressTrackingService
            //        .TrackerList
            //        .Connect()
            //        .Filter(job=>job.JobStateChanges == JobState.Done)
            //        .BindTo()
            //});
        }
    }
}
