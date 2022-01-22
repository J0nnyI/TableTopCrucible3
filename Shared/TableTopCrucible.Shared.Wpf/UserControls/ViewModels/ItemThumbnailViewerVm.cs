using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using TableTopCrucible.Core.DependencyInjection.Attributes;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{
    [Singleton]
    public interface IItemThumbnailViewer{}
    public class ItemThumbnailViewerVm:ReactiveObject,IActivatableViewModel, IItemThumbnailViewer
    {
        public ViewModelActivator Activator { get; } = new();
    }
}
