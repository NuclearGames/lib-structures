using System;
using NUnit.Framework;
using Structures.NetStandard.Utils.Collections;


namespace Structures.UnitTests.NetStandard.Utils.Collections {
    public class AnyElementListUnitTests {
        private readonly int[] _dataCollection = new[] {
            9, 4, 17, 3, 6, 22, 5 ,7, 20
        };
        
        private readonly AnyElementList<int> _collection = new();

        [Test]
        public void SuccessTests() {
            _collection.AddRange(_dataCollection);
            
            Assert.AreEqual(false, _collection.IsEmpty);
            Assert.AreEqual(_dataCollection[0], _collection.Any);
            Assert.AreEqual(true, _collection.TryGetAny(out var value));
            Assert.AreEqual(_dataCollection[0], value);
        }
        
        [Test]
        public void FailTests() {
            Assert.AreEqual(true, _collection.IsEmpty);
            Assert.Catch<NullReferenceException>(() => Console.Write(_collection.Any));
            Assert.AreEqual(false, _collection.TryGetAny(out var value));
            Assert.AreEqual(default(int), value);
        }
    }
}