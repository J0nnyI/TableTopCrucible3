using FluentAssertions;

using NUnit.Framework;
using NUnit.Framework.Internal;

using ReactiveUI;

using Splat;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using TableTopCrucible.Core.Jobs.Helper;
using TableTopCrucible.Core.Jobs.Progression.Models;
using TableTopCrucible.Core.Jobs.Progression.Services;
using TableTopCrucible.Core.Jobs.Progression.ValueTypes;
using TableTopCrucible.Core.TestHelper;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Jobs.Models.Tests
{
    [TestFixture]
    [TestOf(typeof(CompositeTracker))]
    public class CompositeTrackerTests : ReactiveObject
    {
        private IProgressTrackingService? progressService;
        public ICompositeTrackerController Tracker { get; set; }
        public ISubscribedTrackingViewer Viewer { get; set; }


        private CompositeDisposable _disposables;


        Func<Exception, IObservable<T>> Catcher<T>(string observable = null)
        {
            return ex =>
            {
                Assert.Fail((observable ?? typeof(T).Name) + " threw an exception: " + Environment.NewLine + ex);
                return Observable.Empty<T>();
            };
        }

        [SetUp]
        public void BeforeEach()
        {
            Prepare.ApplicationEnvironment();
            this.progressService = Locator.Current.GetService<IProgressTrackingService>();

            this.Tracker = progressService!.CreateCompositeTracker((Name)"testTracker");
            Viewer = Tracker.Subscribe();
            _disposables = new CompositeDisposable(Viewer);
        }

        [TearDown]
        public void AfterEach()
        {
            _disposables?.Dispose();
        }
        [Test]
        public void InitialValues()
        {
            Tracker.Title.Value.Should().Be("testTracker");
            Viewer.TargetProgress.Value.Should().Be(CompositeTracker.Target.Value);
            Viewer.JobState.Should().Be(JobState.ToDo);
        }
        [Test]
        public void SingleChildTest()
        {
            var child = Tracker
                .AddSingle((Name)"firstChild")
                .DisposeWith(_disposables);
            var childViewer = child
                .Subscribe()
                .DisposeWith(_disposables);

            // initial
            Viewer.CurrentProgress
                .Should()
                .Be((CurrentProgress)0);

            Viewer.JobState
                .Should()
                .Be(childViewer.JobState)
                .And
                .Be(JobState.ToDo);

            childViewer.Title.Should().Be((Name)"firstChild");

            child.SetTarget((TargetProgress)5);

            // 0 => 1
            child.Increment();

            Viewer.CurrentProgress
                .Should()
                .Be((CurrentProgress)20);//20% done

            Viewer.JobState
                .Should()
                .Be(childViewer.JobState)
                .And
                .Be(JobState.InProgress);

            // 1 => 3
            child.Increment((ProgressIncrement)2);

            Viewer.CurrentProgress
                .Should()
                .Be((CurrentProgress)60);

            Viewer.JobState
                .Should()
                .Be(childViewer.JobState)
                .And
                .Be(JobState.InProgress);

            // 3 => done
            child.OnCompleted();

            Viewer.CurrentProgress.Value
                .Should()
                .Be(Viewer.TargetProgress.Value)
                .And
                .Be(100);

            Viewer.JobState
                .Should()
                .Be(childViewer.JobState)
                .And
                .Be(JobState.Done);
        }

        [Test]
        public void MultiChild_NoWeight()
        {
            var childA = Tracker.AddSingle((Name)"first Tracker", (TargetProgress)10);
            var viewerA = childA.Subscribe().DisposeWith(_disposables);
            var childB = Tracker.AddSingle((Name)"second Tracker", (TargetProgress)20);
            var viewerB = childB.Subscribe().DisposeWith(_disposables);

            Viewer.JobState
                .Should().Be(viewerA.JobState)
                .And.Be(viewerB.JobState)
                .And.Be(JobState.ToDo);

            Viewer.CurrentProgress
                .Should().Be((CurrentProgress)0);


            childA.Increment((ProgressIncrement)5);

            Viewer.JobState
                .Should().Be(viewerA.JobState)
                .And.Be(JobState.InProgress);

            childB.Increment((ProgressIncrement)10);

            Viewer.CurrentProgress.Value
                .Should().Be(CompositeTracker.Target.Value * 0.5);

            childA.OnCompleted();
            Viewer.CurrentProgress.Value
                .Should().Be(CompositeTracker.Target.Value * 0.75);

            Viewer.JobState
                .Should().Be(JobState.InProgress);

            childB.OnCompleted();
            Viewer.CurrentProgress.Value
                .Should().Be(CompositeTracker.Target.Value * 1.0);

        }

        [Test]
        [TestCase(10.0, 200.0, 30.0, 324.0)]
        [TestCase(32465.45, 43265.32, 1234.13, 54678.45)]
        public void MultiChild_Weighted(double weightA, double targetA, double weightB, double targetB)
        {
            var target = CompositeTracker.Target.Value;
            var totalWeight = weightA + weightB;
            var fractionA = weightA / totalWeight;
            var fractionB = weightB / totalWeight;
            var modA = fractionA * target;
            var modB = fractionB * target;

            var childA = Tracker.AddSingle((Name)"first Tracker", (TargetProgress)targetA, (JobWeight)weightA);
            var childB = Tracker.AddSingle((Name)"second Tracker", (TargetProgress)targetB, (JobWeight)weightB);


            // sum of 1 makes for easy testing and includes conversion of factor 100

            childA.Increment((ProgressIncrement)(targetA * .5));// percent of the sub-tracker
            Viewer.CurrentProgress
                .Should().Be((CurrentProgress)(CompositeTracker.Target.Value * (modA * .5 + modB * 0) / 100));

            childB.Increment((ProgressIncrement)(targetB * .35));
            Viewer.CurrentProgress
                .Should().Be((CurrentProgress)(CompositeTracker.Target.Value * (modA * .5 + modB * .35) / 100));

            childB.OnCompleted();
            Viewer.CurrentProgress
                .Should().Be((CurrentProgress)(CompositeTracker.Target.Value * (modA * .5 + modB * 1) / 100));

            childA.OnCompleted();
            Viewer.CurrentProgress
                .Should().Be((CurrentProgress)(CompositeTracker.Target.Value * (modA * 1 + modB * 1) / 100));
        }

        [Test]
        public void NestedComposite()
        {
            RxApp.MainThreadScheduler = Scheduler.Default;
            RxApp.TaskpoolScheduler = Scheduler.Default;

            Subject<Unit> bufferClose = new();
            JobState[] stateChangesUpdates = null;
            CurrentProgress[] currentProgressUpdates = null;
            Viewer.JobStateChanges
                .Buffer(bufferClose)
                .Subscribe(items => stateChangesUpdates = items.ToArray())
                .DisposeWith(_disposables);
            Viewer.CurrentProgressChanges
                .Buffer(bufferClose)
                .Subscribe(items => currentProgressUpdates = items.ToArray())
                .DisposeWith(_disposables);

            //stateChangesUpdates.Should().Be(1);
            //currentProgressUpdates.Should().Be(1);

            var upperChild = Tracker.AddSingle(null, (TargetProgress)2);
            var upperChildViewer = upperChild.Subscribe().DisposeWith(_disposables);

            var nestedComp = Tracker.AddComposite();
            var nestedCompViewer = nestedComp.Subscribe().DisposeWith(_disposables);

            var lowerChild = nestedComp.AddSingle(null, (TargetProgress)2);
            var lowerChildViewer = lowerChild.Subscribe().DisposeWith(_disposables);


            // ( 0%)    
            // -   0%   0   2
            // -(  0%)  
            //   -   0% 0   2
            // / => ToDo
            Viewer.JobState
                .Should().Be(upperChildViewer.JobState)
                .And.Be(nestedCompViewer.JobState)
                .And.Be(lowerChildViewer.JobState)
                .And.Be(JobState.ToDo);

            Viewer.CurrentProgress
                .Should().Be(upperChildViewer.CurrentProgress)
                .And.Be(nestedCompViewer.CurrentProgress)
                .And.Be(lowerChildViewer.CurrentProgress)
                .And.Be((CurrentProgress)0);

            bufferClose.OnNext(Unit.Default);
            currentProgressUpdates.Should().BeEquivalentTo(new[] { (CurrentProgress)0 });
            stateChangesUpdates.Should().BeEquivalentTo(new[] { JobState.ToDo });

            // ( 25%)   
            // -   0%   0   2
            // -( 50%)  
            //   -  50% 1   2
            // ToDo => InProgress
            lowerChild.Increment();

            Viewer.JobState
                .Should().Be(nestedCompViewer.JobState)
                .And.Be(lowerChildViewer.JobState)
                .And.Be(JobState.InProgress);
            upperChildViewer.JobState
                .Should().Be(JobState.ToDo);

            Viewer.CurrentProgress
                .Should().Be((CurrentProgress)25);
            upperChildViewer.CurrentProgress.Value
                .Should().Be(0);
            nestedCompViewer.CurrentProgress
                .Should().Be((CurrentProgress)50);
            lowerChildViewer.CurrentProgress.Value
                .Should().Be(1);

            bufferClose.OnNext(Unit.Default);
            currentProgressUpdates.Should().BeEquivalentTo(new[] { (CurrentProgress)25 });
            stateChangesUpdates.Should().BeEquivalentTo(new[] { JobState.InProgress });

            // ( 50%)
            // -  50%   1   2
            // -( 50%)
            //   -  50% 1   2
            // InProgress
            upperChild.Increment();

            Viewer.JobState
                .Should().Be(nestedCompViewer.JobState)
                .And.Be(lowerChildViewer.JobState)
                .And.Be(upperChildViewer.JobState)
                .And.Be(JobState.InProgress);

            Viewer.CurrentProgress
                .Should().Be((CurrentProgress)50);
            upperChildViewer.CurrentProgress.Value
                .Should().Be(1);
            nestedCompViewer.CurrentProgress
                .Should().Be((CurrentProgress)50);
            lowerChildViewer.CurrentProgress.Value
                .Should().Be(1);

            bufferClose.OnNext(Unit.Default);
            currentProgressUpdates.Should().BeEquivalentTo(new[] { (CurrentProgress)50 });
            stateChangesUpdates.Should().BeEmpty();


            // ( 75%)
            // -  50%   1   2
            // -(100%)
            //   - 100% 2   2
            // InProgress
            lowerChild.Increment();

            Viewer.JobState
                .Should().Be(upperChildViewer.JobState)
                .And.Be(JobState.InProgress);
            nestedCompViewer.JobState
                .Should().Be(lowerChildViewer.JobState)
                .And.Be(JobState.Done);

            Viewer.CurrentProgress
                .Should().Be((CurrentProgress)75);
            upperChildViewer.CurrentProgress.Value
                .Should().Be(1);
            nestedCompViewer.CurrentProgress
                .Should().Be((CurrentProgress)100);
            lowerChildViewer.CurrentProgress.Value
                .Should().Be(2);

            bufferClose.OnNext(Unit.Default);
            currentProgressUpdates.Should().BeEquivalentTo(new[] { (CurrentProgress)75 });
            stateChangesUpdates.Should().BeEmpty();

            // (100%)
            // -  100%  2   2
            // -(100%)
            //   - 100% 2   2
            // InProgress => Done
            upperChild.Increment();

            Viewer.JobState
                .Should().Be(nestedCompViewer.JobState)
                .And.Be(lowerChildViewer.JobState)
                .And.Be(upperChildViewer.JobState)
                .And.Be(JobState.Done);

            Viewer.CurrentProgress
                .Should().Be(nestedCompViewer.CurrentProgress)
                .And.Be((CurrentProgress)100);

            lowerChildViewer.CurrentProgress.Value
                .Should().Be(upperChildViewer.CurrentProgress.Value)
                .And.Be(2);

            bufferClose.OnNext(Unit.Default);
            currentProgressUpdates.Should().BeEquivalentTo(new[] { (CurrentProgress)100 });
            stateChangesUpdates.Should().BeEquivalentTo(new[] { JobState.Done });
        }

        [Test]
        public void Todo_Todo()
        {
            var childA = Tracker.AddSingle();
            var childB = Tracker.AddSingle();
            Viewer.JobState
                .Should().Be(JobState.ToDo);
        }
        [Test]
        public void InProgress_InProgress()
        {
            var childA = Tracker.AddSingle(null, (TargetProgress)2);
            var childB = Tracker.AddSingle(null, (TargetProgress)2);
            childA.Increment();
            childB.Increment();
            Viewer.JobState
                .Should().Be(JobState.InProgress);
        }
        [Test]
        public void Done_Done()
        {
            var childA = Tracker.AddSingle();
            var childB = Tracker.AddSingle();
            childA.OnCompleted();
            childB.OnCompleted();
            Viewer.JobState
                .Should().Be(JobState.Done);
        }

        [Test]
        public void Todo_Done()
        {
            var childA = Tracker.AddSingle();
            var childB = Tracker.AddSingle();
            childA.OnCompleted();
            childB.OnCompleted();
            Viewer.JobState
                .Should().Be(JobState.Done);
        }

        [Test]
        public void Todo_InProgress()
        {
            var childA = Tracker.AddSingle(null, (TargetProgress)2);
            var childB = Tracker.AddSingle();
            childA.Increment();
            Viewer.JobState
                .Should().Be(JobState.InProgress);
        }


        [Test]
        public void InProgress_Done()
        {
            var childA = Tracker.AddSingle(null, (TargetProgress)2);
            var childB = Tracker.AddSingle();
            childA.Increment();
            childB.OnCompleted();
            Viewer.JobState
                .Should().Be(JobState.InProgress);
        }

        [Test]
        public void Todo_InProgress_Done()
        {
            var childA = Tracker.AddSingle();
            var childB = Tracker.AddSingle();
            var childC = Tracker.AddSingle();
            childB.Increment();
            childC.OnCompleted();
            Viewer.JobState
                .Should().Be(JobState.InProgress);
        }

        [Test]
        [Ignore("todo")]
        public void LateDoubleSupscription()
        {

        }
    }
}