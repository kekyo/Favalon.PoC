using System.Collections.Generic;
using System.Reflection;

namespace System.Reactive.Linq
{
#if NET35 || NET40
    internal static class Observable
    {
        private sealed class DelegatedObserver<T> :
            IObserver<T>
        {
            private Action<T> onNext;
            private Action<Exception> onError;

            public DelegatedObserver(Action<T> onNext, Action<Exception> onError)
            {
                this.onNext = onNext;
                this.onError = onError;
            }

            public void OnNext(T value) =>
                this.onNext(value);

            public void OnError(Exception error) =>
                this.onError(error);

            public void OnCompleted()
            {
            }
        }
        
        public static IEnumerable<T> ToEnumerable<T>(this IObservable<T> observable)
        {
            var list = new List<T>();
            Exception? ex = null;
            observable.Subscribe(new DelegatedObserver<T>(list.Add, e => ex = e));
            if (ex != null)
            {
                throw new TargetInvocationException(ex);
            }
            return list;
        }
    }
#endif
}
