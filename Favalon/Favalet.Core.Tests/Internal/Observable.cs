using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Reactive.Linq
{
    internal static class Observable
    {
        [DebuggerStepThrough]
        public static IEnumerable<T> ToEnumerable<T>(this IObservable<T> observable)
        {
            var list = new List<T>();
#if NET35 || NET40
            Exception? ex = null;
            observable.Subscribe(Observer.Create<T>(
                list.Add,
                e => ex = e,
                () => { }));
            if (ex != null)
            {
                throw new System.Reflection.TargetInvocationException(ex);
            }
#else
            System.Runtime.ExceptionServices.ExceptionDispatchInfo? edi = null;
            observable.Subscribe(Observer.Create<T>(
                list.Add,
                e => edi = System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(e),
                () => { }));
            edi?.Throw();
#endif
            return list;
        }
    }
}
