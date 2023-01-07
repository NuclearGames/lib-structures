using System;
using System.Collections.Generic;
using System.Linq;
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

        [Test]
        public async Task TryGetValueTest([Values(10, 100, 1000)]int iterations) {
            var set = new ConcurrentHashSet<int>(TimeSpan.FromMilliseconds(1000));

            for (int i = 0; i < iterations; i++) {
                set.Add(i);
            }
        
            Assert.AreEqual(iterations, set.Count);
        
            var task1 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    Assert.IsTrue(set.TryGetValue(i, out var value));
                }
            });
        
            var task2 = Task.Factory.StartNew(() => {
                for (int i = iterations; i < iterations * 2; i++) {
                    set.Add(i);
                }
            });

            await Task.WhenAll(task1, task2);
        }

        [Test]
        public async Task RemoveWhereTest([Values(10, 100, 1000)]int iterations) {
            var set = new ConcurrentHashSet<int>(TimeSpan.FromMilliseconds(1000));
            var array = new int[iterations];
            
            for (int i = 0; i < iterations; i++) {
                set.Add(i);
                array[i] = i + iterations;
            }
        
            Assert.AreEqual(iterations, set.Count);
        
            var task1 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    set.RemoveWhere(item => item == i);
                }
            });
        
            var task2 = Task.Factory.StartNew(() => {
                for (int i = iterations; i < iterations * 2; i++) {
                    set.Add(i);
                }
            });

            await Task.WhenAll(task1, task2);
            
            foreach (var item in array) {
                Assert.IsTrue(set.Contains(item));
            }
        }

        [Test]
        public async Task CopyToTest([Values(10, 100, 1000)]int iterations) {
            var set = new ConcurrentHashSet<int>(TimeSpan.FromMilliseconds(10));
            
            for (int i = 0; i < 10; i++) {
                set.Add(i);
            }
            
            var task1 = Task.Factory.StartNew(() => {
                int[] array = new int[set.Count];
                
                for (int i = 0; i < iterations; i++) {
                    set.CopyTo(array);
                    Assert.AreEqual(set.ToArray(), array);
                    
                    set.CopyTo(array, 0);
                    Assert.AreEqual(set.ToArray(), array);
                    
                    set.CopyTo(array, 0, set.Count());
                    Assert.AreEqual(set.ToArray(), array);
                }
            });
        
            var task2 = Task.Factory.StartNew(() => {
                int[] array = new int[set.Count];
                
                for (int i = 0; i < iterations; i++) {
                    set.CopyTo(array);
                    Assert.AreEqual(set.ToArray(), array);
                    
                    set.CopyTo(array, 0);
                    Assert.AreEqual(set.ToArray(), array);
                    
                    set.CopyTo(array, 0, set.Count());
                    Assert.AreEqual(set.ToArray(), array);
                }
            });

            await Task.WhenAll(task1, task2);
        }

        [Test, Repeat(500)]
        public async Task IntersectWithTest() {
            var set = new ConcurrentHashSet<int>(TimeSpan.FromMilliseconds(10)) {
                0, 1, 2, 3, 4, 5, 6, 7, 8, 9
            };

            var anotherSet = new ConcurrentHashSet<int>(TimeSpan.FromMilliseconds(10)) {
                5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15
            };

            var intersectedSet = new ConcurrentHashSet<int>(TimeSpan.FromMilliseconds(10)) {
                5, 6, 7, 8, 9
            };

            var task1 = Task.Factory.StartNew(() => {
                set.IntersectWith(anotherSet);
                Assert.AreEqual(intersectedSet, set);
            });
        
            var task2 = Task.Factory.StartNew(() => {
                set.IntersectWith(anotherSet);
                Assert.AreEqual(intersectedSet, set);
            });

            await Task.WhenAll(task1, task2);
        }
        
        [Test, Repeat(500)]
        public async Task ExceptWithTest() {
            var set = new ConcurrentHashSet<int>(-1) {
                0, 1, 2, 3, 4, 5, 6, 7, 8, 9
            };

            var anotherSet = new ConcurrentHashSet<int>(-1) {
                5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15
            };

            var exceptedSet = new HashSet<int> {
                0, 1, 2, 3, 4
            };

            var task1 = Task.Factory.StartNew(() => {
                set.ExceptWith(anotherSet);
                Assert.AreEqual(exceptedSet, set);
            });
        
            var task2 = Task.Factory.StartNew(() => {
                set.ExceptWith(anotherSet);
                Assert.AreEqual(exceptedSet, set);
            });

            await Task.WhenAll(task1, task2);
        }

        [Test, Repeat(500)]
        public async Task SymmetricExceptWithTest() {
            var set = new ConcurrentHashSet<int>(-1) {
                0, 1, 2, 3, 4, 5, 6, 7, 8, 9
            };

            var anotherSet = new ConcurrentHashSet<int>(-1) {
                5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15
            };

            var task1 = Task.Factory.StartNew(() => {
                set.SymmetricExceptWith(anotherSet);
            });
        
            var task2 = Task.Factory.StartNew(() => {
                for (int i = 16; i < 50; i++) {
                    set.Add(i);
                }
            });

            await Task.WhenAll(task1, task2);

            for (int i = 5; i < 9; i++) {
                Assert.IsFalse(set.Contains(i));
            }
        }
        
        [Test, Repeat(500)]
        public async Task UnionWithTest() {
            var set = new ConcurrentHashSet<int>(TimeSpan.FromMilliseconds(10)) {
                0, 1, 2, 3, 4, 5, 6, 7, 8, 9
            };

            var anotherSet = new ConcurrentHashSet<int>(TimeSpan.FromMilliseconds(10)) {
                5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15
            };
            
            var unitedSet = new HashSet<int> { 
                0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15
            };

            var task1 = Task.Factory.StartNew(() => {
                set.UnionWith(anotherSet);
                Assert.AreEqual(unitedSet, set);
            });
        
            var task2 = Task.Factory.StartNew(() => {
                set.UnionWith(anotherSet);
                Assert.AreEqual(unitedSet, set);
            });

            await Task.WhenAll(task1, task2);
        }

        [Test, Repeat(500)]
        public async Task IsSuperSubSetOfTest() {
            var set = new ConcurrentHashSet<int>(TimeSpan.FromMilliseconds(10)) {
                0, 1, 2, 3, 4
            };

            var anotherSet = new ConcurrentHashSet<int>(TimeSpan.FromMilliseconds(10)) {
                0, 1, 2, 3, 4, 5, 6, 7, 8, 9
            };
            
            var task1 = Task.Factory.StartNew(() => {
                Assert.IsTrue(set.IsSubsetOf(anotherSet));
                Assert.IsFalse(set.IsSupersetOf(anotherSet));
                Assert.IsTrue(set.IsProperSubsetOf(anotherSet));
                Assert.IsFalse(set.IsProperSupersetOf(anotherSet));
            });
        
            var task2 = Task.Factory.StartNew(() => {
                Assert.IsTrue(set.IsSubsetOf(anotherSet));
                Assert.IsFalse(set.IsSupersetOf(anotherSet));
                Assert.IsTrue(set.IsProperSubsetOf(anotherSet));
                Assert.IsFalse(set.IsProperSupersetOf(anotherSet));
            });

            await Task.WhenAll(task1, task2);
        }

        [Test, Repeat(500)]
        public async Task OverlapsTest() {
            var set = new ConcurrentHashSet<int>(TimeSpan.FromMilliseconds(10)) {
                0, 1, 2, 3, 4
            };

            var anotherSet = new ConcurrentHashSet<int>(TimeSpan.FromMilliseconds(10)) {
                0, 1, 2, 3, 4, 5, 6, 7, 8, 9
            };
            
            var task1 = Task.Factory.StartNew(() => {
                Assert.IsTrue(set.Overlaps(anotherSet));
            });
        
            var task2 = Task.Factory.StartNew(() => {
                Assert.IsTrue(set.Overlaps(anotherSet));
            });

            await Task.WhenAll(task1, task2);
        }
        
        [Test, Repeat(500)]
        public async Task SetEqualsTest() {
            var set = new ConcurrentHashSet<int>(TimeSpan.FromMilliseconds(10)) {
                0, 1, 2, 3, 4
            };

            var anotherSet = new ConcurrentHashSet<int>(TimeSpan.FromMilliseconds(10)) {
                0, 1, 2, 3, 4
            };
            
            var task1 = Task.Factory.StartNew(() => {
                Assert.IsTrue(set.SetEquals(anotherSet));
            });
        
            var task2 = Task.Factory.StartNew(() => {
                Assert.IsTrue(set.SetEquals(anotherSet));
            });

            await Task.WhenAll(task1, task2);
        }
    }
}