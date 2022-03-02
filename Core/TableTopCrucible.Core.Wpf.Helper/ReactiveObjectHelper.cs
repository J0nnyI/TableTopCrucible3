using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using ReactiveUI;
using ReactiveUI.Validation.Helpers;
using TableTopCrucible.Core.Helper;

namespace TableTopCrucible.Core.Wpf.Helper;

public static class ReactiveObjectHelper
{
    public static void WhenActivated<T>(
        this T item,
        Func<IEnumerable<IDisposable>> block,
        IEnumerable<string> updatedProperties)
        where T : ReactiveObject, IActivatableViewModel
    {
        ViewForMixins.WhenActivated(item, () =>
        {
            var disposables = block();
            foreach (var property in updatedProperties)
                item.RaisePropertyChanged(property);
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

    

    /// <summary>
    /// bind to a dependency property, used for bindings to attached properties
    /// </summary>
    public static IDisposable BindTo<T>(
        this IObservable<T> src,
        DependencyObject control,
        DependencyProperty dProperty,
        IScheduler scheduler = null
    ) => src
        .ObserveOn(scheduler ?? RxApp.MainThreadScheduler)
        .Subscribe(value => control.SetValue(dProperty, value));

    public static IObservable<bool> HasErrorsChanges(this ReactiveValidationObject src)
        => src.WhenAnyValue(x => x.HasErrors);
    public static IObservable<bool> NoErrorsChanges(this ReactiveValidationObject src)
        => src.WhenAnyValue(x => x.HasErrors, faulty=>!faulty);
}