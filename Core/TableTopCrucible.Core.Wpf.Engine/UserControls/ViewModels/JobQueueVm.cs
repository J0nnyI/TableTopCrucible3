using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

using DynamicData;
using DynamicData.Binding;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

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
    [Transient]
    public interface IJobQueue
    {
        JobFilter JobFilter { get; set; }
    }

    public class JobQueueVm : ReactiveObject, IActivatableViewModel, IJobQueue
    {
        private readonly IProgressTrackingService _progressTrackingService;
        public ViewModelActivator Activator { get; } = new();

        private ReadOnlyObservableCollection<IJobViewerCard> _cards;
        public ReadOnlyObservableCollection<IJobViewerCard> Cards => _cards;

        public JobQueueVm(IProgressTrackingService progressTrackingService)
        {
            _progressTrackingService = progressTrackingService;

            var filterChanges =
                this.WhenAnyValue(vm => vm.JobFilter)
                    .Select(filter => filter ?? JobFilter.Default);


            this.WhenActivated(() => new[]
            {
                _progressTrackingService
                    .TrackerList
                    .Connect()
                    .FilterOnObservable(job =>
                        filterChanges.Select(filter => 
                                filter.Value(job)
                                    .Select(res=>new{res, filter.Description})
                            )
                            .Switch()
                            .Select(x=>x.res)
                            .Catch((Exception ex)=>Observable.Return(true)))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .SubscribeOn(RxApp.MainThreadScheduler)
                    .Transform(getCardForJob)
                    .Bind(out _cards)
                    .DisposeMany()
                    .Subscribe()
            });
        }

        private IJobViewerCard getCardForJob(ITrackingViewer viewer)
        {
            var card = Locator.Current.GetService<IJobViewerCard>();
            card.Viewer = viewer;
            return card;
        }

        /**
         * default behavior: show all (Observable.Return(true))
         * this will also be used if the value is null
         */
        [Reactive] public JobFilter JobFilter { get; set; } = JobFilter.Default;


    }
}