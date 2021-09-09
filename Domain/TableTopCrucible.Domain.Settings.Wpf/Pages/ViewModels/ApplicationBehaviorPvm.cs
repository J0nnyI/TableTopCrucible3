
using MaterialDesignThemes.Wpf;
using ReactiveUI;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;

namespace TableTopCrucible.Domain.Settings.Wpf.PageViewModels
{
    [Transient(typeof(ApplicationBehaviorPvm))]
    public interface IApplicationBehaviorSettingsPage : INavigationPage { }
    public class ApplicationBehaviorPvm : ReactiveObject, IActivatableViewModel, IApplicationBehaviorSettingsPage
    {
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
        public PackIconKind? Icon => PackIconKind.Settings;
        public Name Title => Name.From("Settings");
        public NavigationPageLocation PageLocation => NavigationPageLocation.Lower;
        public SortingOrder Position => SortingOrder.From(2);
    }
}
