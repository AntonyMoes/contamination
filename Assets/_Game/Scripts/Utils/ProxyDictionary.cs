using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace _Game.Scripts.Utils {
    public class ProxyDictionary<TKey, TValue, TValueInner> : IReadOnlyDictionary<TKey, TValue> where TValueInner : TValue {
        private readonly IReadOnlyDictionary<TKey, TValueInner> _dictionary;

        public ProxyDictionary(IReadOnlyDictionary<TKey, TValueInner> dictionary) {
            _dictionary = dictionary;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            foreach (var pair in _dictionary) {
                yield return new KeyValuePair<TKey, TValue>(pair.Key, pair.Value);
            } 
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable) _dictionary).GetEnumerator();
        }

        public int Count => _dictionary.Count;

        public bool ContainsKey(TKey key) {
            return _dictionary.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value) {
            var result = _dictionary.TryGetValue(key, out var innerValue);
            value = innerValue;
            return result;
        }

        public TValue this[TKey key] => _dictionary[key];

        public IEnumerable<TKey> Keys => _dictionary.Keys;

        public IEnumerable<TValue> Values => _dictionary.Values.Select(value => (TValue) value);
    }
}