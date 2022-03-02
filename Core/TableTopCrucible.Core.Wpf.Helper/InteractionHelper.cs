using System;
using System.Reactive;
using ReactiveUI;

namespace TableTopCrucible.Core.Wpf.Helper;

public static class InteractionHelper
{
    public static IObservable<TOut> Handle<TOut>(this Interaction<Unit, TOut> interaction)
        => interaction.Handle(Unit.Default);

    public static IDisposable RegisterAutoCompleteHandler<TIn>(this Interaction<TIn, Unit> interaction, Action<TIn> action)
        => interaction.RegisterHandler(handler =>
        {
            action(handler.Input);
            handler.SetOutput(Unit.Default);
        });

    public static IDisposable RegisterAutoCompleteHandler(this Interaction<Unit, Unit> interaction, Action action)
        => interaction.RegisterAutoCompleteHandler(_ =>action());
}