using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.ValueTypes;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels
{
    [Singleton(typeof(JobQueueVm))]
    public interface IJobQueue : ISidebarPage
    {}
    public class JobQueueVm:ReactiveObject, IActivatableViewModel, IJobQueue
    {
        public ViewModelActivator Activator { get; } = new();
        public Name Title => (Name) "Job Queue";
        public SidebarWidth Width => null;
    }
}
