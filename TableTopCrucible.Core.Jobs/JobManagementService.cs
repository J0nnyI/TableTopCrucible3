using DynamicData;

using ReactiveUI;

using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;

using TableTopCrucible.Core.DI.Attributes;

namespace TableTopCrucible.Core.Jobs
{
    [Singleton(typeof(JobManagementService))]
    public interface IJobManagementService
    {
        IObservableList<IJobInfo> Jobs { get; }
        IJobInfo<T> Start<T>(Action<IJobHandler<T>> task, IObservable<Unit> startTrigger = null);
        IJobInfo<Tres> Start<Tpre, Tres>(Action<IJobHandler<Tres>, Tpre> task, IObservable<Tpre> previousTask = null);
    }
    public class JobManagementService: IJobManagementService
    {
        private readonly SourceList<IJobInfo> _jobs = new SourceList<IJobInfo>();
        public IObservableList<IJobInfo> Jobs => _jobs;
        public IJobInfo<Tres> Start<Tres>(Action<IJobHandler<Tres>> task, IObservable<Unit> pre = null)
            => Start<Unit, Tres>((job, _) => task(job), pre);
        public IJobInfo<Tres> Start<Tpre, Tres>(Action<IJobHandler<Tres>, Tpre> task, IObservable<Tpre> previousTask=null)
        {
            var job = new Job<Tpre, Tres>(task, this, previousTask);
            this._jobs.Add(job);
            return job;
        }
    }
#if FALSE
    static class Demo
    {
        public static void testc()
        {
            IJobManagementService srv = new JobManagementService();
            var info = srv.Start<string>(job =>
            {
                using (var mainProg = job.TrackProgression(10, "main counter", "just counting"))
                {

                    for (int y = 0; y < 10; y++)
                    {
                        mainProg.Details = y.ToString();

                        if (y > 10)
                            job.Fail(new Exception());
                        
                        using var subProg = job.TrackProgression(10, "subProg", "just counting some more");

                        for (int x = 0; x < 10; x++)
                        {
                            subProg.CurrentProgress++;
                            subProg.Details = y.ToString();
                        }


                    }
                    mainProg.CurrentProgress++;
                }

                job.Complete("success");
            }).Then<int>(job2=>
            {
            }).ThenParallel<string>(a=> { }, b=>{ }).Then<string>(x=> { });
        }
    }
#endif
}


