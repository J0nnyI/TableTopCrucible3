using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MaterialDesignThemes.Wpf;

using ReactiveUI;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;

namespace TableTopCrucible.Domain.Automation.Wpf.Pages.ViewModels
{
    [Singleton(typeof(TagAutomationPageVm))]
    public interface ITagAutomationPage : INavigationPage
    {

    }
    public class TagAutomationPageVm : ReactiveObject, IActivatableViewModel, ITagAutomationPage
    {
        public ViewModelActivator Activator { get; } = new();
        public PackIconKind? Icon => PackIconKind.Tags;
        public Name Title => Name.From("Tagging Automation");
        public NavigationPageLocation PageLocation => NavigationPageLocation.Upper;
        public SortingOrder Position => SortingOrder.From(2);
    }
}
