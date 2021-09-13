using ReactiveUI;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using TableTopCrucible.Core.Helper;

namespace TableTopCrucible.Core.Wpf.Helper
{
    public static class ReactiveObjectHelper
    {
        public static void WhenActivated<T>(this T item, Func<IEnumerable<IDisposable>> block, params Expression<Func<T, object>>[] updatedProperties)
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
    }
}
