////////////////////////////////////////////////////////////////////////////
//
// Favalon - An Interactive Shell Based on a Typed Lambda Calculus.
// Copyright (c) 2018-2020 Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//	http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Favalet.Internal
{
    internal struct ImmutableDictionary<TKey, TValue> :
        IReadOnlyDictionary<TKey, TValue>
    {
#if NET35 || NET40
        private sealed class InternalDictionary :
            Dictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
        {
            public InternalDictionary(IEqualityComparer<TKey> comparer) :
                base(comparer)
            { }

            public InternalDictionary(InternalDictionary from) :
                base(from, from.Comparer)
            { }

            IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values =>
                this.Values;
        }
#else
        private sealed class InternalDictionary :
            Dictionary<TKey, TValue>
        {
            public InternalDictionary(IEqualityComparer<TKey> comparer) :
                base(comparer)
            { }

            public InternalDictionary(InternalDictionary from) :
                base(from, from.Comparer)
            { }
        }
#endif
        private InternalDictionary inner;

        internal ImmutableDictionary(IEqualityComparer<TKey> comparer) =>
            this.inner = new InternalDictionary(comparer);

        private ImmutableDictionary(InternalDictionary inner) =>
            this.inner = inner;

        public TValue this[TKey key] =>
            this.inner[key];

        public IEnumerable<TKey> Keys =>
            this.inner.Keys;

        public IEnumerable<TValue> Values =>
            this.inner.Values;

        public int Count =>
            this.inner.Count;

        public bool ContainsKey(TKey key) =>
            this.inner.ContainsKey(key);

        public bool TryGetValue(TKey key, out TValue value) =>
            this.inner.TryGetValue(key, out value);

        public Dictionary<TKey, TValue>.Enumerator GetEnumerator() =>
            this.inner.GetEnumerator();

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() =>
            this.inner.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            this.inner.GetEnumerator();

        private InternalDictionary Clone() =>
            new InternalDictionary(this.inner);

        public ImmutableDictionary<TKey, TValue> Add(TKey key, TValue value)
        {
            var cloned = this.Clone();
            cloned.Add(key, value);
            return new ImmutableDictionary<TKey, TValue>(cloned);
        }

        public ImmutableDictionary<TKey, TValue> AddRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
        {
            var cloned = this.Clone();
            foreach (var pair in pairs)
            {
                cloned.Add(pair.Key, pair.Value);
            }
            return new ImmutableDictionary<TKey, TValue>(cloned);
        }

        public ImmutableDictionary<TKey, TValue> AddRange(params KeyValuePair<TKey, TValue>[] pairs) =>
            this.AddRange((IEnumerable<KeyValuePair<TKey, TValue>>)pairs);

        public ImmutableDictionary<TKey, TValue> Set(TKey key, TValue value)
        {
            var cloned = this.Clone();
            cloned[key] = value;
            return new ImmutableDictionary<TKey, TValue>(cloned);
        }

        public ImmutableDictionary<TKey, TValue> Remove(TKey key)
        {
            var cloned = this.Clone();
            cloned.Remove(key);
            return new ImmutableDictionary<TKey, TValue>(cloned);
        }

        public ImmutableDictionary<TKey, TValue> RemoveRange(IEnumerable<TKey> keys)
        {
            var cloned = this.Clone();
            foreach (var key in keys)
            {
                cloned.Remove(key);
            }
            return new ImmutableDictionary<TKey, TValue>(cloned);
        }

        public ImmutableDictionary<TKey, TValue> RemoveRange(params TKey[] keys) =>
            this.RemoveRange((IEnumerable<TKey>)keys);
    }

    internal static class ImmutableDictionary
    {
        public static ImmutableDictionary<TKey, TValue> Create<TKey, TValue>() =>
            new ImmutableDictionary<TKey, TValue>(EqualityComparer<TKey>.Default);

        public static ImmutableDictionary<TKey, TValue> Create<TKey, TValue>(
            IEqualityComparer<TKey> comparer) =>
            new ImmutableDictionary<TKey, TValue>(comparer);

        public static ImmutableDictionary<TKey, TValue> ToImmutableDictionary<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue>> pairs) =>
            new ImmutableDictionary<TKey, TValue>(EqualityComparer<TKey>.Default).AddRange(pairs);

        public static ImmutableDictionary<TKey, TValue> ToImmutableDictionary<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue>> pairs,
            IEqualityComparer<TKey> comparer) =>
            new ImmutableDictionary<TKey, TValue>(comparer).AddRange(pairs);

        public static ImmutableDictionary<TKey, TValue> ToImmutableDictionary<TElement, TKey, TValue>(
            this IEnumerable<TElement> enumerable,
            Func<TElement, TKey> keySelector,
            Func<TElement, TValue> valueSelector) =>
            new ImmutableDictionary<TKey, TValue>(EqualityComparer<TKey>.Default).AddRange(
                enumerable.Select(element =>
                    new KeyValuePair<TKey, TValue>(keySelector(element), valueSelector(element))));

        public static ImmutableDictionary<TKey, TValue> ToImmutableDictionary<TElement, TKey, TValue>(
            this IEnumerable<TElement> enumerable,
            Func<TElement, TKey> keySelector,
            Func<TElement, TValue> valueSelector,
            IEqualityComparer<TKey> comparer) =>
            new ImmutableDictionary<TKey, TValue>(comparer).AddRange(
                enumerable.Select(element =>
                    new KeyValuePair<TKey, TValue>(keySelector(element), valueSelector(element))));
    }
}
