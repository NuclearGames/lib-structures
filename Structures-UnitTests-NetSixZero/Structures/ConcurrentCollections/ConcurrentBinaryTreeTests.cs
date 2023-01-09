using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Structures.NetSixZero.Extensions;
using Structures.NetSixZero.Structures.BST.Utils;
using Structures.NetSixZero.Structures.ConcurrentCollections;

namespace Structures_UnitTests_NetSixZero.Structures.ConcurrentCollections {
    public class ConcurrentBinaryTreeTests {
        
        [TestCase(1000)]
        public async Task AddIntersectedElementsTest(int iterations) {
            var tree = new ConcurrentBinaryTree<ConcurrentData>(TimeSpan.FromMilliseconds(1000));
            var intersections = 0;

            void TaskAction(Markers marker) {
                for (int i = 0; i < iterations; i++) {
                    if (!tree!.TryAdd(new ConcurrentData(i, marker), out _)) {
                        Interlocked.Increment(ref intersections);
                    }
                }
            }
            
            var task1 = Task.Factory.StartNew(() => TaskAction(Markers.Task1));
            var task2 = Task.Factory.StartNew(() => TaskAction(Markers.Task2));
            
            await Task.WhenAll(task1, task2);
            
            Assert.AreEqual(iterations, tree.Count);
            Assert.AreEqual(iterations, intersections);
        }
        
        [TestCase(1000)]
        public async Task AddNonIntersectedElementsTest(int iterations) {
            var tree = new ConcurrentBinaryTree<ConcurrentData>(TimeSpan.FromMilliseconds(1000));

            var task1 = Task.Run(() => {
                for (int i = 0; i < iterations; i++) {
                    tree.Add(new ConcurrentData(i, Markers.Task1));
                }
            });
        
            var task2 = Task.Run(() => {
                for (int i = iterations; i < iterations * 2; i++) {
                    tree.Add(new ConcurrentData(i, Markers.Task2));
                }
            });

            await Task.WhenAll(task1, task2);
            Assert.AreEqual(iterations * 2, tree.Count);
        }
        
        [TestCase(1000)]
        [TestCase(100000)]
        public async Task AddRangeIntersectedElementsTest(int iterations) {
            var tree = new ConcurrentBinaryTree<ConcurrentData>(TimeSpan.FromMilliseconds(1000));
            
            void TaskAction(Markers marker) {
                var array = Enumerable.Range(0, iterations)
                                      .Select(i => new ConcurrentData(i, marker))
                                      .ToArray();
                array.Shuffle();
                tree.TryAddRange(array);
            }
            
            var task1 = Task.Factory.StartNew(() => TaskAction(Markers.Task1));
            var task2 = Task.Factory.StartNew(() => TaskAction(Markers.Task2));
            
            await Task.WhenAll(task1, task2);
            
            Assert.AreEqual(iterations, tree.Count);

            AssertNodesDistribution(tree);
        }

        [TestCase(1000)]
        [TestCase(100000)]
        public async Task AddRangeNonIntersectedElementsTest(int iterations) {
            var tree = new ConcurrentBinaryTree<ConcurrentData>(TimeSpan.FromMilliseconds(1000));

            void TaskAction(int from, int count, Markers marker) {
                var array = Enumerable.Range(from, count)
                                      .Select(i => new ConcurrentData(i, marker))
                                      .ToArray();
                array.Shuffle();
                tree.TryAddRange(array);
            }
            
            var task1 = Task.Run(() => TaskAction(0, iterations, Markers.Task1));
            var task2 = Task.Run(() => TaskAction(iterations, iterations, Markers.Task2));

            await Task.WhenAll(task1, task2);
            Assert.AreEqual(iterations * 2, tree.Count);

            AssertNodesDistribution(tree, iterations);
        }
        
