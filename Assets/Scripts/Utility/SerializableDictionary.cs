using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Utility
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TKey : IEquatable<TKey>
    {
        [SerializeField] private List<SerializablePair<TKey, TValue>> keyValuePairs = new();

        [SerializeField] private bool isReadOnly;

        public void Add(TKey key, TValue value)
        {
            if (!ContainsKey(key))
            {
                keyValuePairs.Add(new SerializablePair<TKey, TValue> { key = key, value = value });
            }
        }

        public bool ContainsKey(TKey key)
        {
            foreach (var pair in keyValuePairs)
            {
                if (pair.key.Equals(key))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Remove(TKey key)
        {
            for (var i = 0; i < keyValuePairs.Count; ++i)
            {
                if (!keyValuePairs[i].key.Equals(key))
                {
                    continue;
                }

                keyValuePairs.RemoveAt(i);
                return true;
            }

            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (ContainsKey(key))
            {
                value = this[key];
                return true;
            }

            value = default;
            return false;
        }

        public TValue this[TKey key]
        {
            get
            {
                foreach (var pair in keyValuePairs)
                {
                    if (pair.key.Equals(key))
                    {
                        return pair.value;
                    }
                }

                throw new Exception($"Key \"{key}\" not found in serializable dictionary");
            }
            set
            {
                foreach (var pair in keyValuePairs)
                {
                    if (!pair.key.Equals(key))
                    {
                        continue;
                    }

                    pair.value = value;
                    return;
                }

                keyValuePairs.Add(new SerializablePair<TKey, TValue> { key = key, value = value });
            }
        }

        public ICollection<TKey> Keys => keyValuePairs.Select(pair => pair.key).ToArray();

        public ICollection<TValue> Values => keyValuePairs.Select(pair => pair.value).ToArray();

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return keyValuePairs.Select(pair => new KeyValuePair<TKey, TValue>(pair.key, pair.value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            keyValuePairs.Add(new SerializablePair<TKey, TValue> { key = item.Key, value = item.Value });
        }

        public void Clear()
        {
            keyValuePairs.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return keyValuePairs.Select(pair => new KeyValuePair<TKey, TValue>(pair.key, pair.value)).Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            array = new KeyValuePair<TKey, TValue>[keyValuePairs.Count];
            for (var i = 0; i < keyValuePairs.Count; ++i)
            {
                array[i] = new KeyValuePair<TKey, TValue>(keyValuePairs[i].key, keyValuePairs[i].value);
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return keyValuePairs.Remove(new SerializablePair<TKey, TValue> { key = item.Key, value = item.Value });
        }

        public int Count => keyValuePairs.Count;

        public bool IsReadOnly => isReadOnly;
    }
}
