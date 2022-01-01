using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
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
        public static void WhenActivated(this IActivatableViewModel src, Func<CompositeDisposable, IEnumerable<IDisposable>> acc)
        {
            src.WhenActivated(() =>
            {
                var disposable = new CompositeDisposable();
                return acc(disposable).Append(disposable);
            });
        }
    }
}