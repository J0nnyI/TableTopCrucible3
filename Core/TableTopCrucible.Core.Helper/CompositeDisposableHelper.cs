using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace TableTopCrucible.Core.Helper;

/// <summary>
///     executes the given action when disposed
/// </summary>
public class ActOnLifecycle : IDisposable
{
    private readonly Action _onDispose;

    public ActOnLifecycle(Action onCreate, Action onDispose = null)
    {
        onCreate?.Invoke();
        _onDispose = onDispose;
    }

    public void Dispose()
        => _onDispose?.Invoke();
}

public static class CompositeDisposableHelper
{
    public static void Add(this CompositeDisposable compDisposable, params IDisposable[] disposables)
    {
        compDisposable.Add(disposables as IEnumerable<IDisposable>);
    }

    public static void Add(this CompositeDisposable compDisposable, IEnumerable<IDisposable> disposables)
    {
        disposables.ToList().ForEach(compDisposable.Add);
    }

    /// <summary>
    ///     returns a subject which is fired when the composite disposable is being disposed.
    ///     since this subject uses an internal helper, the result of this method should be stored as field and not called each
    ///     time requiring the subject.
    /// </summary>
    /// <param name="compDisposable"></param>
    /// <returns></returns>
    public static IObservable<Unit> AsSubject(this CompositeDisposable compDisposable)
        => DisposeEmitter.ForComposite(compDisposable).OnDisposed;

    private sealed class DisposeEmitter : IDisposable
    {
        private readonly Subject<Unit> _onDisposed = new();
        private bool disposed;

        private DisposeEmitter()
        {
        }

        public IObservable<Unit> OnDisposed => _onDisposed.AsObservable();

        public void Dispose()
        {
            if (disposed)
                return;
            disposed = true;
            _onDisposed.OnNext(Unit.Default);
            _onDisposed.Dispose();
        }

        public static DisposeEmitter ForComposite(CompositeDisposable compositeDisposable)
            => new DisposeEmitter().DisposeWith(compositeDisposable);
    }
}