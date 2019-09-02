using System;
using System.Collections.Generic;

using Favalon.Parsing;

namespace Favalon
{
    public sealed class ObservableParser :
        IObserver<char>, IObservable<Token>
    {
        private static readonly IObserver<Token>[] emptyObservers = new IObserver<Token>[0];

        private readonly ParserCore parser = new ParserCore();

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
            if (parser.Run(inch) is Token token)
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
            if (parser.Flush() is Token token)
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
            private readonly ObservableParser parser;
            private readonly IObserver<Token> observer;

            public Disposer(ObservableParser parser, IObserver<Token> observer)
            {
                this.parser = parser;
                this.observer = observer;
            }

            public void Dispose()
            {
                lock (parser)
                {
                    parser.observers!.Remove(observer);
                    parser.fixedObservers = null;
                }
            }
        }
    }
}
