using FluentAssertions;

using NUnit.Framework;

using ReactiveUI;

using Splat;

using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using TableTopCrucible.Core.Jobs.Services;
using TableTopCrucible.Core.Jobs.ValueTypes;
using TableTopCrucible.Core.TestHelper;

namespace TableTopCrucible.Core.Jobs.Models.Tests
{
    [TestFixture()]
    public class TrackingServiceTests : ReactiveObject
    {
        private IProgressTrackingService? progressService;

        private CompositeDisposable _disposables;


        [SetUp]
        public void BeforeEach()
        {
            Prepare.ApplicationEnvironment();
            this.progressService = Locator.Current.GetService<IProgressTrackingService>();
            _disposables = new();
        }

        [TearDown]
        public void AfterEach()
        {
            _disposables?.Dispose();
        }

        [Test]
        public void TotalProgress_NoLateAdds()
        {
            var srcA = progressService!.CreateSourceTracker(null, (TrackingTarget)2);
            var srcB = progressService!.CreateSourceTracker(null, (TrackingTarget)2);

            CurrentProgressPercent progress = null;
            progressService.CurrentProgress.Subscribe(prog => progress = prog).DisposeWith(_disposables);

            progress.Should().Be((CurrentProgressPercent)0);
            srcA.Increment();
            progress.Should().Be((CurrentProgressPercent)25);
            srcB.Increment();
            progress.Should().Be((CurrentProgressPercent)50);
            srcA.Increment();
            progress.Should().Be((CurrentProgressPercent)75);
            srcB.Increment();
            progress.Should().Be((CurrentProgressPercent)100);
        }
        [Test]
        public void TotalProgress_LateAdds()
        {
            var srcA = progressService!.CreateSourceTracker(null, (TrackingTarget)2);

            CurrentProgressPercent progress = null;
            progressService.CurrentProgress.Subscribe(prog => progress = prog).DisposeWith(_disposables);

            progress.Should().Be((CurrentProgressPercent)0);
            srcA.Increment();
            progress.Should().Be((CurrentProgressPercent)50);

            var srcB = progressService!.CreateSourceTracker(null, (TrackingTarget)2);
            progress.Should().Be((CurrentProgressPercent)25);

            srcB.Increment();
            progress.Should().Be((CurrentProgressPercent)50);
            srcA.Increment();
            progress.Should().Be((CurrentProgressPercent)75);
            srcB.Increment();
            progress.Should().Be((CurrentProgressPercent)100);
        }
    }
}