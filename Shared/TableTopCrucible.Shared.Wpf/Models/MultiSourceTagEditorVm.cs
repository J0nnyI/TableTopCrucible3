using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using ReactiveUI;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Controller;
using TableTopCrucible.Shared.Wpf.Models;

namespace TableTopCrucible.Shared.Wpf.UserControls.ViewModels
{

    [Transient]
    public interface IMultiSourceTagEditor
    {
        void Init(ITagMultiSourceProvider provider);
    }

    public class MultiSourceTagEditorVm:ReactiveObject, IActivatableViewModel
    {
        public ViewModelActivator Activator { get; } = new();
    }
}
