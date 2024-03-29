﻿using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using FluentAssertions;
using NUnit.Framework;
using ReactiveUI;
using Splat;
using TableTopCrucible.Core.Jobs.Helper;
using TableTopCrucible.Core.Jobs.Progression.Models;
using TableTopCrucible.Core.Jobs.Progression.Services;
using TableTopCrucible.Core.Jobs.ValueTypes;
using TableTopCrucible.Core.TestHelper;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Jobs.Tests.Models
{
    [TestFixture]
    public class SourceTrackerTest : ReactiveObject
    {
        [SetUp]
        public void BeforeEach()
        {
            Prepare.ApplicationEnvironment();
            progressService = Locator.Current.GetService<IProgressTrackingService>();

            Tracker = progressService!.CreateSourceTracker((Name)"testTracker");
            Viewer = Tracker.Subscribe();
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

        private IProgressTrackingService progressService;
        public ISourceTracker Tracker { get; set; }
        public ISubscribedTrackingViewer Viewer { get; set; }


        private CompositeDisposable _disposables;


        private Func<Exception, IObservable<T>> Catcher<T>(string observable = null)
        {
            return ex =>
            {
                Assert.Fail((observable ?? typeof(T).Name) + " threw an exception: " + Environment.NewLine + ex);
                return Observable.Empty<T>();
            };
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
            Tracker.SetTarget((TargetProgress)5);

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
            Tracker.SetTarget((TargetProgress)5);

            Tracker.Increment();

            Tracker.Complete();
            Viewer.JobState.Should().Be(JobState.Done);
            Viewer.CurrentProgress.Value.Should().Be(Viewer.TargetProgress.Value);
        }


        [Test]
        public void FlushFillScenario()
        {
            Tracker.SetTarget((TargetProgress)5);
            Tracker.Increment((ProgressIncrement)5);
            Viewer.JobState.Should().Be(JobState.Done);
            Tracker.Complete();
        }

        [Test]
        public void OverFillScenario()
        {
            Tracker.SetTarget((TargetProgress)5);
            Tracker.Increment((ProgressIncrement)6);
            Viewer.JobState.Should().Be(JobState.Done);
            Tracker.Complete();
        }

        [Test]
        public void InstantClose()
        {
            Tracker.Complete();
            Viewer.JobState
                .Should().Be(JobState.Done);
            Viewer.CurrentProgress
                .Should().Be((CurrentProgress)1);
        }

        [Test]
        public void LateSubscriptions()
        {
            Tracker.SetTarget((TargetProgress)5);
            Viewer.JobState.Should().Be(JobState.ToDo);
            Tracker.Increment();
            Tracker.Increment();
            Tracker.Increment();
            CurrentProgress lateProgress = null;
            JobState? jobState = null;
            Tracker
                .CurrentProgressChanges
                .Subscribe(x => lateProgress = x)
                .DisposeWith(_disposables);
            Tracker
                .JobStateChanges
                .Subscribe(x => jobState = x)
                .DisposeWith(_disposables);

            lateProgress.Should().Be(Viewer.CurrentProgress).And.Be((CurrentProgress)3);
            jobState.Should().Be(Viewer.JobState).And.Be(JobState.InProgress);

            Tracker.Increment();

            lateProgress.Should().Be(Viewer.CurrentProgress).And.Be((CurrentProgress)4);
            jobState.Should().Be(Viewer.JobState).And.Be(JobState.InProgress);

            Tracker.Increment();
            lateProgress.Should().Be(Viewer.CurrentProgress).And.Be((CurrentProgress)5);
            jobState.Should().Be(Viewer.JobState).And.Be(JobState.Done);
        }
    }
}