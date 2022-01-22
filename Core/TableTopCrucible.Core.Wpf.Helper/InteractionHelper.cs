using System;
using System.Reactive;
using ReactiveUI;

namespace TableTopCrucible.Core.Wpf.Helper
{
    public static class InteractionHelper
    {
        public static IObservable<TOut> Handle<TOut>(this Interaction<Unit, TOut> interaction)
            => interaction.Handle(Unit.Default);
    }
}