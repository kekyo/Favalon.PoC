////////////////////////////////////////////////////////////////////////////
//
// Favalon - An Interactive Shell Based on a Typed Lambda Calculus.
// Copyright (c) 2018-2020 Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//	http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace Favalet.Internal
{
    internal abstract class ObservableBase<TProduce> :
        IObservable<TProduce>, IDisposable
    {
        // Cold purpose.

        private IObserver<TProduce>? observer;

        protected ObservableBase()
        { }

        public virtual void Dispose() =>
            this.observer = null;

        public virtual IDisposable Subscribe(IObserver<TProduce> observer)
        {
            if (this.observer != null)
            {
                throw new InvalidOperationException();
            }

            this.observer = observer;
            return new Disposer(this);
        }

        protected void SendValues(params TProduce[] values)
        {
            if (this.observer is IObserver<TProduce> observer)
            {
                foreach (var value in values)
                {
                    observer.OnNext(value);
                }
            }
        }

        protected void SendAndFinish(TProduce value)
        {
            if (this.observer is IObserver<TProduce> observer)
            {
                observer.OnNext(value);
                observer.OnCompleted();

                this.observer = null;
            }
        }

        protected void Finish()
        {
            if (this.observer is IObserver<TProduce> observer)
            {
                observer.OnCompleted();

                this.observer = null;
            }
        }
        protected void Finish(Exception error)
        {
            if (this.observer is IObserver<TProduce> observer)
            {
                observer.OnError(error);

                this.observer = null;
            }
        }

        private sealed class Disposer : IDisposable
        {
            private ObservableBase<TProduce>? parent;

            public Disposer(ObservableBase<TProduce> parent) =>
                this.parent = parent;

            public void Dispose()
            {
                if (this.parent is ObservableBase<TProduce> parent)
                {
                    parent.Dispose();
                    this.parent = null;
                }
            }
        }
    }
}
