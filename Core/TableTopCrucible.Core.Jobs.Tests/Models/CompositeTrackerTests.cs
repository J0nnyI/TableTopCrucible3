using NUnit.Framework;
using TableTopCrucible.Core.Jobs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Splat;
using TableTopCrucible.Core.Jobs.Services;
using TableTopCrucible.Core.TestHelper;

namespace TableTopCrucible.Core.Jobs.Models.Tests
{
    [TestFixture()]
    public class CompositeTrackerTests
    {
        private IProgressTrackingService? progressService;

        [SetUp]
        public void BeforeEach()
        {
            Prepare.ApplicationEnvironment();
            this.progressService = Locator.Current.GetService<IProgressTrackingService>();
        }
        [Test()]
        public void AddSingleTest()
        {
        }
    }
}