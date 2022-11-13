using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Structures.NetSixZero.Structures.BST;
using Structures.NetSixZero.Utils.Generators;

namespace Structures_UnitTests_NetSixZero.Utils.Generators {
    public abstract class BaseIdGeneratorsUnitTests<TGenerator, T> where TGenerator : BaseIdGenerator<T> where T : IComparable<T> {

        [TestCase(10)]
        public void OkTests(int size) {
            var uids = new List<T>();
            var generator = Constructor(size);

            var buffer = GetBuffer(generator);
            Assert.AreEqual(0, buffer.Count);

            for (int index = 0; index < size; index++) {
                uids.Add(generator.NewId());
                Assert.AreEqual(size - index - 1, buffer.Count);
            }

            for (int index = 0; index < size; index++) {
                uids.Add(generator.NewId());
                Assert.AreEqual(size - index - 1, buffer.Count);
            }

            Assert.AreEqual(size * 2, uids.Count);

            while (uids.Count > 0) {
                generator.Release(uids[0]);
                uids.RemoveAt(0);
            }
            
            Assert.AreEqual(size*2, buffer.Count);
        }

        [TestCase(1)]
        public void FailInitializeTests(int size) {
            var generator = Constructor(size);

            generator.Release(default(T));
            Assert.Catch(() => generator.NewId());
        }

#region Abstracts

        private protected abstract TGenerator Constructor(int size);

#endregion

#region Utils

        private BinaryTree<T> GetBuffer(TGenerator generator) {
            return (BinaryTree<T>)typeof(TGenerator)
                                  .GetField("Buffer", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField)
                                  .GetValue(generator);
        }

#endregion
    }
}