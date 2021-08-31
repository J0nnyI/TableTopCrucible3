using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace TableTopCrucible.Domain.Settings.Wpf.PageViewModels
{
    public class ApplicationBehaviorPvm:ReactiveObject, IActivatableViewModel
    {
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
