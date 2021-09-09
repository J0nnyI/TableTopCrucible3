using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Text;
using ReactiveUI;

namespace TableTopCrucible.Core.BaseUtils
{
    public abstract class DisposableReactiveObject:ReactiveObject, IDisposable
    {
        protected CompositeDisposable disposables = new CompositeDisposable();
        protected Subject<Unit> destroy = new Subject<Unit>();
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.disposables.Dispose();
                this.destroy.OnNext(Unit.Default);
                this.destroy.OnCompleted();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
