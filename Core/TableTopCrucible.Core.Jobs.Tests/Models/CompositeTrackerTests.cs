using NUnit.Framework;
using TableTopCrucible.Core.Jobs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using NUnit.Framework.Internal;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using TableTopCrucible.Core.Jobs.Helper;
using TableTopCrucible.Core.Jobs.Services;
using TableTopCrucible.Core.Jobs.ValueTypes;
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

            this.Tracker = progressService!.CreateNewCompositeTracker((Name)"testTracker");
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

            child.SetTarget((TrackingTarget)5);

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
            var childA = Tracker.AddSingle((Name)"first Tracker", (TrackingTarget)10);
            var viewerA = childA.Subscribe().DisposeWith(_disposables);
            var childB = Tracker.AddSingle((Name)"second Tracker", (TrackingTarget)20);
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
        //[TestCase(nextrand, 200.0, 30.0, 324.0)]
        [Repeat(5)]
        public void MultiChild_Weighted([Random(1, 199, 1)] double weightA, [Random(1, 199, 1)] double targetA, [Random(1, 199, 1)] double weightB, [Random(1, 199, 1)] double targetB)
        {
            var target = CompositeTracker.Target.Value;
            var totalWeight = weightA + weightB;
            var fractionA = weightA / totalWeight;
            var fractionB = weightB / totalWeight;
            var modA = fractionA * target;
            var modB = fractionB * target;

            var childA = Tracker.AddSingle((Name)"first Tracker", (TrackingTarget)targetA, (TrackingWeight)weightA);
            var childB = Tracker.AddSingle((Name)"second Tracker", (TrackingTarget)targetB, (TrackingWeight)weightB);


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
        [Ignore("todo")]
        public void Todo_Done()
        {
        }

        [Test]
        [Ignore("todo")]
        public void Todo_InProgress()
        {

        }

        [Test]
        [Ignore("todo")]
        public void InProgress_Done()
        {

        }

        [Test]
        [Ignore("todo")]
        public void Todo_InProgress_Done()
        {

        }
    }
}