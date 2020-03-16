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

using Favalet.LexRunners;
using Favalet.Tokens;
using System;
using System.Collections.Generic;

namespace Favalet
{
#if !NET35
    internal sealed class ObservableLexer : IObserver<char>, IObservable<Token>
    {
        private IDisposable? parentDisposer;
        private readonly HashSet<IObserver<Token>> observers = new HashSet<IObserver<Token>>();

        private readonly LexRunnerContext context = LexRunnerContext.Create();
        private LexRunner runner = WaitingIgnoreSpaceRunner.Instance;

        public ObservableLexer(IObservable<char> parent) =>
            this.parentDisposer = parent.Subscribe(this);

        public IDisposable Subscribe(IObserver<Token> observer)
        {
            lock (observers)
            {
                observers.Add(observer);
                return new Disposer(this, observer);
            }
        }

        public void OnNext(char inch)
        {
            lock (this)
            {
                if (this.parentDisposer != null)
                {
                    switch (runner.Run(context, inch))
                    {
                        case LexRunnerResult(LexRunner next, Token token0, Token token1):
                            lock (observers)
                            {
                                foreach (var observer in observers)
                                {
                                    observer.OnNext(token0);
                                    observer.OnNext(token1);
                                }
                            }
                            runner = next;
                            break;
                        case LexRunnerResult(LexRunner next, Token token, _):
                            lock (observers)
                            {
                                foreach (var observer in observers)
                                {
                                    observer.OnNext(token);
                                }
                            }
                            runner = next;
                            break;
                        case LexRunnerResult(LexRunner next, _, _):
                            runner = next;
                            break;
                    }
                }
            }
        }

        public void OnCompleted()
        {
            lock (this)
            {
                if (this.parentDisposer is IDisposable parentDisposer)
                {
                    if (runner.Finish(context) is LexRunnerResult(_, Token finalToken, _))
                    {
                        lock (observers)
                        {
                            foreach (var observer in observers)
                            {
                                observer.OnNext(finalToken);
                                observer.OnCompleted();
                            }

                            observers.Clear();
                        }
                    }
                    else
                    {
                        lock (observers)
                        {
                            foreach (var observer in observers)
                            {
                                observer.OnCompleted();
                            }

                            observers.Clear();
                        }
                    }

                    parentDisposer.Dispose();
                    this.parentDisposer = null;
                }
            }
        }

        public void OnError(Exception error)
        {
            lock (this)
            {
                if (this.parentDisposer != null)
                {
                    lock (observers)
                    {
                        foreach (var observer in observers)
                        {
                            observer.OnError(error);
                        }
                    }
                }
            }
        }

        private sealed class Disposer : IDisposable
        {
            private ObservableLexer? parent;
            private IObserver<Token>? observer;

            public Disposer(ObservableLexer parent, IObserver<Token> observer)
            {
                this.parent = parent;
                this.observer = observer;
            }

            public void Dispose()
            {
                if (this.parent is ObservableLexer parent &&
                    this.observer is IObserver<Token> observer)
                {
                    lock (parent.observers)
                    {
                        parent.observers.Remove(observer);
                    }

                    this.parent = null;
                    this.observer = null;
                }
            }
        }
    }
#endif
}
