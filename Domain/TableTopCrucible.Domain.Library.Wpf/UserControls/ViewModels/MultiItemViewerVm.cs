using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using TableTopCrucible.Core.DependencyInjection.Attributes;

namespace TableTopCrucible.Domain.Library.Wpf.UserControls.ViewModels
{
    [Singleton]
    public interface IMultiItemViewer
    {

    }
    public class MultiItemViewer:ReactiveObject, IActivatableViewModel, IMultiItemViewer
    {
        public ViewModelActivator Activator { get; } = new();
    }
}
