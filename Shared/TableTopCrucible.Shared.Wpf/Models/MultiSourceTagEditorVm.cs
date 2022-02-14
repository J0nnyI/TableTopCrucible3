using System.Diagnostics;
using ReactiveUI;
using TableTopCrucible.Core.DependencyInjection.Attributes;

namespace TableTopCrucible.Shared.Wpf.Models
{

    [Transient]
    public interface IMultiSourceTagEditor
    {
        void Init(ITagMultiSourceProvider provider);
    }

    public class MultiSourceTagEditorVm:ReactiveObject, IActivatableViewModel, IMultiSourceTagEditor
    {
        public ViewModelActivator Activator { get; } = new();
        public void Init(ITagMultiSourceProvider provider)
        {
           Debugger.Break();
        }
    }
}
