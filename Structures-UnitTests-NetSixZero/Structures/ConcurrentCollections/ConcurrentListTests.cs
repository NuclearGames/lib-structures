using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Structures.NetSixZero.Structures.ConcurrentCollections;

namespace Structures_UnitTests_NetSixZero.Structures.ConcurrentCollections {
    public class ConcurrentListTests {
        [Repeat(3)]
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

        [Repeat(3)]
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

        [Repeat(3)]
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

        [Test]
        public async Task CapacityTest([Values(1, 0, 100, 1000, 10000)]int iterations) {
            var list = new ConcurrentList<int>(-1);

            var task1 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    list.Add(i);
                }
            });
        
            var task2 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    list.Add(i);
                }
            });

            await Task.WhenAll(task1, task2);
            var expectedCapacity = 0;
            
            if (iterations == 1) {
                expectedCapacity = 4;
            } else if (iterations >= 2) {
                expectedCapacity = (int) Math.Pow(2, (int) Math.Ceiling(Math.Log(iterations * 2, 2)));
            }
            
            Assert.AreEqual(expectedCapacity, list.Capacity);
        }

        [Test]
        public async Task SetTest([Values(10, 100, 1000, 10000)]int iterations) {
            var list = new ConcurrentList<int>(-1);
            
            for (int i = 0; i < iterations; i++) {
                list.Add(i);
            }
            
            Assert.AreEqual(iterations, list.Count);

            var task1 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    list[i] = 5;
                }
            });
        
            var task2 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    list[i] = 5;
                }
            });

            await Task.WhenAll(task1, task2);

            for (int i = 0; i < iterations; i++) {
               Assert.AreEqual(5, list[i]);
            }
        }

        [Test]
        public async Task AddRangeTest([Values(1, 10, 100, 1000)]int iterations) {
            var list = new ConcurrentList<int>(-1);

            IEnumerable<int> enumerable1 = new List<int> {
                0, 1, 2, 3, 4, 5, 6, 7, 8, 9
            };
            
            IEnumerable<int> enumerable2 = new HashSet<int> {
                10, 11, 12, 13, 14, 15, 16, 17, 18, 19
            };

            var task1 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    list.AddRange(enumerable1);
                }
            });
        
            var task2 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    list.AddRange(enumerable2);
                }
            });
            
            await Task.WhenAll(task1, task2);
            
            var expectedCount = (enumerable1.Count() + enumerable2.Count()) * iterations;
            Assert.AreEqual(expectedCount, list.Count);
        }

        [Test]
        public async Task BinarySearchTest([Values(10, 100, 10000)]int iterations) {
            var list = new ConcurrentList<int>(-1);
            
            for (int i = 0; i < iterations; i++) {
                list.Add(i);
            }

            var task1 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    Assert.AreEqual(i, list.BinarySearch(i));
                    Assert.AreEqual(i, list.BinarySearch(i, Comparer<int>.Default));
                    Assert.AreEqual(i, list.BinarySearch(0, iterations, i, Comparer<int>.Default));
                }
            });
        
            var task2 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    Assert.AreEqual(i, list.BinarySearch(i));
                    Assert.AreEqual(i, list.BinarySearch(i, Comparer<int>.Default));
                    Assert.AreEqual(i, list.BinarySearch(0, iterations, i, Comparer<int>.Default));
                }
            });

            await Task.WhenAll(task1, task2);
        }

        [Test]
        public async Task CopyToTest([Values(10, 100, 1000)]int iterations) {
            var list = new ConcurrentList<int>(TimeSpan.FromMilliseconds(10));
            
            for (int i = 0; i < 10; i++) {
                list.Add(i);
            }
            
            var task1 = Task.Factory.StartNew(() => {
                int[] array = new int[list.Count];
                
                for (int i = 0; i < iterations; i++) {
                    list.CopyTo(array);
                    Assert.AreEqual(list.ToArray(), array);
                    
                    list.CopyTo(array, 0);
                    Assert.AreEqual(list.ToArray(), array);
                    
                    list.CopyTo(0, array, 0, list.Count);
                    Assert.AreEqual(list.ToArray(), array);
                }
            });
        
            var task2 = Task.Factory.StartNew(() => {
                int[] array = new int[list.Count];
                
                for (int i = 0; i < iterations; i++) {
                    list.CopyTo(array);
                    Assert.AreEqual(list.ToArray(), array);
                    
                    list.CopyTo(array, 0);
                    Assert.AreEqual(list.ToArray(), array);
                    
                    list.CopyTo(0, array, 0, list.Count);
                    Assert.AreEqual(list.ToArray(), array);
                }
            });

            await Task.WhenAll(task1, task2);
        }

        [Test]
        public async Task EnsureCapacityTest([Values(10, 100, 1000)]int iterations) {
            var list = new ConcurrentList<int>(TimeSpan.FromMilliseconds(1000));
            
            var expectedCapacity = 0;
            
            if (iterations == 1) {
                expectedCapacity = 4;
            } else if (iterations >= 2) {
                expectedCapacity = (int) Math.Pow(2, (int) Math.Ceiling(Math.Log(iterations, 2)));
            }

            var task1 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    list.Add(i);
                }
            });

            var finalCapacity = 0;
            var task2 = Task.Factory.StartNew(() => {
                for (int i = 1; i < iterations; i++) {
                    finalCapacity = list.EnsureCapacity(expectedCapacity + i);
                }
            });

            await Task.WhenAll(task1, task2);
            
            Assert.AreEqual(finalCapacity, list.Capacity);
            Assert.AreEqual(iterations, list.Count);
        }

        [Test]
        public async Task ExistsTest([Values(10, 100, 1000)]int iterations) {
            var list = new ConcurrentList<int>(TimeSpan.FromMilliseconds(1000));

            for (int i = 0; i < iterations; i++) {
                list.Add(i);
            }

            var task1 = Task.Factory.StartNew(() => {
                for (int i = iterations; i < iterations * 2; i++) {
                    list.Add(i);
                }
            });

            var task2 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    Assert.IsTrue(list.Exists(item => item == i));
                }
            });

            await Task.WhenAll(task1, task2);
        }

        [Test]
        public async Task GetRangeTest([Values(10, 100, 1000)]int iterations) {
            var list = new ConcurrentList<int>(TimeSpan.FromMilliseconds(1000));

            for (int i = 0; i < iterations; i++) {
                list.Add(i);
            }

            var task1 = Task.Factory.StartNew(() => {
                for (int i = iterations; i < iterations * 2; i++) {
                    list.Add(i);
                }
            });

            var task2 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    var range = list.GetRange(i, iterations - i);

                    for (int j = i; j < iterations - i; j++) {
                        Assert.AreEqual(j, range[j - i]);
                    }
                }
            });

            await Task.WhenAll(task1, task2);
        }
        
        [Test]
        public async Task ContainsTest([Values(10, 100, 1000)]int iterations) {
            var list = new ConcurrentList<int>(TimeSpan.FromMilliseconds(1000));

            for (int i = 0; i < iterations; i++) {
                list.Add(i);
            }

            var task1 = Task.Factory.StartNew(() => {
                for (int i = iterations; i < iterations * 2; i++) {
                    list.Add(i);
                }
            });

            var task2 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    Assert.IsTrue(list.Contains(i));
                }
            });

            await Task.WhenAll(task1, task2);
        }

        [Test]
        public async Task FindTest([Values(10, 100, 1000)]int iterations) {
            var list = new ConcurrentList<int>(TimeSpan.FromMilliseconds(1000));
            var array = new int[iterations];
            
            for (int i = 0; i < iterations; i++) {
                list.Add(i);
                array[i] = i;
            }

            var task1 = Task.Factory.StartNew(() => {
                for (int i = iterations; i < iterations * 2; i++) {
                    list.Add(i);
                }
            });

            var task2 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    Assert.AreEqual(i, list.Find(item => item == i));
                    Assert.AreEqual(array, list.FindAll(item => item < iterations));
                }
            });

            await Task.WhenAll(task1, task2);
        }
        
        [Test]
        public async Task FindIndexTest([Values(10, 100, 1000)]int iterations) {
            var list = new ConcurrentList<int>(-1);

            for (int i = 0; i < iterations; i++) {
                list.Add(i);
            }

            var task1 = Task.Factory.StartNew(() => {
                for (int i = iterations; i < iterations * 2; i++) {
                    list.Add(i);
                }
            });

            var task2 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    Assert.AreEqual(i, list.FindIndex(item => item == i));
                    Assert.AreEqual(i, list.FindIndex(0, item => item == i));
                    Assert.AreEqual(i, list.FindIndex(0, iterations, item => item == i));
                }
            });

            await Task.WhenAll(task1, task2);
        }

        [Test]
        public async Task FindLastIndexTest([Values(10, 100, 1000)]int iterations) {
            var list = new ConcurrentList<int>(TimeSpan.FromMilliseconds(1000));

            for (int i = 0; i < iterations; i++) {
                list.Add(i);
            }

            var task1 = Task.Factory.StartNew(() => {
                for (int i = iterations; i < iterations * 2; i++) {
                    list.Add(i);
                }
            });

            var task2 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    Assert.AreEqual(i, list.FindLastIndex(item => item == i));
                    Assert.AreEqual(i, list.FindLastIndex(iterations - 1, item => item == i));
                    Assert.AreEqual(i, list.FindLastIndex(iterations - 1, iterations, item => item == i));
                }
            });

            await Task.WhenAll(task1, task2);
        }

        [Test]
        public async Task FindLastTest([Values(10, 100, 1000)]int iterations) {
            var list = new ConcurrentList<int>(TimeSpan.FromMilliseconds(1000));

            for (int i = 0; i < iterations; i++) {
                list.Add(i);
            }

            var task1 = Task.Factory.StartNew(() => {
                for (int i = iterations; i < iterations * 2; i++) {
                    list.Add(i);
                }
            });

            var task2 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    Assert.AreEqual(i, list.FindLast(item => item == i));
                }
            });

            await Task.WhenAll(task1, task2);
        }

        [Test, Repeat(5)]
        public async Task ForEachTest([Values(10, 100, 1000)]int iterations) {
            var list = new ConcurrentList<int>(TimeSpan.FromMilliseconds(10));

            for (int i = 0; i < iterations; i++) {
                list.Add(i);
            }

            var task1 = Task.Factory.StartNew(() => {
                var counter = 0;
                list.ForEach(item => {
                    Assert.AreEqual(counter, item);
                    counter++;
                });
            });

            var task2 = Task.Factory.StartNew(() => {
                var counter = 0;
                list.ForEach(item => {
                    Assert.AreEqual(counter, item);
                    counter++;
                });
            });

            await Task.WhenAll(task1, task2);
        }
        
        [Test]
        public async Task TrueForAllTest([Values(10, 100, 1000)]int iterations) {
            var list = new ConcurrentList<int>(-1);

            for (int i = 0; i < iterations; i++) {
                list.Add(i);
            }

            var task1 = Task.Factory.StartNew(() => {
                for (int i = iterations; i < iterations * 2; i++) {
                    list.Add(i);
                }
            });

            var task2 = Task.Factory.StartNew(() => {
                for (int i = iterations; i < iterations * 2; i++) {
                    Assert.IsTrue(list.TrueForAll(item => item > -1));
                }
            });

            await Task.WhenAll(task1, task2);
        }

        [Test]
        public async Task IndexOfTest([Values(10, 100, 1000)] int iterations) {
            var list = new ConcurrentList<int>(TimeSpan.FromMilliseconds(1000));

            for (int i = 0; i < iterations; i++) {
                list.Add(i);
            }

            var task1 = Task.Factory.StartNew(() => {
                for (int i = iterations; i < iterations * 2; i++) {
                    list.Add(i);
                }
            });

            var task2 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    Assert.AreEqual(i, list.IndexOf(i));
                    Assert.AreEqual(i, list.IndexOf(i, 0));
                    Assert.AreEqual(i, list.IndexOf(i, 0, iterations));
                }
            });

            await Task.WhenAll(task1, task2);
        }
        
        [Test]
        public async Task LastIndexOfTest([Values(10, 100, 1000)] int iterations) {
            var list = new ConcurrentList<int>(TimeSpan.FromMilliseconds(1000));

            for (int i = 0; i < iterations; i++) {
                list.Add(i);
            }

            var task1 = Task.Factory.StartNew(() => {
                for (int i = iterations; i < iterations * 2; i++) {
                    list.Add(i);
                }
            });

            var task2 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    Assert.AreEqual(i, list.LastIndexOf(i));
                    Assert.AreEqual(i, list.LastIndexOf(i, iterations - 1));
                    Assert.AreEqual(i, list.LastIndexOf(i, iterations - 1, iterations));
                }
            });

            await Task.WhenAll(task1, task2);
        }
        
        [Test, Repeat(10)]
        public async Task RemoveAllTest([Values(10, 100, 1000)] int iterations) {
            var list = new ConcurrentList<int>(TimeSpan.FromMilliseconds(1000));

            for (int i = 0; i < iterations; i++) {
                list.Add(i);
            }

            var task1 = Task.Factory.StartNew(() => {
                for (int i = iterations; i < iterations * 2; i++) {
                    list.Add(i);
                }
            });

            var task2 = Task.Factory.StartNew(() => {
                list.RemoveAll(item => item < iterations);
            });

            await Task.WhenAll(task1, task2);
            
            for (int i = 0; i < iterations; i++) {
                Assert.AreEqual(i + iterations, list[i]);
            }
        }
        
        [Test, Repeat(10)]
        public async Task RemoveAtTest([Values(10, 100, 1000)] int iterations) {
            var list = new ConcurrentList<int>(-1);

            for (int i = 0; i < iterations; i++) {
                list.Add(i);
            }

            var task1 = Task.Factory.StartNew(() => {
                for (int i = iterations; i < iterations * 2; i++) {
                    list.Add(i);
                }
            });

            var task2 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    list.RemoveAt(0);
                }
            });

            await Task.WhenAll(task1, task2);
            
            for (int i = 0; i < iterations; i++) {
                Assert.AreEqual(i + iterations, list[i]);
            }
        }
        
        [Test, Repeat(10)]
        public async Task RemoveRangeTest([Values(10, 100, 1000)] int iterations) {
            var list = new ConcurrentList<int>(-1);

            for (int i = 0; i < iterations; i++) {
                list.Add(i);
            }

            var task1 = Task.Factory.StartNew(() => {
                for (int i = iterations; i < iterations * 2; i++) {
                    list.Add(i);
                }
            });

            var task2 = Task.Factory.StartNew(() => {
                list.RemoveRange(0, iterations);
            });

            await Task.WhenAll(task1, task2);
            
            for (int i = 0; i < iterations; i++) {
                Assert.AreEqual(i + iterations, list[i]);
            }
        }

        [Test, Repeat(3)]
        public async Task InsertTest([Values(10, 100, 1000)] int iterations) {
            var list = new ConcurrentList<int>(-1);

            for (int i = 0; i < iterations; i++) {
                list.Add(i);
            }

            var task1 = Task.Factory.StartNew(() => {
                for (int i = iterations; i < iterations * 2; i++) {
                    list.Insert(0, i);
                }
            });

            var task2 = Task.Factory.StartNew(() => {
                for (int i = iterations; i < iterations * 2; i++) {
                    list.Insert(0, i);
                }
            });

            await Task.WhenAll(task1, task2);
            Assert.AreEqual(iterations * 3, list.Count);
        }
        
        [Test, Repeat(3)]
        public async Task InsertRangeTest([Values(10, 100, 1000)] int iterations) {
            var list = new ConcurrentList<int>(-1);

            for (int i = 0; i < iterations; i++) {
                list.Add(i);
            }

            IEnumerable<int> collection = new[] {
                2, 3, 5, 7, 8, 9
            };

            var task1 = Task.Factory.StartNew(() => {
                for (int i = iterations; i < iterations * 2; i++) {
                    list.InsertRange(0, collection);
                }
            });

            var task2 = Task.Factory.StartNew(() => {
                for (int i = iterations; i < iterations * 2; i++) {
                    list.InsertRange(0, collection);
                }
            });

            await Task.WhenAll(task1, task2);
            Assert.AreEqual(iterations * (2 * collection.Count() + 1), list.Count);
        }

        [Test, Repeat(3)]
        public async Task ClearTest([Values(10, 100, 1000)] int iterations) {
            var list = new ConcurrentList<int>(-1);

            for (int i = 0; i < iterations; i++) {
                list.Add(i);
            }

            var task1 = Task.Factory.StartNew(() => {
                for (int i = iterations; i < iterations * 2; i++) {
                    list.Add(i);
                }
            });

            var task2 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    list.Clear();
                }
            });

            await Task.WhenAll(task1, task2);
            
            for (int i = 0; i < iterations; i++) {
                Assert.IsFalse(list.Contains(i));
            }
        }
        
        [Test, Repeat(3)]
        public async Task ConvertAll([Values(10, 100, 1000)] int iterations) {
            var list = new ConcurrentList<int>(-1);

            for (int i = 0; i < iterations; i++) {
                list.Add(i);
            }

            var task1 = Task.Factory.StartNew(() => {
                for (int i = iterations; i < iterations * 2; i++) {
                    list.Add(i);
                }
            });

            int ConvertFunction(int input) {
                return input + 1;
            }

            List<int> convertedList = new();
            var task2 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    convertedList = list.ConvertAll(ConvertFunction);
                }
            });

            await Task.WhenAll(task1, task2);
            
            for (int i = 0; i < iterations; i++) {
                Assert.AreEqual(i + 1, convertedList[i]);
            }
        }
    }
}