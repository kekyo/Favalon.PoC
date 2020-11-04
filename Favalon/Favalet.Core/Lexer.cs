using System;
using Favalet.Lexers;
using Favalet.Tokens;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Favalet
{
    public interface ILexer
    {
        IObservable<Token> ToTokens(IObservable<char> chars);
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
        public IObservable<Token> ToTokens(IObservable<char> chars) =>
            new TokenObservableObserver(chars);
     
        [DebuggerStepThrough]
        public static Lexer Create() =>
            new Lexer();
    }

    [DebuggerStepThrough]
    public static class LexerExtension
    {
        [DebuggerStepThrough]
        private sealed class Disposer : IDisposable
        {
            private Disposer()
            { }

            public void Dispose()
            { }
            
            public static readonly Disposer Instance =
                new Disposer();
        }
        
        private sealed class CharEnumerableObservable :
            IObservable<char>
        {
            private volatile IEnumerable<char>? chars;

            [DebuggerStepThrough]
            public CharEnumerableObservable(IEnumerable<char> chars) =>
                this.chars = chars;

            public IDisposable Subscribe(IObserver<char> observer)
            {
                var chars = Interlocked.Exchange(ref this.chars, null);
                if (chars == null)
                {
                    throw new InvalidOperationException();
                }

                foreach (var inch in chars)
                {
                    observer.OnNext(inch);
                }
                
                observer.OnCompleted();

                return Disposer.Instance;
            }
        }

        public static IObservable<Token> ToTokens(this ILexer lexer, IEnumerable<char> chars) =>
            lexer.ToTokens(new CharEnumerableObservable(chars));
         
        private sealed class TextReaderObservable :
            IObservable<char>
        {
            private volatile TextReader? tr;

            [DebuggerStepThrough]
            public TextReaderObservable(TextReader tr) =>
                this.tr = tr;

            public IDisposable Subscribe(IObserver<char> observer)
            {
                var tr = Interlocked.Exchange(ref this.tr, null);
                if (tr == null)
                {
                    throw new InvalidOperationException();
                }

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

                return Disposer.Instance;
            }
        }

        public static IObservable<Token> ToTokens(this ILexer lexer, TextReader tr) =>
            lexer.ToTokens(new TextReaderObservable(tr));
    }
}
