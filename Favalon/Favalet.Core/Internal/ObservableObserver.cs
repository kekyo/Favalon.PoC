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
    internal abstract class ObservableObserver<TProduce, TConsume> :
        ObservableBase<TProduce>, IObserver<TConsume>, IDisposable
    {
        private IObservable<TConsume> parent;
        private IDisposable? parentDisposable;

        protected ObservableObserver(IObservable<TConsume> parent) =>
            this.parent = parent;

        public override void Dispose()
        {
            lock (this)
            {
                if (this.parentDisposable is IDisposable parentDisposable)
                {
                    this.parentDisposable = null;
                    parentDisposable.Dispose();
                }

                base.Dispose();
            }
        }

        public override IDisposable Subscribe(IObserver<TProduce> observer)
        {
            var disposable = base.Subscribe(observer);

            lock (this)
            {
                if (this.parentDisposable == null)
                {
                    this.parentDisposable = this.parent.Subscribe(this);
                }
            }

            return disposable;
        }

        protected abstract void OnValueReceived(TConsume value);

        protected abstract void OnFinalize();

        public void OnNext(TConsume value)
        {
            lock (this)
            {
                this.OnValueReceived(value);
            }
        }

        public void OnCompleted()
        {
            lock (this)
            {
                this.OnFinalize();
            }
        }

        public void OnError(Exception error)
        {
            lock (this)
            {
                this.Finish(error);
            }
        }
    }
}
