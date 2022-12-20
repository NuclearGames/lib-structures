using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NuclearGames.StructuresUnity.Utils.Collections {
    /// <summary>
    /// Адаптер для IReadOnlyDictionary словаря. Преобразует тип <typeparamref name="TRawValue"/> в его базовый <typeparamref name="TValue"/>.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TRawValue"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class ReadOnlyDictionaryAdapter<TKey, TValue, TRawValue> : IReadOnlyDictionary<TKey, TValue> where TRawValue : TValue {
        public TValue this[TKey key] => _source[key];
        public IEnumerable<TKey> Keys => _source.Keys;
        public IEnumerable<TValue> Values => _source.Values.Cast<TValue>();
        public int Count => _source.Count;

        private readonly IReadOnlyDictionary<TKey, TRawValue> _source;

        public ReadOnlyDictionaryAdapter(IReadOnlyDictionary<TKey, TRawValue> source) {
            _source = source;
        }

        public bool ContainsKey(TKey key) {
            return _source.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value) {
            if (_source.TryGetValue(key, out var rawValue)) {
                value = rawValue;
                return true;
            }
            value = default!;
            return false;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            return new Enumerator(_source);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return new Enumerator(_source);
        }

        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IEnumerator, IDisposable {
            private readonly IEnumerator<KeyValuePair<TKey, TRawValue>> _source;

            public Enumerator(IReadOnlyDictionary<TKey, TRawValue> source) : this() {
                _source = source.GetEnumerator();
            }

            public KeyValuePair<TKey, TValue> Current => new KeyValuePair<TKey, TValue>(_source.Current.Key, _source.Current.Value);
            object IEnumerator.Current => _source.Current;

            public bool MoveNext() {
                return _source.MoveNext();
            }

            public void Dispose() {
                _source.Dispose();
            }

            public void Reset() {
                _source.Reset();
            }
        }
    }
}
