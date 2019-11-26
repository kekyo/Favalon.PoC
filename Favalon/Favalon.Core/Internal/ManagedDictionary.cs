using System.Collections.Generic;

namespace Favalon.Internal
{
    internal sealed class ManagedDictionary<TKey, TValue>
    {
        private Dictionary<TKey, TValue> dictionary;
        private bool cloned;

        public ManagedDictionary()
        {
            this.dictionary = new Dictionary<TKey, TValue>();
            this.cloned = true;
        }

        private ManagedDictionary(Dictionary<TKey, TValue> dictionary) =>
            this.dictionary = dictionary;

        public ManagedDictionary<TKey, TValue> Fork() =>
            // TODO: performance issue for cloning, will change linked list impls.
            new ManagedDictionary<TKey, TValue>(dictionary);

        public TValue this[TKey key] =>
            dictionary[key];

        public IEnumerable<TKey> Keys =>
            dictionary.Keys;

        public IEnumerable<TValue> Values =>
            dictionary.Values;

        public int Count =>
            dictionary.Count;

        public bool ContainsKey(TKey key) =>
            dictionary.ContainsKey(key);

        public bool TryGetValue(TKey key, out TValue value) =>
            dictionary.TryGetValue(key, out value);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() =>
            dictionary.GetEnumerator();

        public void Set(TKey key, TValue value)
        {
            if (!cloned)
            {
                dictionary = new Dictionary<TKey, TValue>(dictionary);
                cloned = true;
            }

            dictionary[key] = value;
        }
    }
}
