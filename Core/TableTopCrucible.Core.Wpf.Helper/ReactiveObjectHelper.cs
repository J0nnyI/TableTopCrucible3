using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;

using ReactiveUI;

using TableTopCrucible.Core.Helper;

namespace TableTopCrucible.Core.Wpf.Helper
{
    public static class ReactiveObjectHelper
    {
        public static void WhenActivated<T>(this T item, Func<IEnumerable<IDisposable>> block,
            params Expression<Func<T, object>>[] updatedProperties)
            where T : ReactiveObject, IActivatableViewModel
        {
            ViewForMixins.WhenActivated(item, () =>
            {
                var disposables = block();
                foreach (var property in updatedProperties)
                    item.RaisePropertyChanged(property.GetPropertyName());
                return disposables;
            });
        }

        public static void WhenActivated(this IActivatableViewModel src,
            Func<CompositeDisposable, IEnumerable<IDisposable>> acc)
        {
            src.WhenActivated(() =>
            {
                var disposable = new CompositeDisposable();
                return acc(disposable).Append(disposable);
            });
        }

        public static IObservable<bool> GetIsActivatedChanges(this IActivatableViewModel src)
            => src.Activator.Activated.Select(_ => true).Merge(src.Activator.Deactivated.Select(_ => false));

        public static IDisposable OneWayBind<TV, TVm, TProp>(
            this TV view,
            Expression<Func<TVm, TProp>> vmProperty,
            Expression<Func<TV, TProp>> vProperty
        )
            where TVm : ReactiveObject
            where TV : ReactiveUserControl<TVm>
            => view.OneWayBind(view.ViewModel, vmProperty, vProperty);
        public static IDisposable Bind<TV, TVm, TProp>(
            this TV view,
            Expression<Func<TVm, TProp>> vmProperty,
            Expression<Func<TV, TProp>> vProperty
        )
            where TVm : ReactiveObject
            where TV : ReactiveUserControl<TVm>
            => view.Bind(view.ViewModel, vmProperty, vProperty);
        /// <summary>
        /// bind to a dependency property, used for bindings to attached properties
        /// </summary>
        /// <typeparam name="TV"></typeparam>
        /// <typeparam name="TVm"></typeparam>
        /// <typeparam name="TProp"></typeparam>
        /// <typeparam name="TControl"></typeparam>
        /// <param name="view"></param>
        /// <param name="vmProperty"></param>
        /// <param name="control"></param>
        /// <param name="dProperty"></param>
        /// <param name="scheduler"></param>
        /// <returns></returns>
        public static IDisposable BindTo<T>(
            this IObservable<T> src,
            DependencyObject control,
            DependencyProperty dProperty,
            IScheduler scheduler = null
        ) => src
            .ObserveOn(scheduler ?? RxApp.MainThreadScheduler)
            .Subscribe(value =>control.SetValue(dProperty, value));
    }
}