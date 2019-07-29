// This is part of Favalon project - https://github.com/kekyo/Favalon
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
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Favalon.IO
{
    public sealed class ObservableBaseAwaitable<T> :
        IObserver<T>, IDisposable
    {
        private readonly IDisposable disposable;
        private readonly Queue<object?> valueQueue = new Queue<object?>();
        private readonly Queue<TaskCompletionSource<T>> tcsQueue = new Queue<TaskCompletionSource<T>>();

        internal ObservableBaseAwaitable(ObservableBase<T> parent) =>
            disposable = parent.Subscribe(this);

        public void Dispose()
        {
            disposable.Dispose();
            foreach (var tcs in tcsQueue)
            {
                tcs.TrySetCanceled();
            }
            valueQueue.Clear();
        }

        void IObserver<T>.OnNext(T value)
        {
            Monitor.Enter(this);
            if (tcsQueue.Count >= 1)
            {
                var tcs = tcsQueue.Dequeue();
                Monitor.Exit(this);

                tcs.TrySetResult(value);
            }
            else
            {
                valueQueue.Enqueue(value);
                Monitor.Exit(this);
            }
        }

        void IObserver<T>.OnCompleted() =>
            this.Dispose();

        void IObserver<T>.OnError(Exception ex)
        {
            Monitor.Enter(this);
            if (tcsQueue.Count >= 1)
            {
                var tcs = tcsQueue.Dequeue();
                Monitor.Exit(this);

                tcs.TrySetException(ex);
            }
            else
            {
                valueQueue.Enqueue(ex);
                Monitor.Exit(this);
            }
        }

        public ValueTaskAwaiter<T> GetAwaiter()
        {
            Monitor.Enter(this);
            if (valueQueue.Count >= 1)
            {
                var obj = valueQueue.Dequeue();
                Monitor.Exit(this);

                if (obj is Exception)
                {
                    var tcs = new TaskCompletionSource<T>();
                    tcs.SetException((Exception)obj);
                    return new ValueTask<T>(tcs.Task).GetAwaiter();
                }
                else
                {
                    return (new ValueTask<T>((T)obj!)).GetAwaiter();
                }
            }
            else
            {
                var tcs = new TaskCompletionSource<T>();
                tcsQueue.Enqueue(tcs);
                Monitor.Exit(this);
                return (new ValueTask<T>(tcs.Task)).GetAwaiter();
            }
        }
    }
}
