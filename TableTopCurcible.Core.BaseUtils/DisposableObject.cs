﻿using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Text;

namespace TableTopCurcible.Core.BaseUtils
{
    public class DisposableObject : IDisposable
    {
        private readonly Subject<Unit> _destroy = new Subject<Unit>();
        protected readonly CompositeDisposable disposables = new CompositeDisposable();
        protected IObservable<Unit> destroy => _destroy;
        protected virtual void onDispose() { }

        #region IDisposable Support
        public bool IsDisposed { get; private set; } = false; // To detect redundant calls

        private void _dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    onDispose();
                    _destroy.OnNext(Unit.Default);
                    _destroy.Dispose();
                    disposables.Dispose();
                }
                IsDisposed = true;
            }
        }
        public void Dispose()
            => _dispose(true);
        #endregion
    }
}
