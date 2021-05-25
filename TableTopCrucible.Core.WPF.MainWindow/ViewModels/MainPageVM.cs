using ReactiveUI;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Text;

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
