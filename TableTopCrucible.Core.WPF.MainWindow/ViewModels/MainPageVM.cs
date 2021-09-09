using ReactiveUI;

using System.ComponentModel;
using System.Reactive.Disposables;

using TableTopCrucible.Core.DI.Attributes;

namespace TableTopCrucible.Core.WPF.MainWindow.ViewModels
{
    [Singleton(typeof(MainPageVM))]
    public interface IMainPage
    {

    }
    public class MainPageVM : ReactiveObject, IActivatableViewModel, INotifyPropertyChanged, IMainPage
    {
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
        public MainPageVM()
        {
            this.WhenActivated((CompositeDisposable destroy) =>
            {
            });
        }
    }
}
