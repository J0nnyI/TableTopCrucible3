using System.Linq;
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
using TableTopCrucible.Infrastructure.Models.Controller;
using TableTopCrucible.Shared.Wpf.UserControls.ViewModels;

namespace TableTopCrucible.Domain.Settings.Wpf.Pages.ViewModels;

[Singleton]
public interface IDevAssist : INavigationPage
{
}

public class DevAssistVm : IDevAssist, IActivatableViewModel
{
    public ICommand AddNotificationsCommand { get; }
    public ICommand AddTrackerCommand { get; }


    public DevAssistVm(
        INotificationService notificationService,
        IProgressTrackingService progressTrackingService,
        ITagEditorChip editChip,
        ITagEditorChip addChip,
        ITagEditor editor)
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
        }, null, RxApp.TaskpoolScheduler);
        AddTrackerCommand = ReactiveCommand.Create(() =>
        {
            progressTrackingService.CreateSourceTracker("test job", (TargetProgress)3).Increment();
        });



        TagCollection availableTags = new();
        TagCollection selectedTags = new();
        availableTags.Edit(editor =>
        {
            editor.AddRange(new[]
            {
                "first tag",
                "second tag",
                "third tag",
                "edit tag",
                "anotherTag"
            }.Select(Tag.From));
        });
        selectedTags.Edit(editor =>
        {
            editor.AddRange(new[]
            {
                "edit tag",
                "anotherTag"
            }.Select(Tag.From));
        });
        EditChip = editChip;
        EditChip.Init((Tag)"edit tag", selectedTags, availableTags, TagEditorWorkMode.View, true);

        AddChip = addChip;
        AddChip.Init(null, selectedTags, availableTags, TagEditorWorkMode.View, true);

        Editor = editor;
        Editor.SelectedTags = selectedTags;
    }

    public PackIconKind? Icon => PackIconKind.DevTo;
    public Name Title => "Dev Tools";
    public NavigationPageLocation PageLocation => NavigationPageLocation.Lower;
    public SortingOrder Position => (SortingOrder)6;
    public ViewModelActivator Activator { get; } = new();
    public ITagEditorChip EditChip { get; }
    public ITagEditorChip AddChip { get; }
    public ITagEditor Editor { get; }
}