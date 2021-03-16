using DynamicData;
using DynamicData.Kernel;

using ReactiveUI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using TableTopCrucible.Core.Helper;

namespace TableTopCrucible.Core.Jobs
{
    public enum JobState
    {
        ToDo,
        InProgress,
        Done,
        Failed
    }

    public class Job<Tres> : Job<Unit, Tres>
    {
        public Job(
            Action<IJobHandler<Tres>, Unit> action,
            IJobManagementService jobManagementService,
            IObservable<Unit> predecessor) : base(action, jobManagementService, predecessor)
        {
        }
    }
    public class Job<Tpre, Tres> : JobBase<Tpre, Tres>, IJobInfo<Tres>, IJobHandler<Tres>
    {
        private readonly BehaviorSubject<Optional<Tres>> _result = new BehaviorSubject<Optional<Tres>>(Optional.None<Tres>());
        private readonly SourceList<IProgression> _progression = new SourceList<IProgression>();

        public override IObservable<Tres> Result => _result.Where(x => x.HasValue).Select(x => x.Value);
        public override IObservable<Unit> Done => Result.Select(_ => new Unit());
        public override IObservableList<IProgression> Progression => _progression;
        public Job(
            Action<IJobHandler<Tres>, Tpre> action,
            IJobManagementService jobManagementService,
            IObservable<Tpre> predecessor) : base(jobManagementService, predecessor)
        {
            ExecuteAction(pre => action(this, pre), this.Fail);
        }



        public void Complete(Tres result)
        {
            _result.OnNext(result);
            _result.OnCompleted();
        }

        public void Fail(Exception ex)
        {
            _result.OnError(ex);
            _result.OnCompleted();
        }

        public IProgressionController TrackProgression(int targetValue, string title, string details)
        {
            var prog = new Progression(targetValue, title, details);
            prog.Destroy
                .Take(1)
                .Delay(new TimeSpan(0, 0, 10))
                .Subscribe(_ =>
                    _progression.Remove(prog)
                );
            return prog;
        }

    }


    public static class Helper
    {
        public static IObservable<IEnumerable<IObservable<Tout>>> DoParallel<Tin,Tout>(
            this IObservable<IEnumerable<Tin>> source,
            Func<Tin,Tout> action,
            Func<int, IProgressionController> progressionGenerator,
            int threadCount)
        {
            return source
                .Select(items =>
            {
                using var prog = progressionGenerator(items.Count());

                return items
                    .SplitEvenly(threadCount)
                    .Select(group =>
                    {
                        return Observable.Start(() =>
                        {
                            var sub = new ReplaySubject<Tout>();
                            foreach (var item in group)
                                sub.OnNext(action(item));
                            return sub.AsObservable();
                        }, RxApp.TaskpoolScheduler)
                            .Switch();
                    });
            });
        }
    }
}
