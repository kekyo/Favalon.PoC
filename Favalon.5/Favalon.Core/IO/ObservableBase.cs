﻿// This is part of Favalon project - https://github.com/kekyo/Favalon
// Copyright (c) 2019 Kouji Matsui
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Favalon.IO
{
    public abstract class ObservableBase<T> : IObservable<T>
    {
        private sealed class ObservableDisposable : IDisposable
        {
            private readonly ObservableBase<T> parent;
            private readonly IObserver<T> observer;

            public ObservableDisposable(ObservableBase<T> parent, IObserver<T> observer)
            {
                this.parent = parent;
                this.observer = observer;
            }

            public void Dispose() =>
                this.parent.observers.Remove(this.observer);
        }

        private readonly Dictionary<IObserver<T>, ObservableDisposable> observers =
            new Dictionary<IObserver<T>, ObservableDisposable>(EqualityComparer<object>.Default);
        private IObserver<T>[]? observersCache;
        private ObservableBaseAwaitable<T>? awaitable;

        protected ObservableBase()
        {
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            lock (this.observers)
            {
                if (!observers.TryGetValue(observer, out var ico))
                {
                    ico = new ObservableDisposable(this, observer);
                    observers.Add(observer, ico);

                    observersCache = null;
                }

                return ico;
            }
        }

#line hidden
        private IObserver<T>[] GetObservers()
        {
            var observers = observersCache;
            if (observers == null)
            {
                lock (this.observers)
                {
                    observers = observersCache;
                    if (observers == null)
                    {
                        observers = this.observers.Keys.ToArray();
                        observersCache = observers;
                    }
                }
            }

            return observers;
        }

        protected void OnNext(T value)
        {
            foreach (var observer in this.GetObservers())
            {
                observer.OnNext(value);
            }
        }

        protected void OnError(Exception ex)
        {
            foreach (var observer in this.GetObservers())
            {
                observer.OnError(ex);
            }
        }

        protected void OnCompleted()
        {
            foreach (var observer in this.GetObservers())
            {
                observer.OnCompleted();
            }
        }

        public ObservableBaseAwaitable<T> Awaitable
        {
            get
            {
                var awaitable = this.awaitable;
                if (awaitable == null)
                {
                    lock (this)
                    {
                        awaitable = this.awaitable;
                        if (awaitable == null)
                        {
                            awaitable = new ObservableBaseAwaitable<T>(this);
                            this.awaitable = awaitable;
                        }
                    }
                }
                return awaitable;
            }
        }
#line default
    }
}
