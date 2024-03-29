﻿using System;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Aggregation;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Jobs.Progression.Models;
using TableTopCrucible.Core.Jobs.Progression.Services;
using TableTopCrucible.Core.Jobs.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels
{
    [Transient]
    public interface IJobQueue
    {
        JobFilter JobFilter { get; set; }
        public IObservable<JobCount> JobCountChanges { get; }
    }

    public class JobQueueVm : ReactiveObject, IActivatableViewModel, IJobQueue
    {
        private readonly IObservable<JobFilter> _filterChanges;
        private readonly IProgressTrackingService _progressTrackingService;

        public ObservableCollectionExtended<IJobViewerCard> Cards = new();

        public JobQueueVm(IProgressTrackingService progressTrackingService)
        {
            _progressTrackingService = progressTrackingService;

            _filterChanges =
                this.WhenAnyValue(vm => vm.JobFilter)
                    .DistinctUntilChanged()
                    .Select(filter => filter ?? JobFilter.All)
                    .Replay(1)
                    .RefCount();

            JobCountChanges =
                _progressTrackingService
                    .TrackerList
                    .Connect()
                    .FilterOnObservable(JobToFilter, null, RxApp.TaskpoolScheduler)
                    .Count()
                    .Select(JobCount.From)
                    .Replay(1)
                    .RefCount();

            this.WhenActivated(() =>
            {
                Cards.Clear();
                return new[]
                {
                    _progressTrackingService
                        .TrackerList
                        .Connect()
                        .ObserveOn(RxApp.TaskpoolScheduler)
                        .Transform(getCardForJob)
                        .FilterOnObservable(JobToFilter, null, RxApp.TaskpoolScheduler)
                        .DisposeMany()
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .SubscribeOn(RxApp.MainThreadScheduler)
                        .Bind(Cards)
                        .Subscribe()
                };
            });
        }

        public ViewModelActivator Activator { get; } = new();
        public IObservable<JobCount> JobCountChanges { get; }

        /**
         * default behavior: show all (Observable.Return(true))
         * this will also be used if the value is null
         */
        [Reactive]
        public JobFilter JobFilter { get; set; } = JobFilter.All;

        internal IObservable<bool> JobToFilter(IJobViewerCard card)
        {
            return card.WhenAnyValue(card => card.Viewer)
                .Select(JobToFilter)
                .Switch();
        }

        internal IObservable<bool> JobToFilter(ITrackingViewer job)
        {
            return _filterChanges
                .Select(filter =>
                    filter.Value(job)
                        .Select(res => new { res, job, filter.Description })
                )
                .Switch()
                .Select(x => x.res);
        }

        public IJobViewerCard getCardForJob(ITrackingViewer viewer)
        {
            var card = Locator.Current.GetService<IJobViewerCard>();
            card.Viewer = viewer;
            return card;
        }
    }
}