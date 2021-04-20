using Microsoft.Extensions.Logging;

using TableTopCrucible.App.WPF.Views;
using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Core.Jobs.WPF.ViewModels;
using TableTopCrucible.Core.WPF.Helper.Attributes;
using TableTopCrucible.Data.Library.Models.ValueTypes.General;
using TableTopCrucible.Data.Library.Services.Sources;
using TableTopCrucible.Data.Models.Sources;
using TableTopCrucible.Domain.WPF.Library.PageViewModels;
using TableTopCrucible.DomainCore.FileIntegration;
using TableTopCrucible.DomainCore.WPF.Toolbar.ViewModels;

namespace TableTopCrucible.App.WPF.ViewModels
{
    [Transient(typeof(MainPageVM))]
    public interface IMainPage
    {
    }

    [ViewModel(typeof(MainPageV))]
    internal class MainPageVM : IMainPage
    {
        public MainPageVM(
            IToolbar toolbar,
            ILibraryPage libraryPage,
            IFileSynchronizationService fileLoaderService,
            IFileSetupService fileSetupService,
            IJobOverview jobOverview,
            ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<MainPageVM>();
            Toolbar = toolbar;
            LibraryPage = libraryPage;
            JobOverview = jobOverview;
            fileSetupService.AddOrUpdateDirectory(new SourceDirectory(@"D:\3d Demofiles", new DirectorySetupName("test 1"), new Description("demo files")));
            fileLoaderService.StartSync();
            logger.LogDebug("initialized");
        }

        private readonly ILogger<MainPageVM> logger;

        public IToolbar Toolbar { get; }
        public ILibraryPage LibraryPage { get; }
        public IJobOverview JobOverview { get; }
    }
}
