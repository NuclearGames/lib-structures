using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Structures.NetSixZero.Structures.ConcurrentCollections;

namespace Structures_UnitTests_NetSixZero.Structures.ConcurrentCollections {
    public class ConcurrentHashSetTests {
        [Repeat(3)]
        [TestCase(10000)]
        public async Task AddElementsTest(int iterations) {
            var set = new ConcurrentHashSet<int>(TimeSpan.FromMilliseconds(1000));

            var task1 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    set.Add(i);
                }
            });
        
            var task2 = Task.Factory.StartNew(() => {
                for (int i = iterations; i < iterations * 2; i++) {
                    set.Add(i);
                }
            });

            await Task.WhenAll(task1, task2);
            Assert.AreEqual(iterations * 2, set.Count);
        }

        [Repeat(3)]
        [TestCase(10000)]
        public async Task AddRemoveElementsTest(int iterations) {
            var set = new ConcurrentHashSet<int>(TimeSpan.FromMilliseconds(1000));

            for (int i = 0; i < iterations; i++) {
                set.Add(i);
            }
        
            Assert.AreEqual(iterations, set.Count);
        
            var task1 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    set.Remove(i);
                }
            });
        
            var task2 = Task.Factory.StartNew(() => {
                for (int i = iterations; i < iterations * 2; i++) {
                    set.Add(i);
                }
            });

            await Task.WhenAll(task1, task2);
            Assert.AreEqual(iterations, set.Count);
        }
        
        [Repeat(3)]
        [TestCase(10000)]
        public async Task GetElementsTest(int iterations) {
            var set = new ConcurrentHashSet<int>(TimeSpan.FromMilliseconds(1));

            for (int i = 0; i < iterations; i++) {
                set.Add(i);
            }
        
            Assert.AreEqual(iterations, set.Count);
            
            var task1 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    Assert.IsTrue(set.Contains(i));
                }
            });
        
            var task2 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    Assert.IsTrue(set.Contains(i));
                }
            });

            await Task.WhenAll(task1, task2);
        }
    }
}