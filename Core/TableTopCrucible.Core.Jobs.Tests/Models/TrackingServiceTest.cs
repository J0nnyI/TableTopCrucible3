using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FluentAssertions;
using NUnit.Framework;
using ReactiveUI;
using Splat;
using TableTopCrucible.Core.Jobs.Progression.Services;
using TableTopCrucible.Core.Jobs.ValueTypes;
using TableTopCrucible.Core.TestHelper;

namespace TableTopCrucible.Core.Jobs.Tests.Models
{
    [TestFixture()]
    public class TrackingServiceTests : ReactiveObject
    {
        private IProgressTrackingService progressService;

        private CompositeDisposable _disposables;


        [SetUp]
        public void BeforeEach()
        {
            Prepare.ApplicationEnvironment();
            progressService = Locator.Current.GetService<IProgressTrackingService>();
            _disposables = new CompositeDisposable();
        }

        [TearDown]
        public void AfterEach()
        {
            _disposables?.Dispose();
        }

        [Test]
        public void TotalProgress_NoLateAdds()
        {
            var srcA = progressService!.CreateSourceTracker(null, (TargetProgress)2);
            var srcB = progressService!.CreateSourceTracker(null, (TargetProgress)2);

            CurrentProgressPercent progress = null;
            progressService.TotalProgress.Subscribe(prog => progress = prog).DisposeWith(_disposables);

            progress.Should().Be((CurrentProgressPercent)0);
            srcA.Increment();
            progress.Should().Be((CurrentProgressPercent)25);
            srcB.Increment();
            progress.Should().Be((CurrentProgressPercent)50);
            srcA.Increment();
            progress.Should().Be((CurrentProgressPercent)75);
            srcB.Increment();
            progress.Should().Be((CurrentProgressPercent)0);
        }

        [Test]
        public void TotalProgress_LateAdds()
        {
            var srcA = progressService!.CreateSourceTracker(null, (TargetProgress)2);

            CurrentProgressPercent lastProgress = null;
            IList<CurrentProgressPercent> emittedValues = null;

            Subject<Unit> bufferClose = new();
            progressService
                .TotalProgress
                .Buffer(bufferClose)
                .Subscribe(progress =>
                {
                    lastProgress = progress.LastOrDefault();
                    emittedValues = progress;
                }).DisposeWith(_disposables);

            bufferClose.OnNext(Unit.Default);
            emittedValues.Count.Should().Be(1);
            lastProgress.Should().Be((CurrentProgressPercent)0);

            srcA.Increment();
            bufferClose.OnNext(Unit.Default);
            emittedValues.Count.Should().Be(1);
            lastProgress.Should().Be((CurrentProgressPercent)50);

            var srcB = progressService!.CreateSourceTracker(null, (TargetProgress)2);
            bufferClose.OnNext(Unit.Default);
            emittedValues.Count.Should().Be(1);
            lastProgress.Should().Be((CurrentProgressPercent)25);

            srcB.Increment();
            bufferClose.OnNext(Unit.Default);
            emittedValues.Count.Should().Be(1);
            lastProgress.Should().Be((CurrentProgressPercent)50);

            srcA.Increment();
            bufferClose.OnNext(Unit.Default);
            emittedValues.Count.Should().Be(1);
            lastProgress.Should().Be((CurrentProgressPercent)75);

            srcB.Increment();
            bufferClose.OnNext(Unit.Default);
            emittedValues.Count.Should().Be(1);
            lastProgress.Should().Be((CurrentProgressPercent)0);
        }
    }
}