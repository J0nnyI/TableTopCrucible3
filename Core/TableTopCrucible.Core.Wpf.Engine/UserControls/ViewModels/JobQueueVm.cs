using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using Splat;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Jobs.Progression.Models;
using TableTopCrucible.Core.Jobs.Progression.Services;
using TableTopCrucible.Core.Jobs.Progression.ValueTypes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels
{
    [Singleton]
    public interface IJobQueue
    {
        Func<ITrackingViewer, IObservable<bool>> JobFilter { get; set; }
    }
    public class JobQueueVm:ReactiveObject, IActivatableViewModel, IJobQueue
    {
        private readonly IProgressTrackingService _progressTrackingService;
        public ViewModelActivator Activator { get; } = new();
        public Name Title => (Name) "Job Queue";
        public SidebarWidth Width => null;
        public ITrackingViewer Viewer { get; }

        public ObservableCollectionExtended<IJobViewerCard> Cards = new();

        public JobQueueVm(IProgressTrackingService progressTrackingService)
        {
            _progressTrackingService = progressTrackingService;

            this.WhenActivated(() => new[]
            {
                _progressTrackingService
                    .TrackerList
                    .Connect()
                    .FilterOnObservable(job=>
                        job.JobStateChanges.Select(state=>state != JobState.Done).StartWith(false))
                    .Transform(getCardForJob)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(Cards)
                    .Subscribe()
            });
        }

        private IJobViewerCard getCardForJob(ITrackingViewer viewer)
        {
            var card = Locator.Current.GetService<IJobViewerCard>();
            card.Viewer = viewer;
            return card;
        }

        public Func<ITrackingViewer, IObservable<bool>> JobFilter { get; set; }
    }
}
