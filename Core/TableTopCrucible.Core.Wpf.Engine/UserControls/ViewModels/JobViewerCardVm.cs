using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Jobs.Helper;
using TableTopCrucible.Core.Jobs.Progression.Models;

namespace TableTopCrucible.Core.Wpf.Engine.UserControls.ViewModels
{
    [Transient]
    public interface IJobViewerCard
    {
        ITrackingViewer Viewer { get; set; }
    }

    public class JobViewerCardVm : ReactiveObject, IJobViewerCard, IActivatableViewModel
    {
        [Reactive]
        public ITrackingViewer Viewer { get; set; }


        public ViewModelActivator Activator { get; } = new();
    }
}