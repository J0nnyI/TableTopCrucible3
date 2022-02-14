using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Jobs.Progression.Models;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels;

[Transient]
public interface IJobViewerCard
{
    ITrackingViewer Viewer { get; set; }
}

public class JobViewerCardVm : ReactiveObject, IJobViewerCard, IActivatableViewModel
{
    public ViewModelActivator Activator { get; } = new();

    [Reactive]
    public ITrackingViewer Viewer { get; set; }
}