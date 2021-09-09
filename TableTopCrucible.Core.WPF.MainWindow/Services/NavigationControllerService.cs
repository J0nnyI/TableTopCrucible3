
using TableTopCrucible.Core.DI.Attributes;

namespace TableTopCrucible.Core.WPF.MainWindow.Services
{
    [Singleton(typeof(NavigationControllerService))]
    public interface INavigationControllerService
    {

    }
    class NavigationControllerService : INavigationControllerService
    {
    }
}
