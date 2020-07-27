using System;
using System.Diagnostics;

namespace Favalet.Internal
{
    internal static class ArrayEx
    {
#if NETSTANDARD2_0
        [DebuggerStepThrough]
        public static T[] Empty<T>() =>
            Array.Empty<T>();
#else
        private static class EmptyHolder<T>
        {
            public static readonly T[] Empty = new T[0];
        }

        [DebuggerStepThrough]
        public static T[] Empty<T>() =>
            EmptyHolder<T>.Empty;
#endif
    }
}
