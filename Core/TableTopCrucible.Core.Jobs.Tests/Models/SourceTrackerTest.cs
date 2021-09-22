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
using TableTopCrucible.Core.Jobs.Services;
using TableTopCrucible.Core.Jobs.ValueTypes;
using TableTopCrucible.Core.TestHelper;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Jobs.Models.Tests
{
    [TestFixture()]
    public class SourceTrackerTest : ReactiveObject
    {
        private IProgressTrackingService? progressService;
        [Reactive]
        public ISourceTrackerController Tracker { get; set; }

        private ObservableAsPropertyHelper<JobState> _trackerJobState;
        public JobState TrackerJobState => _trackerJobState.Value;

        private ObservableAsPropertyHelper<CurrentProgress> _currentProgress;
        public CurrentProgress CurrentProgress => _currentProgress.Value;
        
        private ObservableAsPropertyHelper<TrackingTarget> _targetProgress;
        public TrackingTarget TargetProgress => _targetProgress.Value;


        private CompositeDisposable _disposables;


        Func<Exception, IObservable<T>> Catcher<T>(string observable=null)
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
            _disposables = new CompositeDisposable(
                Tracker,
                this.Tracker.CurrentProgressChanges
                    .Catch(Catcher<CurrentProgress>())
                    .ToProperty(this, t => t.CurrentProgress, out _currentProgress),
                this.Tracker.TargetProgressChanges
                    .Catch(Catcher<TrackingTarget>())
                    .ToProperty(this, t => t.TargetProgress, out _targetProgress),
                this.Tracker.JobStateChanges
                    .Catch(Catcher<JobState>())
                    .ToProperty(this, t => t.TrackerJobState, out _trackerJobState)
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
            TargetProgress.Value.Should().Be(1);
            TrackerJobState.Should().Be(JobState.ToDo);
        }
        [Test]
        public void DefaultTrackingWithManualComplete()
        {

            Tracker.SetTarget((TrackingTarget)5);
            TargetProgress.Value.Should().Be(5,"new");
            TrackerJobState.Should().Be(JobState.ToDo);

            CurrentProgress.Value.Should().Be(0,"init (0)");
            Tracker.Increment();
            CurrentProgress.Value.Should().Be(1, "0 => 1");
            TrackerJobState.Should().Be(JobState.InProgress);

            Tracker.Increment((ProgressIncrement)2);
            TrackerJobState.Should().Be(JobState.InProgress);
            CurrentProgress.Value.Should().Be(3,"1 => 3");

            Tracker.OnCompleted();
            TrackerJobState.Should().Be(JobState.Done);
            CurrentProgress.Value.Should().Be(TargetProgress.Value, "3 => complete (disposed)");
        }

        [Test]
        public void DefaultTrackingWithAutoComplete()
        {
            Tracker.SetTarget((TrackingTarget)5);
            Tracker.Increment((ProgressIncrement)5);
            TargetProgress.Should().Be((TrackingTarget) 5);
            CurrentProgress.Should().Be((CurrentProgress) 5);
            TrackerJobState.Should().Be(JobState.Done);
        }
    }
}