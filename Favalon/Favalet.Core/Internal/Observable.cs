using System.Diagnostics;

namespace System.Reactive
{
    internal static class Observable
    {
        [DebuggerStepThrough]
        private sealed class DelegatedObservable<T> :
            IObservable<T>
        {
            private Func<IObserver<T>, IDisposable> subscribe;

            public DelegatedObservable(Func<IObserver<T>, IDisposable> subscribe) =>
                this.subscribe = subscribe;

            public IDisposable Subscribe(IObserver<T> observer) =>
                this.subscribe(observer);
        }
        
        public static IObservable<T> Create<T>(Func<IObserver<T>, IDisposable> subscribe) =>
            new DelegatedObservable<T>(subscribe);
    }
}