        [Test]
        public async Task FindTest([Values(10, 100, 1000)]int iterations) {
            var tree = new ConcurrentBinaryTree<int>(-1);
            
            for (int i = 0; i < iterations; i++) {
                tree.Add(i);
            }

            var task1 = Task.Factory.StartNew(() => {
                for (int i = iterations; i < iterations * 2; i++) {
                    tree.Add(i);
                }
            });

            var task2 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    Assert.IsTrue(tree.TryFind(i, out var iValue));
                    Assert.IsTrue(tree.TryFindMax(out var maxValue));
                    Assert.IsTrue(tree.TryFindMin(out var minValue));
                    
                    Assert.NotNull(iValue);
                    Assert.NotNull(minValue);
                    Assert.NotNull(maxValue);
                    
                    Assert.AreEqual(0, minValue.Data);
                    Assert.AreEqual(i, iValue.Data);
                    Assert.IsTrue(maxValue.Data >= iterations - 1);
                }
            });

            await Task.WhenAll(task1, task2);
        }
        
        [Repeat(3)]
        [TestCase(1000)]
        public async Task AddRemoveElementsTest(int iterations) {
            var tree = new ConcurrentBinaryTree<int>(-1);

            for (int i = 0; i < iterations; i++) {
                tree.Add(i);
            }
        
            Assert.AreEqual(iterations, tree.Count);
        
            var task1 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    tree.Remove(i);
                }
            });
        
            var task2 = Task.Factory.StartNew(() => {
                for (int i = iterations; i < iterations * 2; i++) {
                    tree.Add(i);
                }
            });

            await Task.WhenAll(task1, task2);
            Assert.AreEqual(iterations, tree.Count);
        }
        
        [Test]
        public async Task ContainsTest([Values(10, 100, 1000)]int iterations) {
            var tree = new ConcurrentBinaryTree<int>(-1);

            for (int i = 0; i < iterations; i++) {
                tree.Add(i);
            }

            var task1 = Task.Factory.StartNew(() => {
                for (int i = iterations; i < iterations * 2; i++) {
                    tree.Add(i);
                }
            });

            var task2 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    Assert.IsTrue(tree.Contains(i));
                }
            });

            await Task.WhenAll(task1, task2);
        }
        
        [Test]
        public async Task CopyToTest([Values(10, 100, 1000)]int iterations) {
            var tree = new ConcurrentBinaryTree<int>(TimeSpan.FromMilliseconds(1000));
            
            for (int i = 0; i < 10; i++) {
                tree.Add(i);
            }
            
            var task1 = Task.Factory.StartNew(() => {
                int[] array = new int[tree.Count];
                
                for (int i = 0; i < iterations; i++) {
                    tree.CopyTo(array, 0);
                    Assert.AreEqual(tree.ToArray(), array);
                }
            });
        
            var task2 = Task.Factory.StartNew(() => {
                int[] array = new int[tree.Count];
                
                for (int i = 0; i < iterations; i++) {
                    tree.CopyTo(array, 0);
                    Assert.AreEqual(tree.ToArray(), array);
                }
            });

            await Task.WhenAll(task1, task2);
        }

        [Test]
        public async Task TryDequeueTest([Values(10, 100, 1000)]int iterations) {
            var tree = new ConcurrentBinaryTree<int>(-1);
            
            for (int i = 0; i < iterations; i++) {
                tree.Add(i);
            }
            
            var task1 = Task.Factory.StartNew(() => {
                for (int i = iterations; i < iterations * 2; i++) {
                    tree.Add(i);
                }
            });
        
            var task2 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    Assert.IsTrue(tree.TryDequeue(out var result));
                }
            });

            await Task.WhenAll(task1, task2);
        }

        [Test]
        public async Task FindMinMaxHeightTest([Values(10, 100, 1000)]int iterations) {
            var tree = new ConcurrentBinaryTree<int>(-1);
            
            for (int i = 0; i < iterations; i++) {
                tree.Add(i);
            }
            
            var task1 = Task.Factory.StartNew(() => {
                for (int i = iterations; i < iterations * 2; i++) {
                    tree.Add(i);
                }
            });
        
            var task2 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    int minHeight = tree.FindMinHeight();
                    int maxHeight = tree.FindMaxHeight();
                    
                    Assert.IsTrue(minHeight == 0);
                    Assert.IsTrue(maxHeight >= iterations && maxHeight <= iterations * 2);
                }
            });

            await Task.WhenAll(task1, task2);
        }
        
        [Test, Repeat(3)]
        public async Task ClearTest([Values(10, 100, 1000)] int iterations) {
            var tree = new ConcurrentBinaryTree<int>(-1);

            for (int i = 0; i < iterations; i++) {
                tree.Add(i);
            }

            var task1 = Task.Factory.StartNew(() => {
                for (int i = iterations; i < iterations * 2; i++) {
                    tree.Add(i);
                }
            });

            var task2 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    tree.Clear();
                }
            });

            await Task.WhenAll(task1, task2);
            
            for (int i = 0; i < iterations; i++) {
                Assert.IsFalse(tree.Contains(i));
            }
        }

#region Utils

        private enum Markers {
            Task1, Task2
        }

        private record ConcurrentData(int Sorter, Markers Marker) : IComparable<ConcurrentData> {
            public int CompareTo(ConcurrentData? other) {
                if (ReferenceEquals(this, other)) return 0;
                if (ReferenceEquals(null, other)) return 1;
                var sorterComparison = Sorter.CompareTo(other.Sorter);

                return sorterComparison;
            }
        }
        
        private static void AssertNodesDistribution(ConcurrentBinaryTree<ConcurrentData> tree, int? iterations = null) {
            int[] markersCount = new int[] { 0, 0 };
            foreach (var concurrentData in tree) {
                if (concurrentData.Marker == Markers.Task1) {
                    markersCount[0]++;
                } else if (concurrentData.Marker == Markers.Task2) {
                    markersCount[1]++;
                }
            }
            
            Assert.AreNotEqual(markersCount[0], tree.NodesCount);
            Assert.AreNotEqual(markersCount[0], 0);
            Assert.AreNotEqual(markersCount[1], tree.NodesCount);
            Assert.AreNotEqual(markersCount[1], 0);

            if (iterations.HasValue) {
                Assert.AreEqual(markersCount[0], iterations);
                Assert.AreEqual(markersCount[1], iterations);
            }
        }

#endregion
    }
}