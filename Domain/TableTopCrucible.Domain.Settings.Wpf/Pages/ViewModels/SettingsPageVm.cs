using MaterialDesignThemes.Wpf;

using ReactiveUI;

using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;

namespace TableTopCrucible.Domain.Settings.Wpf.Pages.ViewModels
{
    [Transient(typeof(SettingsPageVm))]
    public interface ISettingsPage : INavigationPage { }
    public class SettingsPageVm : ReactiveObject, IActivatableViewModel, ISettingsPage
    {
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
        public PackIconKind? Icon => PackIconKind.Settings;
        public Name Title => Name.From("Settings");
        public NavigationPageLocation PageLocation => NavigationPageLocation.Lower;
        public SortingOrder Position => SortingOrder.From(2);
    }
}
