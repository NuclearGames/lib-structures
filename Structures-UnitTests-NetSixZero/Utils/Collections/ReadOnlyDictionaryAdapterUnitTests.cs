using NUnit.Framework;
using Structures.NetSixZero.Utils.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Structures_UnitTests_NetSixZero.Utils.Collections {
    internal class ReadOnlyDictionaryAdapterUnitTests {
        private interface ISomeType {
            int Value { get; set; }
        }

        private class SomeType : ISomeType {
            public int Value { get; set; }
            public SomeType(int value = 0) {
                Value = value;
            }
        }

        [Test]
        public void Main() {
            // Проверяет:
            // - Данные возвращаемые адаптером точно соответсвуют данным источника.

            var source = new Dictionary<string, SomeType>();
            IReadOnlyDictionary<string, ISomeType> adapter = new ReadOnlyDictionaryAdapter<string, ISomeType, SomeType>(source);

            // Сравнивает souce и adapter.
            void AssertState() {
                Assert.AreEqual(adapter.Count, source.Count);
                Assert.IsTrue(Enumerable.SequenceEqual(adapter.Keys, source.Keys));
                Assert.IsTrue(Enumerable.SequenceEqual(adapter.Values, source.Values));

                foreach (var key in source.Keys) {
                    Assert.AreEqual(adapter.ContainsKey(key), source.ContainsKey(key));
                    Assert.AreEqual(adapter[key], source[key]);
                    Assert.IsTrue(
                        adapter.TryGetValue(key, out var adapterValue) == source.TryGetValue(key, out var sourceValue) &&
                        adapterValue == sourceValue);
                }

                var adapterEnumeration = adapter.ToArray();
                var sourceEnumeration = source.ToArray();
                Assert.IsTrue(Enumerable.SequenceEqual(adapterEnumeration.Select(x => x.Key), sourceEnumeration.Select(x => x.Key)));
                Assert.IsTrue(Enumerable.SequenceEqual(adapterEnumeration.Select(x => x.Value), sourceEnumeration.Select(x => x.Value)));
            }

            // Run.
            AssertState();
            source.Add("abc", new SomeType(5));
            source.Add("bbb", new SomeType(2));
            AssertState();
            source.Remove("abc");
            AssertState();
            source.Add("qwe", new SomeType(2));
            source.Add("rey", new SomeType(2));
            AssertState();
            source.Clear();
            AssertState();
        }
    }
}
