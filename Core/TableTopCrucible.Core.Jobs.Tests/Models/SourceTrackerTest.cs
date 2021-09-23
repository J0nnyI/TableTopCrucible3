using NUnit.Framework;
using TableTopCrucible.Core.Jobs.Models;
using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
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
    public class SourceTrackerTest : ReactiveObject
    {
        private IProgressTrackingService? progressService;
        public ISourceTrackerController Tracker { get; set; }
        public ISubscribedTrackingViewer Viewer { get; set; }


        private CompositeDisposable _disposables;


        Func<Exception, IObservable<T>> Catcher<T>(string observable = null)
        {
            return new(ex =>
            {
                Assert.Fail((observable ?? typeof(T).Name) + " threw an exception: " + Environment.NewLine + ex);
                return Observable.Empty<T>();
            });
        }

        [SetUp]
        public void BeforeEach()
        {
            Prepare.ApplicationEnvironment();
            this.progressService = Locator.Current.GetService<IProgressTrackingService>();

            this.Tracker = progressService.CreateSourceTracker((Name)"testTracker");
            this.Viewer = Tracker.Subscribe();
            _disposables = new CompositeDisposable(
                Tracker,
                Viewer
            );
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
            Viewer.TargetProgress.Value.Should().Be(1);
            Viewer.JobState.Should().Be(JobState.ToDo);
        }

        [Test]
        public void Increment()
        {
            Tracker.SetTarget((TrackingTarget)5);

            Viewer.CurrentProgress.Value.Should().Be(0, "init (0)");
            Tracker.Increment();
            Viewer.CurrentProgress.Value.Should().Be(1, "0 => 1");
            Viewer.JobState.Should().Be(JobState.InProgress);

            Tracker.Increment((ProgressIncrement)2);
            Viewer.JobState.Should().Be(JobState.InProgress);
            Viewer.CurrentProgress.Value.Should().Be(3, "1 => 3");
        }
        [Test]
        public void UnderFillScenario()
        {

            Tracker.SetTarget((TrackingTarget)5);

            Tracker.Increment();

            Tracker.OnCompleted();
            Viewer.JobState.Should().Be(JobState.Done);
            Viewer.CurrentProgress.Value.Should().Be(Viewer.TargetProgress.Value);
        }



        [Test]
        public void FlushFillScenario()
        {
            Tracker.SetTarget((TrackingTarget)5);
            Tracker.Increment((ProgressIncrement)5);
            Viewer.JobState.Should().Be(JobState.Done);
            Tracker.OnCompleted();
        }

        [Test]
        public void OverFillScenario()
        {
            Tracker.SetTarget((TrackingTarget)5);
            Tracker.Increment((ProgressIncrement)6);
            Viewer.JobState.Should().Be(JobState.Done);
            Tracker.OnCompleted();
        }

        [Test]
        public void LateSubscriptions()
        {
            Tracker.SetTarget((TrackingTarget)5);
            Tracker.Increment();
            Tracker.Increment();
            Tracker.Increment();
            CurrentProgress lateProgress = null;
            Tracker
                .CurrentProgressChanges
                .Subscribe(x => lateProgress = x)
                .DisposeWith(_disposables);

            lateProgress.Should().Be(Viewer.CurrentProgress).And.Be((CurrentProgress)3);

            Tracker.Increment();

            lateProgress.Should().Be(Viewer.CurrentProgress).And.Be((CurrentProgress) 4);
        }
    }
}