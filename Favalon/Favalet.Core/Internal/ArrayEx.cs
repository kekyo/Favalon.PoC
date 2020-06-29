using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalet.Internal
{
    internal static class ArrayEx
    {
#if NETSTANDARD2_0
        public static T[] Empty<T>() =>
            Array.Empty<T>();
#else
        private static class EmptyHolder<T>
        {
            public static readonly T[] Empty = new T[0];
        }

        public static T[] Empty<T>() =>
            EmptyHolder<T>.Empty;
#endif
    }
}
