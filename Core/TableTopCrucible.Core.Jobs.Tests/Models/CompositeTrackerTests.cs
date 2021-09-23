using NUnit.Framework;
using TableTopCrucible.Core.Jobs.Models;
using System;
using System.Collections.Generic;
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
    public class CompositeTrackerTests:ReactiveObject
    {
        private IProgressTrackingService? progressService;
        public ICompositeTrackerController Tracker { get; set; }
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
            Viewer.TargetProgress.Value.Should().Be(1);
            Viewer.JobState.Should().Be(JobState.ToDo);
        }
        [Test]
        public async Task SingleChildTest()
        {
            var child = Tracker.AddSingle((Name) "firstChild");
            Viewer.CurrentProgress.Should()
                .Be(await child.CurrentProgressChanges.Timeout(TimeSpan.FromMilliseconds(10)).FirstAsync());
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