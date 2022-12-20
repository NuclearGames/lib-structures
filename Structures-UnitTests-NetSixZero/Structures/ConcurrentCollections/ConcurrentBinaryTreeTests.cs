using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Structures.NetSixZero.Structures.ConcurrentCollections;

namespace Structures_UnitTests_NetSixZero.Structures.ConcurrentCollections {
    public class ConcurrentBinaryTreeTests {
        [TestCase(1000)]
        [TestCase(1000)]
        [TestCase(1000)]
        public async Task AddElementsTest(int iterations) {
            var tree = new ConcurrentBinaryTree<int>(TimeSpan.FromMilliseconds(1000));

            var task1 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    tree.Add(i);
                }
            });
        
            var task2 = Task.Factory.StartNew(() => {
                for (int i = iterations; i < iterations * 2; i++) {
                    tree.Add(i);
                }
            });

            await Task.WhenAll(task1, task2);
            Assert.AreEqual(iterations * 2, tree.Count);
        }

        [TestCase(1000)]
        [TestCase(1000)]
        [TestCase(1000)]
        public async Task RemoveElementsTest(int iterations) {
            var tree = new ConcurrentBinaryTree<int>(TimeSpan.FromMilliseconds(1000));

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
        
        [TestCase(1000)]
        [TestCase(1000)]
        [TestCase(1000)]
        public async Task GetElementsTest(int iterations) {
            var tree = new ConcurrentBinaryTree<int>(TimeSpan.FromMilliseconds(1));

            for (int i = 0; i < iterations; i++) {
                tree.Add(i);
            }
        
            Assert.AreEqual(iterations, tree.Count);
            
            var task1 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    Assert.IsTrue(tree.Contains(i));
                }
            });
        
            var task2 = Task.Factory.StartNew(() => {
                for (int i = 0; i < iterations; i++) {
                    Assert.IsTrue(tree.Contains(i));
                }
            });

            await Task.WhenAll(task1, task2);
        }
    }
}