
using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Core.WPF.Helper.Attributes;
using TableTopCrucible.DomainCore.WPF.Toolbar.Views;

namespace TableTopCrucible.DomainCore.WPF.Toolbar.ViewModels
{
    [Singleton(typeof(ToolbarVM))]
    public interface IToolbar
    {

    }
    [ViewModel(typeof(ToolbarV))]
    internal class ToolbarVM : IToolbar
    {
    }
}
