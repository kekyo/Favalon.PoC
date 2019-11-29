using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Favalon.Internal
{
    internal sealed class ManagedDictionary<TKey, TValue>
    {
        private readonly ManagedDictionary<TKey, TValue>? parent;
#if DEBUG
        private SortedDictionary<TKey, TValue>? dictionary;
#else
        private Dictionary<TKey, TValue>? dictionary;
#endif

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public ManagedDictionary()
        { }

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private ManagedDictionary(ManagedDictionary<TKey, TValue>? parent) =>
            this.parent = parent;

        public TValue this[TKey key]
        {
#if NET45 || NETSTANDARD1_0
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            get
            {
                ManagedDictionary<TKey, TValue>? current = this;
                do
                {
#if DEBUG
                    if (current.dictionary is SortedDictionary<TKey, TValue> d &&
                        d.TryGetValue(key, out var v))
#else
                    if (current.dictionary is Dictionary<TKey, TValue> d &&
                        d.TryGetValue(key, out var v))
#endif
                    {
                        return v;
                    }
                    current = current.parent;
                }
                while (current != null);
                throw new KeyNotFoundException();
            }
#if NET45 || NETSTANDARD1_0
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            set
            {
                if (dictionary == null)
                {
#if DEBUG
                    dictionary = new SortedDictionary<TKey, TValue>();
#else
                    dictionary = new Dictionary<TKey, TValue>();
#endif
                }
                dictionary[key] = value;
            }
        }

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private IEnumerable<KeyValuePair<TKey, TValue>> GetNormalized()
        {
            ManagedDictionary<TKey, TValue>? current = this;
            var keys = new HashSet<TKey>();
            do
            {
                if (current.dictionary is Dictionary<TKey, TValue> d)
                {
                    foreach (var entry in d)
                    {
                        if (keys.Add(entry.Key))
                        {
                            yield return entry;
                        }
                    }
                }
                current = current.parent;
            }
            while (current != null);
        }

        public IEnumerable<TKey> Keys =>
            this.GetNormalized().Select(entry => entry.Key);

        public IEnumerable<TValue> Values =>
            this.GetNormalized().Select(entry => entry.Value);

        public int Count =>
            this.GetNormalized().Count();

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public bool ContainsKey(TKey key)
        {
            {
                ManagedDictionary<TKey, TValue>? current = this;
                do
                {
#if DEBUG
                    if (current.dictionary is SortedDictionary<TKey, TValue> d &&
                        d.ContainsKey(key))
#else
                    if (current.dictionary is Dictionary<TKey, TValue> d &&
                        d.ContainsKey(key))
#endif
                    {
                        return true;
                    }
                    current = current.parent;
                }
                while (current != null);
                return false;
            }
        }

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public bool TryGetValue(TKey key, out TValue value)
        {
            ManagedDictionary<TKey, TValue>? current = this;
            do
            {
#if DEBUG
                if (current.dictionary is SortedDictionary<TKey, TValue> d &&
                    d.TryGetValue(key, out value))
#else
                if (current.dictionary is Dictionary<TKey, TValue> d &&
                    d.TryGetValue(key, out value))
#endif
                {
                    return true;
                }
                current = current.parent;
            }
            while (current != null);
            value = default!;
            return false;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() =>
            this.GetNormalized().GetEnumerator();

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public ManagedDictionary<TKey, TValue> Clone() =>
            new ManagedDictionary<TKey, TValue>(this);
    }
}
