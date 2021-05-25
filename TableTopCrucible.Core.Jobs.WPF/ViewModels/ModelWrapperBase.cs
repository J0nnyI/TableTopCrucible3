
using ReactiveUI;

using System.ComponentModel;
using System.Reactive.Linq;

namespace TableTopCrucible.Core.Jobs.WPF.ViewModels
{
    /// <summary>
    /// wraps the type T so that the property changed event is invoked on the ui thread
    /// </summary>
    /// <typeparam name="T">the original datatype</typeparam>
    internal abstract class ModelWrapperBase<T> : INotifyPropertyChanged where T : INotifyPropertyChanged
    {
        protected readonly T _source;

        public ModelWrapperBase(T source)
        {

            _source = source;
            _source.PropertyChanged += (s, e) =>
            {
                Observable.Start(() =>
                {
                    PropertyChanged?.Invoke(s, e);
                }, RxApp.MainThreadScheduler);
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
