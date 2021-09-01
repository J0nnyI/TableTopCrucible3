using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ReactiveUI;

using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.Services;
using TableTopCrucible.Infrastructure.Repositories.Models.ValueTypes;

using TableTopCtucible.Core.DependencyInjection.Attributes;

namespace TableTopCrucible.Domain.Settings.Wpf.PageViewModels
{
    [Transient(typeof(ApplicationBehaviorPvm))]
    public interface IApplicationBehaviorSettingsPage : ISettingsCategoryPage { }
    public class ApplicationBehaviorPvm : ReactiveObject, IActivatableViewModel, IApplicationBehaviorSettingsPage
    {
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
        public Name Title => Name.From("Application Behavior");
    }
}
