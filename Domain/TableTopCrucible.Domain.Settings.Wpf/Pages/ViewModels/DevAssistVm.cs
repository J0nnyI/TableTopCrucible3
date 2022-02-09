using System.Threading;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using ReactiveUI;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Engine.Services;
using TableTopCrucible.Core.Jobs.Progression.Services;
using TableTopCrucible.Core.Jobs.ValueTypes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;

namespace TableTopCrucible.Domain.Settings.Wpf.Pages.ViewModels
{

    [Singleton]
    public interface IDevAssist : INavigationPage
    {

    }
    public class DevAssistVm : IDevAssist, IActivatableViewModel
    {
        public ICommand AddNotificationsCommand { get; }
        public ICommand AddTrackerCommand { get; }


        public DevAssistVm(INotificationService notificationService, IProgressTrackingService progressTrackingService)
        {
            AddNotificationsCommand = ReactiveCommand.Create(() =>
            {
                notificationService.AddInfo("info notification", "info details");
                Thread.Sleep(5);
                notificationService.AddConfirmation("confirm notification", "confirm details");
                Thread.Sleep(5);
                notificationService.AddWarning("warning notification", "warning details");
                Thread.Sleep(5);
                notificationService.AddError("error notification", "error details");

            },null,RxApp.TaskpoolScheduler);
            AddTrackerCommand = ReactiveCommand.Create(() =>
            {
                progressTrackingService.CreateSourceTracker("test job", (TargetProgress)3).Increment();
            });
        }

        public PackIconKind? Icon => PackIconKind.DevTo;
        public Name Title => "Dev Tools";
        public NavigationPageLocation PageLocation => NavigationPageLocation.Lower;
        public SortingOrder Position => (SortingOrder)6;
        public ViewModelActivator Activator { get; } = new();
    }
}
