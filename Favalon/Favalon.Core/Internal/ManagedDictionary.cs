using System;
using System.Collections.Generic;
using System.Linq;

namespace Favalon.Internal
{
    internal sealed class ManagedDictionary<TKey, TValue>
    {
        private readonly ManagedDictionary<TKey, TValue>? parent;
        private Dictionary<TKey, TValue>? dictionary;

        public ManagedDictionary()
        { }

        private ManagedDictionary(ManagedDictionary<TKey, TValue>? parent) =>
            this.parent = parent;

        public TValue this[TKey key]
        {
            get
            {
                ManagedDictionary<TKey, TValue>? current = this;
                do
                {
                    if (current.dictionary is Dictionary<TKey, TValue> d &&
                        d.TryGetValue(key, out var v))
                    {
                        return v;
                    }
                    current = current.parent;
                }
                while (current != null);
                throw new KeyNotFoundException();
            }
            set
            {
                if (dictionary == null)
                {
                    dictionary = new Dictionary<TKey, TValue>();
                }
                dictionary[key] = value;
            }
        }

        private IEnumerable<KeyValuePair<TKey, TValue>> Normalized
        {
            get
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
        }

        public IEnumerable<TKey> Keys =>
            this.Normalized.Select(entry => entry.Key);

        public IEnumerable<TValue> Values =>
            this.Normalized.Select(entry => entry.Value);

        public int Count =>
            this.Normalized.Count();

        public bool ContainsKey(TKey key)
        {
            {
                ManagedDictionary<TKey, TValue>? current = this;
                do
                {
                    if (current.dictionary is Dictionary<TKey, TValue> d &&
                        d.ContainsKey(key))
                    {
                        return true;
                    }
                    current = current.parent;
                }
                while (current != null);
                return false;
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            ManagedDictionary<TKey, TValue>? current = this;
            do
            {
                if (current.dictionary is Dictionary<TKey, TValue> d &&
                    d.TryGetValue(key, out value))
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
            this.Normalized.GetEnumerator();

        public ManagedDictionary<TKey, TValue> Clone() =>
            new ManagedDictionary<TKey, TValue>(this);
    }
}
