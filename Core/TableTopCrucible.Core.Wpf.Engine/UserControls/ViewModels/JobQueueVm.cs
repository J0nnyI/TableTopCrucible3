using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using TableTopCrucible.Core.Jobs.JobQueue.Models;
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

        public ObservableCollectionExtended<IJobViewerCard> Cards = new();

        private readonly IObservable<JobFilter> _filterChanges;

        internal IObservable<bool> JobToFilter(IJobViewerCard card)
        {
            return card.WhenAnyValue(card => card.Viewer)
                .Select(job =>
                    _filterChanges.Select(filter =>
                            filter.Value(job)
                                .Select(res => new {res, job, filter.Description})
                        )
                        .Switch()
                        .Select(x => x.res)
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .SubscribeOn(RxApp.MainThreadScheduler)
                )
                .Switch();

        }

        public JobQueueVm(IProgressTrackingService progressTrackingService)
        {
            _progressTrackingService = progressTrackingService;

            _filterChanges =
                this.WhenAnyValue(vm => vm.JobFilter)
                    .DistinctUntilChanged()
                    .Select(filter => filter ?? JobFilter.All)
                    .Replay(1)
                    .RefCount();

            this.WhenActivated(() =>
            {
                Cards.Clear();
                return new []
                {
                    _progressTrackingService
                        .TrackerList
                        .Connect()
                        .Transform(getCardForJob)
                        .Catch((Exception ex) =>
                        {
                            Debugger.Break();
                            return Observable.Never<IChangeSet<IJobViewerCard>>();
                        })
                        .FilterOnObservable(JobToFilter)
                        .Catch((Exception ex) =>
                        {
                            Debugger.Break();
                            return Observable.Never<IChangeSet<IJobViewerCard>>();
                        })
                        .DisposeMany()
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .SubscribeOn(RxApp.MainThreadScheduler)
                        .Bind(Cards)
                        .Subscribe(x =>
                        {

                        }, ex =>
                        {
                            Debugger.Break();
                        }, () =>
                        {
                        })
                };
            });

        }

        public IJobViewerCard getCardForJob(ITrackingViewer viewer)
        {
            var card = Locator.Current.GetService<IJobViewerCard>();
            card.Viewer = viewer;
            return card;
        }

        /**
         * default behavior: show all (Observable.Return(true))
         * this will also be used if the value is null
         */
        [Reactive] public JobFilter JobFilter { get; set; } = JobFilter.All;


    }
}