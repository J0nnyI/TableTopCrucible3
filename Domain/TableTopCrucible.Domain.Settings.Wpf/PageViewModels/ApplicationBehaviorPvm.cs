
using ReactiveUI;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;

namespace TableTopCrucible.Domain.Settings.Wpf.PageViewModels
{
    [Transient(typeof(ApplicationBehaviorPvm))]
    public interface IApplicationBehaviorSettingsPage : ISettingsCategoryPage { }
    public class ApplicationBehaviorPvm : ReactiveObject, IActivatableViewModel, IApplicationBehaviorSettingsPage
    {
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
        public Name Title => Name.From("Application Behavior");
        public SortingOrder Position => SortingOrder.From(2);
    }
}
