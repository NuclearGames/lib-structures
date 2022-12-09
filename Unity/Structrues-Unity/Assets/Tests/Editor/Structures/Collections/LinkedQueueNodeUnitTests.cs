using System;
using System.Collections.Generic;
using System.Linq;
using NuclearGames.StructuresUnity.Structures.Collections;
using NuclearGames.StructuresUnity.Structures.Collections.Utils;
using NUnit.Framework;

namespace Tests.Editor.Structures.Collections {
    public class LinkedQueueNodeUnitTests {
        
        [Test]
        public void EmptyConstructorTest() {
            var linkedQueue = new LinkedQueue<int>();
            
            Assert.AreEqual(0, linkedQueue.Count);
            Assert.Null(linkedQueue.First);
            Assert.Null(linkedQueue.Last);
        }

        [Test]
        public void SingleElementConstructorTest() {
            var value = new Random().Next(0, 10000);
            
            var linkedQueue = new LinkedQueue<int>(value);
            
            Assert.AreEqual(1, linkedQueue.Count);
            Assert.NotNull(linkedQueue.First);
            Assert.NotNull(linkedQueue.Last);
            Assert.AreEqual(linkedQueue.First, linkedQueue.Last);
            
            Assert.AreEqual(value, linkedQueue.First!.Value);
        }

        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public void EnumerableConstructorTest(int size) {
            var controlList = new List<int>(size);
            controlList.AddRange(Enumerable.Range(0, size));

            var linkedQueue = new LinkedQueue<int>(controlList);
            Assert.AreEqual(size, linkedQueue.Count);
            
            Assert.NotNull(linkedQueue.First);
            Assert.AreEqual(controlList[0], linkedQueue.First!.Value);
            
            Assert.NotNull(linkedQueue.Last);
            Assert.AreEqual(controlList[controlList.Count - 1], linkedQueue.Last!.Value);
        }

        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public void OperationsTest(int size) {
            var controlList = new List<int>(size);
            controlList.AddRange(Enumerable.Range(0, size));

            var linkedQueue = new LinkedQueue<int>();
            
            // Test Enqueue
            for (int i = 0; i < size; i++) {
                if (i == 0) {
                    Assert.Null(linkedQueue.First);
                    Assert.Null(linkedQueue.Last);
                }
                
                linkedQueue.Enqueue(controlList[i]);
                Assert.AreEqual(i+1, linkedQueue.Count);
                Assert.AreEqual(controlList[i],linkedQueue.Last!.Value);

                if (i == 0) {
                    Assert.NotNull(linkedQueue.First);
                    Assert.NotNull(linkedQueue.Last);
                    Assert.AreEqual(linkedQueue.First, linkedQueue.Last);
                } else {
                    Assert.NotNull(linkedQueue.First);
                    Assert.NotNull(linkedQueue.Last);
                    Assert.AreNotEqual(linkedQueue.First, linkedQueue.Last);
                }
            }
            
            
            // Test Dequeue
            
            for (int i = 0; i < size; i++) {
                Assert.AreEqual(size -i, linkedQueue.Count);
                if (i == size - 1) {
                    Assert.AreEqual(linkedQueue.First, linkedQueue.Last);
                }
                
                var dequeueResult = linkedQueue.TryDequeue(out var value);
                Assert.AreEqual(true, dequeueResult);
                Assert.AreEqual(size - i -1, linkedQueue.Count);
                
                Assert.AreEqual(controlList[i], value);
                if (i < size - 1) {
                    Assert.NotNull(linkedQueue.First);
                    Assert.NotNull(linkedQueue.Last);
                    
                    Assert.AreEqual(controlList[i+1], linkedQueue.First!.Value);
                } else {
                    Assert.Null(linkedQueue.First);
                    Assert.Null(linkedQueue.Last);
                }
            }
            
            Assert.Null(linkedQueue.First);
            Assert.Null(linkedQueue.Last);
            
            // Empty queue dequeue
            Assert.AreEqual(false, linkedQueue.TryDequeue(out _));
        }

        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public void CopyToTest(int size) {
            var destinationArray = new int[size];

            var queue = new LinkedQueue<int>(Enumerable.Range(0, size));
            queue.CopyTo(destinationArray, 0);
            
            Assert.AreEqual(destinationArray[0], 0);
            Assert.AreEqual(destinationArray[size- 1], size -1);
            
            Array.Clear(destinationArray, 0, destinationArray.Length);

            try {
                queue.CopyTo(destinationArray, 1);
                Assert.Fail("Should thrown error!");
            } catch (ArgumentOutOfRangeException) {
                
            }
        }
        
        [Test]
        public void NodeProps() {
            var queue = new LinkedQueue<int>(0);
            var node = queue.First;
            
            Assert.AreEqual(0, node!.Value);
            Assert.AreEqual(queue, node.Queue);

            object valueRef = node.ValueRef;
            node.Value = 1;
            
            Assert.AreEqual(1, node.Value);
            Assert.AreEqual(0, valueRef);
            
            queue.Enqueue(2);
            var nextNode = node.Next;
            Assert.AreEqual(2, nextNode!.Value);
        }

        /// <summary>
        /// Проверяет вызов получения и освобождения нода.
        /// </summary>
        [Test]
        public void GetReleaseNodeTest() {
            GetReleaseNodeLinkedQueue<int>? queue = null;
            int useNodesCounter = 0;

            LinkedQueueNode<int> GetNode(int data) {
                useNodesCounter++;
                return new LinkedQueueNode<int>(queue, data);
            }

            void ReleaseNode(LinkedQueueNode<int> node) {
                useNodesCounter--;
            }

            queue = new GetReleaseNodeLinkedQueue<int>(GetNode, ReleaseNode);

            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);
            Assert.AreEqual(3, useNodesCounter);

            queue.TryDequeue(out _);
            Assert.AreEqual(2, useNodesCounter);
            queue.TryDequeue(out _);
            queue.TryDequeue(out _);
            Assert.AreEqual(0, useNodesCounter);
        }

        private class GetReleaseNodeLinkedQueue<T> : LinkedQueue<T> {
            private readonly Func<T, LinkedQueueNode<T>> _getNode;
            private readonly Action<LinkedQueueNode<T>> _releaseNode;
            public GetReleaseNodeLinkedQueue(Func<T, LinkedQueueNode<T>> getNode, Action<LinkedQueueNode<T>> releaseNode) {
                _getNode = getNode;
                _releaseNode = releaseNode;
            }
            protected override LinkedQueueNode<T> GetNode(T value) => _getNode(value);
            protected override void ReleaseNode(LinkedQueueNode<T> node) => _releaseNode(node);
        }
    }
}