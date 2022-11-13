using System;
using NuclearGames.StructuresUnity.Structures.Collections;
using NUnit.Framework;

namespace Tests.Editor.Structures.Collections {
    public class BinaryHeapUnitTests {

        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public void ExtendCollectionTest(int size) {
            var heap = CreateDefaultHeap(size);
            var rnd = new Random();
            var lastMax = int.MinValue;
            for (int i = 0; i < size; i++) {
                var newValue = rnd.Next(0, size);
                heap.Push(newValue);
                Assert.AreEqual(i+1, heap.Count);

                if (newValue > lastMax) {
                    lastMax = newValue;
                    Assert.AreEqual(lastMax, heap.Peek());
                }
            }
            
            
            Console.WriteLine(string.Join("|", heap));
        }

        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public void PopCollectionTest(int size) {
            var heap = CreateDefaultHeap(size);
            Assert.Catch(() => heap.Pop());
            Assert.Catch(() => heap.Peek());
            
            var rnd = new Random();
            var lastMax = int.MinValue;
            for (int i = 0; i < size; i++) {
                var newValue = rnd.Next(0, size);
                heap.Push(newValue);
                
                if (newValue > lastMax) {
                    lastMax = newValue;
                }
            }
            
            Assert.AreEqual(lastMax,heap.Peek());
            Assert.AreEqual(lastMax,heap.Pop());
            Assert.LessOrEqual(heap.Pop(), lastMax);
        }

        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public void PushPopTest(int size) {
            var heap = CreateDefaultHeap();
            var rnd = new Random();

            var value = rnd.Next(0, size);
            Assert.AreEqual(value, heap.PushPop(value));
            
            for (int i = 0; i < size; i++) {
                var newValue = rnd.Next(0, size);
                heap.Push(newValue);
            }

            var maxValue = heap.Peek();
            Assert.AreEqual(maxValue, heap.PushPop(-1));

            value = size + 1;
            Assert.AreEqual(value, heap.PushPop(value));
        }

        [Test]
        public void ContainsTest() {
            var heap = new BinaryHeap<RefInt, int>((refObj) => refObj.Value);

            var obj = new RefInt { Value = 0 };
            heap.Push(obj);
            
            Assert.AreEqual(true, heap.Contains(obj));
            Assert.AreEqual(true, heap.Contains(obj.Value));

            var newObj = new RefInt { Value = 0 };
            Assert.AreEqual(false, heap.Contains(newObj));
            Assert.AreEqual(true, heap.Contains(newObj.Value));

            newObj = new RefInt { Value = 1 };
            
            Assert.AreEqual(false, heap.Contains(newObj));
            Assert.AreEqual(false, heap.Contains(newObj.Value));
        }

        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public void RemoveTest(int size) {
            var heap = new BinaryHeap<RefInt, int>((refObj) => refObj.Value, size);

            Assert.Catch(() => heap.Remove(new RefInt()));
            Assert.AreEqual(false, heap.TryRemoveByKey(-1, out _));
            
            var rnd = new Random();
            var lastMax = new RefInt { Value = int.MinValue };
            var lastMin = new RefInt { Value = int.MaxValue };
            for (int i = 0; i < size; i++) {
                var newObj = new RefInt { Value = rnd.Next(0, size) };
                heap.Push(newObj);
                
                if (newObj.Value > lastMax.Value) {
                    lastMax = newObj;
                }

                if (lastMin.Value > newObj.Value) {
                    lastMin = newObj;
                }
            }
            
            Assert.AreEqual(true, heap.TryRemoveByKey(lastMax.Value, out _));
            Assert.AreEqual(true, heap.TryRemoveByKey(lastMin.Value, out _));
        }

#region utils

        private class RefInt {
            internal int Value;
        }
        
        private static BinaryHeap<int, int> CreateDefaultHeap(int size = 0) => new BinaryHeap<int, int>((value) => value, size);

#endregion
    }
}