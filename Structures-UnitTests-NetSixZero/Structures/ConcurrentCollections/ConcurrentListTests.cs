using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Structures.NetSixZero.Structures.ConcurrentCollections;

namespace Structures_UnitTests_NetSixZero.Structures.ConcurrentCollections {
    public class ConcurrentListTests {
        [TestCase(10000)]
        [TestCase(10000)]
        [TestCase(10000)]
        public async Task AddElementsTest(int iterations) {
            var list = new ConcurrentList<int>(TimeSpan.FromMilliseconds(1000));

            var task1 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    list.Add(i);
                }
            });
        
            var task2 = Task.Factory.StartNew(() => {
                for (int i = iterations; i < iterations * 2; i++) {
                    list.Add(i);
                }
            });

            await Task.WhenAll(task1, task2);
            Assert.AreEqual(iterations * 2, list.Count);
        }

        [TestCase(10000)]
        [TestCase(10000)]
        [TestCase(10000)]
        public async Task AddRemoveElementsTest(int iterations) {
            var list = new ConcurrentList<int>(TimeSpan.FromMilliseconds(1000));

            for (int i = 0; i < iterations; i++) {
                list.Add(i);
            }
        
            Assert.AreEqual(iterations, list.Count);
        
            var task1 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    list.Remove(i);
                }
            });
        
            var task2 = Task.Factory.StartNew(() => {
                for (int i = iterations; i < iterations * 2; i++) {
                    list.Add(i);
                }
            });

            await Task.WhenAll(task1, task2);
            Assert.AreEqual(iterations, list.Count);
        }

        [TestCase(10000)]
        [TestCase(10000)]
        [TestCase(10000)]
        public async Task GetElementsTest(int iterations) {
            var list = new ConcurrentList<int>(TimeSpan.FromMilliseconds(1));

            for (int i = 0; i < iterations; i++) {
                list.Add(i);
            }
        
            Assert.AreEqual(iterations, list.Count);
            
            var task1 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    Assert.AreEqual(i, list[i]);
                }
            });
        
            var task2 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    Assert.AreEqual(i, list[i]);
                }
            });

            await Task.WhenAll(task1, task2);
        }
    }
}