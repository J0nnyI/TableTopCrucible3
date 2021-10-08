using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

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
        public Name Title { get; }
        protected readonly IProgressTrackingService _TrackingService;
        protected readonly ICompositeTracker Progress;
        protected readonly IScheduler _scheduler;
        public ITrackingViewer ProgressViewer => Progress;


        protected internal JobBase(Name title, IScheduler scheduler)
        {
            Title = title;
            this._TrackingService = Locator.Current.GetService<IProgressTrackingService>();
            this.Progress = _TrackingService!.CreateCompositeTracker(title);
            _scheduler = scheduler ?? RxApp.TaskpoolScheduler;
        }

        protected internal abstract void Run();
    }

    public class ActionJob:JobBase
    {
        private readonly Action<ICompositeTracker> _action;

        protected internal ActionJob(Name title, Action<ICompositeTracker> action, IScheduler scheduler) : base(title, scheduler)
        {
            _action = action;
        }

        protected internal override void Run()
            => Observable.Start(() => _action(Progress), _scheduler);
    }
}
