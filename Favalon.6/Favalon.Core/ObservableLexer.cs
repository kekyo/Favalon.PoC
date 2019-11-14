using System;
using System.Collections.Generic;
using Favalon.Internals;
using Favalon.Tokens;

namespace Favalon
{
    public sealed class ObservableLexer :
        IObserver<char>, IObservable<Token>
    {
        private static readonly IObserver<Token>[] emptyObservers = new IObserver<Token>[0];

        private readonly LexerCore lexerCore = new LexerCore();

        private List<IObserver<Token>>? observers;
        private IObserver<Token>[]? fixedObservers;

        private IObserver<Token>[] GetObservers()
        {
            if (observers == null)
            {
                return emptyObservers;
            }

            var fixedObservers = this.fixedObservers;
            if (fixedObservers == null)
            {
                lock (this)
                {
                    fixedObservers = this.fixedObservers;
                    if (fixedObservers == null)
                    {
                        fixedObservers = observers.ToArray();
                        this.fixedObservers = fixedObservers;
                    }
                }
            }

            return fixedObservers;
        }

        void IObserver<char>.OnNext(char inch)
        {
            if (lexerCore.Examine(inch) is Token token)
            {
                foreach (var observer in this.GetObservers())
                {
                    observer.OnNext(token);
                }
            }
        }

        void IObserver<char>.OnError(Exception ex)
        {
            foreach (var observer in this.GetObservers())
            {
                observer.OnError(ex);
            }
        }

        void IObserver<char>.OnCompleted()
        {
            if (lexerCore.Flush() is Token token)
            {
                foreach (var observer in this.GetObservers())
                {
                    observer.OnNext(token);
                    observer.OnCompleted();
                }
            }
            else
            {
                foreach (var observer in this.GetObservers())
                {
                    observer.OnCompleted();
                }
            }
        }

        public IDisposable Subscribe(IObserver<Token> observer)
        {
            lock (this)
            {
                if (observers == null)
                {
                    observers = new List<IObserver<Token>>();
                }
                observers.Add(observer);
                fixedObservers = null;

                return new Disposer(this, observer);
            }
        }

        private sealed class Disposer : IDisposable
        {
            private readonly ObservableLexer lexer;
            private readonly IObserver<Token> observer;

            public Disposer(ObservableLexer lexer, IObserver<Token> observer)
            {
                this.lexer = lexer;
                this.observer = observer;
            }

            public void Dispose()
            {
                lock (lexer)
                {
                    lexer.observers!.Remove(observer);
                    lexer.fixedObservers = null;
                }
            }
        }
    }
}
