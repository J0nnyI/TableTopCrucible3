using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Transactions;
using System.Windows.Markup;

using DynamicData;
using DynamicData.Kernel;
using FluentAssertions;

using Microsoft.Reactive.Testing;

using NUnit.Framework;

using ReactiveUI;

using Splat;
using TableTopCrucible.Core.Jobs.JobQueue.Models;
using TableTopCrucible.Core.Jobs.Progression.Models;
using TableTopCrucible.Core.Jobs.Progression.Services;
using TableTopCrucible.Core.Jobs.ValueTypes;
using TableTopCrucible.Core.TestHelper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;

namespace TableTopCrucible.Core.Wpf.Engine.Tests.UserControls.ViewModels
{
    [TestFixture()]
    public class JobQueueVmTests
    {
        public CompositeDisposable Disposables;
        public IProgressTrackingService ProgressService;
        public JobQueueVm JobQueueVm;

        [SetUp]
        public void BeforeEach()
        {
            RxApp.TaskpoolScheduler = RxApp.MainThreadScheduler;
            Prepare.ApplicationEnvironment();
            JobQueueVm = Locator.Current.GetService<IJobQueue>() as JobQueueVm;
            ProgressService = Locator.Current.GetService<IProgressTrackingService>();
            Disposables = new CompositeDisposable();
        }


        [TearDown]
        public void AfterEach()
        {
            JobQueueVm.Activator.Deactivate();
        }

        [Test]
        public void SetupWorks()
        {
            JobQueueVm.Should().NotBeNull();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterState">the state which the VM filters for</param>
        /// <param name="lateUpdate">whether the jobs have been updated before or after the component was activated</param>
        [Test]
        [TestCase(JobState.ToDo, true)]
        [TestCase(JobState.InProgress, true)]
        [TestCase(JobState.Done, true)]
        [TestCase(JobState.ToDo, false)]
        [TestCase(JobState.InProgress, false)]
        [TestCase(JobState.Done, false)]
        public void FilteredList(JobState filterState, bool lateUpdate)
        {
            JobQueueVm.JobFilter = JobFilter.FromState(filterState);
            if (!lateUpdate)
            {
                JobQueueVm.Activator.Activate();
            }
            var todo = ProgressService.CreateSourceTracker((Name)JobState.ToDo.ToString())
                .DisposeWith(Disposables);
            var inProgress = ProgressService.CreateSourceTracker((Name)JobState.InProgress.ToString(), (TargetProgress)2)
                .DisposeWith(Disposables);
            var done = ProgressService.CreateSourceTracker((Name)JobState.Done.ToString())
                .DisposeWith(Disposables);
            
            inProgress.Increment();
            done.Complete();

            if (lateUpdate)
            {
                JobQueueVm.Activator.Activate();
            }

            JobQueueVm
                .Cards
                .Select(card => card.Viewer.Title)
                .Should()
                .BeEquivalentTo(
                new[]
                {
                    (Name) filterState.ToString()
                });
        }

        public void TripleFilter()
        {
            var todoJob = ProgressService.CreateSourceTracker((Name)JobState.ToDo.ToString());
            var inProgressJob = ProgressService.CreateSourceTracker((Name)JobState.InProgress.ToString(), (TargetProgress)2);
            var doneJob = ProgressService.CreateSourceTracker((Name)JobState.Done.ToString());



            inProgressJob.Increment();
            doneJob.Complete();

            //JobQueueVm.JobFilter = JobFilter.FromState(filterState);
            JobQueueVm.Activator.Activate();

            //JobQueueVm.Cards.Should().ContainSingle(card => card.Viewer.Title == (Name)filterState.ToString());
        }


        private void setup(JobState targetState)
        {
        }

        private void testState<T>(T targetState,T preSubState, IObservable<T> srcObservable)
        {
            preSubState.Should().Be(targetState);
            var x = Optional.None<T>();
            srcObservable.Take(1).Subscribe(y => x = Optional.Some(y));
            x.HasValue.Should().BeTrue();
            x.Value.Should().Be(targetState);

        }
        [Test]
        [TestCase(JobState.ToDo)]
        [TestCase(JobState.InProgress)]
        [TestCase(JobState.Done)]
        public void FilterChange(JobState state)
        {
            bool? visible = null;
            ISourceTracker job;
            IObservable<bool> filter = null;
            JobQueueVm.JobFilter = JobFilter.FromState(state);

            job = ProgressService.CreateSourceTracker((Name)"Tracker", (TargetProgress)2);

            filter = JobQueueVm.JobToFilter(job);

            visible = null;
            filter.Subscribe(x => visible = x);

            testState(state == JobState.ToDo, visible.Value, filter);
            job.Increment();
            testState(state == JobState.InProgress, visible.Value, filter);
            job.Increment();
            testState(state == JobState.Done, visible.Value, filter);
        }
        /// <summary>
        /// tests, whether a simplified jobList can be properly filtered
        /// </summary>
        //public void simplifiedListFilter()
        //{
        //    bool? visible = null;
        //    ISourceTracker job;
        //    IObservable<bool> filter = null;
        //    JobQueueVm.JobFilter = JobFilter.FromState(state);

        //    job = ProgressService.CreateSourceTracker((Name)"Tracker", (TargetProgress)2);

        //    filter = JobQueueVm.JobToFilter(job);

        //    visible = null;
        //    filter.Subscribe(x => visible = x);

        //    testState(state == JobState.ToDo, visible.Value, filter);
        //    job.Increment();
        //    testState(state == JobState.InProgress, visible.Value, filter);
        //    job.Increment();
        //    testState(state == JobState.Done, visible.Value, filter);

        //}
    }


}