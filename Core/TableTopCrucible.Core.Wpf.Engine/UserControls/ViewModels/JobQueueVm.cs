using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using TableTopCrucible.Core.DependencyInjection.Attributes;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels
{
    [Singleton(typeof(JobQueueVm))]
    public interface IJobQueue{}
    public class JobQueueVm:ReactiveObject, IActivatableViewModel, IJobQueue
    {
        public ViewModelActivator Activator { get; } = new();
    }
}
