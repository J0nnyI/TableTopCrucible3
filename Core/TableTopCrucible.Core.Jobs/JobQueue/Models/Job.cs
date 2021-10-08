using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using ReactiveUI;
using Splat;
using TableTopCrucible.Core.Jobs.Progression.Models;
using TableTopCrucible.Core.Jobs.Progression.Services;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Jobs.JobQueue.Models
{
    public interface IJobViewer
    {
        Name Title { get; }
        ITrackingViewer ProgressViewer { get; }
    }

    public interface IJobRunner : IJobViewer
    {
    }

    public abstract class JobBase : ReactiveObject, IJobViewer, IJobRunner
    {
        protected readonly IScheduler _scheduler;
        protected readonly IProgressTrackingService _TrackingService;
        protected readonly ICompositeTracker Progress;


        protected internal JobBase(Name title, IScheduler scheduler)
        {
            Title = title;
            _TrackingService = Locator.Current.GetService<IProgressTrackingService>();
            Progress = _TrackingService!.CreateCompositeTracker(title);
            _scheduler = scheduler ?? RxApp.TaskpoolScheduler;
        }

        public Name Title { get; }
        public ITrackingViewer ProgressViewer => Progress;

        protected internal abstract void Run();
    }

    public class ActionJob : JobBase
    {
        private readonly Action<ICompositeTracker> _action;

        protected internal ActionJob(Name title, Action<ICompositeTracker> action, IScheduler scheduler) : base(title,
            scheduler)
        {
            _action = action;
        }

        protected internal override void Run()
        {
            Observable.Start(() => _action(Progress), _scheduler);
        }
    }
}