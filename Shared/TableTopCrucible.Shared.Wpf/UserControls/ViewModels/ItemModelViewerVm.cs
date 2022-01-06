using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using TableTopCrucible.Core.DependencyInjection.Attributes;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    [Transient]
    public interface IItemModelViewer
    {

    }
    public class ItemModelViewerVm:ReactiveObject,IActivatableViewModel, IItemModelViewer
    {
        public ViewModelActivator Activator { get; } = new();
    }
}
