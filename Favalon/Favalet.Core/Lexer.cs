using Favalet.Lexers;
using Favalet.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading;

namespace Favalet
{
    public interface ILexer
    {
        IObservable<Token> Analyze(IObservable<char> chars);
    }
    
    public sealed class Lexer : ILexer
    {
        [DebuggerStepThrough]
        private Lexer()
        {
        }

        private sealed class TokenObservableObserver :
            IObserver<char>, IObservable<Token>
        {
            private LexRunnerContext context =
                LexRunnerContext.Create();
            private LexRunner runner =
                WaitingIgnoreSpaceRunner.Instance;

            private volatile IObservable<char>? chars;
            private IObserver<Token>? observer;

            public TokenObservableObserver(IObservable<char> chars) =>
                this.chars = chars;

            public void OnNext(char inch)
            {
                switch (this.runner.Run(this.context, inch))
                {
                    case LexRunnerResult(LexRunner next, Token token0, Token token1):
                        this.observer?.OnNext(token0);
                        this.observer?.OnNext(token1);
                        this.runner = next;
                        break;
                    case LexRunnerResult(LexRunner next, Token token, _):
                        this.observer?.OnNext(token);
                        this.runner = next;
                        break;
                    case LexRunnerResult(LexRunner next, _, _):
                        this.runner = next;
                        break;
                }
            }

            public void OnCompleted()
            {
                if (this.runner.Finish(this.context) is LexRunnerResult(_, Token finalToken, _))
                {
                    this.observer?.OnNext(finalToken);
                }
                this.observer?.OnCompleted();
            }

            public void OnError(Exception error) =>
                this.observer?.OnError(error);

            [DebuggerStepThrough]
            private sealed class Disposer : IDisposable
            {
                private IDisposable? disposable;
                private TokenObservableObserver? parent;

                public Disposer(TokenObservableObserver parent, IDisposable disposable)
                {
                    this.parent = parent;
                    this.disposable = disposable;
                }

                public void Dispose()
                {
                    if (this.disposable != null)
                    {
                        this.disposable.Dispose();
                        this.disposable = null;
                        this.parent!.observer = null;
                        this.parent = null;
                    }
                }
            }
            
            public IDisposable Subscribe(IObserver<Token> observer)
            {
                var chars = Interlocked.Exchange(ref this.chars, null);
                if (chars == null)
                {
                    throw new InvalidOperationException();
                }

                Debug.Assert(this.observer == null);

                this.observer = observer;
                var disposable = chars.Subscribe(this);
                return new Disposer(this, disposable);
            }
        }

        [DebuggerStepThrough]
        public IObservable<Token> Analyze(IObservable<char> chars) =>
            Observable.Create<Token>(observer =>
            {
                var context = LexRunnerContext.Create();
                var runner = WaitingIgnoreSpaceRunner.Instance;

                return chars.Subscribe(Observer.Create<char>(
                    inch =>
                    {
                        switch (runner.Run(context, inch))
                        {
                            case LexRunnerResult(LexRunner next, Token token0, Token token1):
                                observer.OnNext(token0);
                                observer.OnNext(token1);
                                runner = next;
                                break;
                            case LexRunnerResult(LexRunner next, Token token, _):
                                observer.OnNext(token);
                                runner = next;
                                break;
                            case LexRunnerResult(LexRunner next, _, _):
                                runner = next;
                                break;
                        }
                    },
                    observer.OnError,
                    () =>
                    {
                        if (runner.Finish(context) is LexRunnerResult(_, Token finalToken, _))
                        {
                            observer.OnNext(finalToken);
                        }
                        observer.OnCompleted();
                    }));
            });
     
        [DebuggerStepThrough]
        public static Lexer Create() =>
            new Lexer();
    }

    [DebuggerStepThrough]
    public static class LexerExtension
    {
        public static IObservable<Token> Analyze(this ILexer lexer, IEnumerable<char> chars) =>
            lexer.Analyze(Observable.Create<char>(observer =>
            {
                foreach (var inch in chars)
                {
                    observer.OnNext(inch);
                }
                observer.OnCompleted();
                return Disposable.Empty;
            }));

        public static IObservable<Token> Analyze(this ILexer lexer, TextReader tr) =>
            lexer.Analyze(Observable.Create<char>(observer =>
            {
                while (true)
                {
                    var inch = tr.Read();
                    if (inch < 0)
                    {
                        break;
                    }
                    observer.OnNext((char)inch);
                }
                observer.OnCompleted();
                return Disposable.Empty;
            }));
    }
}
