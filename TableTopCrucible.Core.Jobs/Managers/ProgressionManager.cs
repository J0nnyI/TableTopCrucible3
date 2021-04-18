using DynamicData;

using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;

using TableTopCrucible.Core.BaseUtils;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Jobs.Enums;
using TableTopCrucible.Core.Jobs.Models;

namespace TableTopCrucible.Core.Jobs.Managers
{
    public interface IProgressionViewer : INotifyPropertyChanged
    {
        JobState State { get; }
        int Target { get; }
        int Current { get; }
        string Title { get; }
        string Details { get; }
        int Weight { get; }
    }


    public interface IProgressionHandler : IProgressionViewer
    {
        new JobState State { get; set; }
        new int Target { get; set; }
        new int Current { get; set; }
        new string Title { get; set; }
        new string Details { get; set; }
        /// <summary>
        ///  the Weight (duration) of this progress relative to the others in the <see cref="IJobViewer"/>
        /// </summary>
        new int Weight { get; set; }
    }


    class DerivedProgressionManager : DisposableReactiveObjectBase, IProgressionViewer
    {
        private readonly ILogger<DerivedProgressionManager> logger;

        [Reactive]
        public JobState State { get; private set; }
        [Reactive]
        public int Target { get; private set; }
        [Reactive]
        public int Current { get; private set; }
        [Reactive]
        public string Title { get; private set; }
        [Reactive]
        public string Details { get; private set; }
        /// <summary>
        /// Not supported in derived progressuib managers
        /// </summary>
        [Reactive]
        public int Weight { get; private set; } = 1;
        public int Resolution => 1000;


        public DerivedProgressionManager(IObservableList<IProgressionViewer> subProgs, ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<DerivedProgressionManager>();
            subProgs
                .Connect()
                .ToCollection()
                .Select(progs => progs.Select(prog => prog.WhenAnyValue(
                    vm => vm.Title,
                    vm => vm.State,
                    vm => vm.Target,
                    vm => vm.Current,
                    vm => vm.Details,
                    vm => vm.Weight,
                    ProgressionSnapshot.Factory)))
                .Select(Observable.CombineLatest)
                .Switch()
                .Subscribe(progs =>
                {
                    this.State = JobStateHelper.MergeStates(progs.Select(prog => prog.State));
                    Target = progs.Sum(prog => prog.Weight) * Resolution;

                    Current = (int)progs.Sum(prog => (decimal)prog.Current / (decimal)prog.Target * Weight * Resolution);

                    var newDetails = progs.FirstOrDefault(x => x.State == JobState.InProgress).Details;
                    if (State.IsIn(JobState.ToDo, JobState.Done))
                        newDetails = string.Empty;
                    if (State == JobState.Failed)
                    {
                        newDetails = $"Job Failed.{Environment.NewLine + progs.FirstOrDefault(x => x.State == JobState.Failed).Details}";
                        logger.LogError("job failed: {0}", newDetails);
                    }
                    Details = newDetails;
                    logger.LogTrace("extra info:" + subProgs.Count + Environment.NewLine +
                        string.Join(Environment.NewLine, progs.Select(prog => $" {prog.Title} | {prog.State} | {prog.Current}/{prog.Target} : {prog.Details}"))
                        );
                    logger.LogTrace("updating progress - {0}subs: {1}/{2} ({3}): {4}", progs.Count(), Current, Target, State, Details);
                });
        }
    }


    class ProgressionManager : DisposableReactiveObjectBase, IProgressionHandler
    {
        public ProgressionManager(string title, int target, int weight)
        {
            Title = title;
            Target = target;
            Weight = weight;
        }

        [Reactive]
        public JobState State { get; set; } = JobState.ToDo;
        [Reactive]
        public int Target { get; set; }
        [Reactive]
        public int Current { get; set; }
        [Reactive]
        public string Title { get; set; }
        [Reactive]
        public string Details { get; set; }
        [Reactive]
        public int Weight { get; set; }
    }
}
