using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace TableTopCrucible.Core.Wpf.Helper
{
    public static class InteractionHelper
    {
        public static IObservable<TOut> Handle<TOut>(this Interaction<Unit, TOut> interaction)
            => interaction.Handle(Unit.Default);
    }
}
