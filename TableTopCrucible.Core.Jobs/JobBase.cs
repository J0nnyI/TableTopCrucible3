using DynamicData;

using ReactiveUI;

using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;

namespace TableTopCrucible.Core.Jobs
{
    public abstract class JobBase<Tpre, Tres> : ReactiveObject, IJobInfo<Tres>
    {
        protected readonly IJobManagementService _jobManagementService;
        protected readonly IObservable<Tpre> _predecessor;
        public JobBase(IJobManagementService jobManagementService, IObservable<Tpre> predecessor)
        {
            _jobManagementService = jobManagementService;
            _predecessor = predecessor;
        }
        public abstract IObservable<Tres> Result { get; }
        public abstract IObservable<Unit> Done { get; }
        public abstract IObservableList<IProgression> Progression { get; }

        public IJobInfo<Tnew> Then<Tnew>(Action<IJobHandler<Tnew>> task)
            => _jobManagementService.Start(task, this.Done.Select(_ => new Unit()));

        public IJobInfo<Tnew> Then<Tnew>(Action<IJobHandler<Tnew>, Tres> task)
        => _jobManagementService.Start<Tres, Tnew>(task, this.Result);

        protected void ExecuteAction(Action<Tpre> action, Action<Exception> onError)
        {
            (_predecessor ?? Observable.Return<Tpre>(default))
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Subscribe(x =>
                {
                    action(x);
                });
        }
    }

    public interface IJobInfo
    {
        IObservableList<IProgression> Progression { get; }
        IJobInfo<Tnew> Then<Tnew>(Action<IJobHandler<Tnew>> task);
    }

    public interface IJobInfo<Tres> : IJobInfo
    {
        IJobInfo<Tnew> Then<Tnew>(Action<IJobHandler<Tnew>, Tres> task);
    }
    public interface ISingleJobInfo<Tres> : IJobInfo<Tres>
    {
        IObservable<Tres> Result { get; }
    }
    public interface IParallelJobInfo<Tres> : IJobInfo
    {
        IEnumerable<IObservable<Tres>> Results { get; }
    }
    public interface IJobHandler<T> : ISingleJobInfo<T>
    {
        IProgressionController TrackProgression(int targetValue, string title, string details);
        void Complete(T result);
        void Fail(Exception ex);
    }
}
