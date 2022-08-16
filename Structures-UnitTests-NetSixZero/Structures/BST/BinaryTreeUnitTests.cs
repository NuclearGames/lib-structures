using System;
using System.Linq;
using NUnit.Framework;
using Structures.NetSixZero.Structures.BST;
using Structures.NetSixZero.Structures.BST.Utils;


namespace Structures_UnitTests_NetSixZero.Structures.BST {
    public class BinaryTreeUnitTests {
        private readonly int[] _dataCollection = new int[] {
            9, 4, 17, 3, 6, 22, 5 ,7, 20
        };
        private BinaryTree<int> _tree;

        [SetUp]
        public void SetUp() {
            _tree = new BinaryTree<int>();
        }

        [TestCase(17)]
        public void InitialTests(int data) {
            Assert.AreEqual(true, _tree.IsEmpty);
            Assert.AreEqual(false, _tree.TryFind(data, out _));
            Assert.AreEqual(false, _tree.TryFindMin(out _));
            Assert.AreEqual(false, _tree.TryFindMax(out _));
            Assert.AreEqual(false, _tree.Remove(data));
            
            Assert.AreEqual(0, _tree.ToArray().Length);
        }


        [Test]
        public void ConstructorTest([Values(10, 11, 1000, 1001)]int count) {
            var initial = Enumerable.Range(0, count).ToArray();
            var newTree = new BinaryTree<int>(initial);
            Assert.AreEqual(initial.Length, newTree.ToArray().Length);
        }

        [TestCase]
        public void SuccessfulAdd() {
            FillTree(true);
        }
        
        [Test]
        public void SuccessfulAddRange([Values(10, 11, 1000, 1001)]int count) {
            var sourceBuffer = Enumerable.Range(0, count).ToArray();
            var newTree = new BinaryTree<int>(sourceBuffer);
            Assert.AreEqual(sourceBuffer.Length, newTree.NodesCount);
            Assert.AreEqual(sourceBuffer.Length, newTree.ToArray().Length);

            var newSourceBuffer = Enumerable.Range(count, count).ToArray();
            var addResult = newTree.TryAddRange(newSourceBuffer);
            Assert.AreEqual(true, addResult);
            Assert.AreEqual(count + count, newTree.NodesCount);
            Assert.AreEqual(count + count, newTree.ToArray().Length);
        }
        
        [TestCase(20)]
        public void FailAdd(int data) {
            FillTree();
            var addResult = _tree.TryAdd(data, out var node);
            
            Assert.AreEqual(false, addResult);
            Assert.IsNotNull(node);
            Assert.AreEqual(data, node.Data);
        }

        [TestCase]
        public void FoundAfter() {
            FillTree();
            Assert.AreEqual(true, _tree.TryFind(4, out _));
            Assert.AreEqual(false, _tree.TryFind(19, out _));
            
            void AssertResults(bool result, Node<int>? node, int? value) {
                Assert.AreEqual(value.HasValue, result);
                if (value.HasValue) {
                    Assert.IsNotNull(node);
                    Assert.AreEqual(node!.Data, value.Value);
                } else {
                    Assert.IsNull(node);
                }
            }
            
            var minResult = _tree.TryFindMin(out var minNode);
            AssertResults(minResult, minNode, 3);
            
            var maxResult = _tree.TryFindMax(out var maxNode);
            AssertResults(maxResult, maxNode, 22);
        }

        [TestCase]
        public void Children() {
            FillTree();
            CheckChild(6, 5, 7);
            CheckChild(3, null, null);
            CheckChild(17, null, 22);
            CheckChild(22, 20, null);
        }

        [TestCase]
        public void IterationIsOrdered() {
            FillTree();

            var values = _tree.ToArray();
            
            Assert.AreEqual(_dataCollection.Length, values.Length);
            
            for (int i = 0; i < values.Length - 1; i++) {
                Assert.LessOrEqual(values[i], values[i+1]);
            }
        }


        [TestCase]
        public void Clear() {
            FillTree();
            Assert.AreNotEqual(0, _tree.NodesCount);
            Assert.AreEqual(false, _tree.IsEmpty);
            _tree.Clear();
            Assert.AreEqual(true, _tree.IsEmpty);
            Assert.AreEqual(0, _tree.NodesCount);
        }

        [TestCase()]
        public void FindHeight() {
            FillTree();
            Assert.AreEqual(1, _tree.FindMinHeight());
            Assert.AreEqual(3, _tree.FindMaxHeight());
            Assert.AreEqual(false, _tree.IsBalanced());
            
            _tree.Clear();
            Assert.AreEqual(-1, _tree.FindMinHeight());
            Assert.AreEqual(-1, _tree.FindMaxHeight());
            Assert.AreEqual(true, _tree.IsBalanced());
        }

        [TestCase]
        public void Remove() {
            FillTree();

            var currentCount = _tree.NodesCount;
            
            var removeResult = _tree.Remove(3);
            Assert.AreEqual(true, removeResult);
            Assert.AreEqual(--currentCount, _tree.NodesCount);
            removeResult = _tree.Remove(3);
            Assert.AreEqual(false, removeResult);
            Assert.AreEqual(currentCount, _tree.NodesCount);

            removeResult = _tree.Remove(6);
            Assert.AreEqual(true, removeResult);
            Assert.AreEqual(--currentCount, _tree.NodesCount);

            removeResult = _tree.Remove(17);
            Assert.AreEqual(true, removeResult);
            Assert.AreEqual(--currentCount, _tree.NodesCount);

            _tree.TryFind(4, out var node);
            Assert.IsNotNull(node);
            Assert.IsNull(node!.Left);
            Assert.IsNotNull(node.Right);
            Assert.AreEqual(7, node.Right!.Data);
            Assert.IsNull(node.Right!.Right);
            Assert.IsNotNull(node.Right!.Left);
            Assert.AreEqual(5, node.Right!.Left!.Data);

            _tree.TryFind(22, out node);
            Assert.IsNotNull(node);
            Assert.IsNull(node!.Right);
            Assert.IsNotNull(node.Left);
            Assert.AreEqual(20, node.Left!.Data);
            Assert.IsNull(node.Left!.Left);
            Assert.IsNull(node.Left!.Right);

            _tree.TryAdd(50, out _);
            _tree.TryAdd(30, out _);
            currentCount += 2;
            Assert.AreEqual(currentCount, _tree.NodesCount);
            
            removeResult = _tree.Remove(22);
            Assert.AreEqual(true, removeResult);
            Assert.AreEqual(--currentCount, _tree.NodesCount);

            _tree.TryFind(30, out node);
            Assert.IsNotNull(node);
            Assert.IsNotNull(node!.Left);
            Assert.IsNotNull(node.Right);
            Assert.AreEqual(20, node.Left!.Data);
            Assert.AreEqual(50, node.Right!.Data);
        }

        [Test]
        public void CollectionInterfaceTest() {
            Assert.AreEqual(false, _tree.IsReadOnly);
            
            // Add and Count
            Assert.AreEqual(0, _tree.Count);
            
            _tree.Add(5);
            Assert.AreEqual(1, _tree.Count);

            Assert.Catch(() => _tree.Add(5));
            Assert.AreEqual(1, _tree.Count);
            
            _tree.Add(6);
            _tree.Add(4);
            
            Assert.AreEqual(3, _tree.Count);
            
            // Contains
            
            Assert.AreEqual(true, _tree.Contains(4));
            Assert.AreEqual(false, _tree.Contains(8));
            
            // Remove
            Assert.AreEqual(false, _tree.Remove(8));
            Assert.AreEqual(true, _tree.Remove(4));
            
            Assert.AreEqual(2, _tree.Count);
            
            //Copy to
            var destArray = new int[2];
            _tree.CopyTo(destArray, 0);
            var index = 0;
            foreach (var value in _tree) {
                Assert.AreEqual(value, destArray[index]);
                index++;
            }
            
            Array.Clear(destArray);
            Assert.Catch(() => _tree.CopyTo(destArray, 1));
        }
        
        [Test]
        public void AnyElementCollectionInterface(){
            Assert.AreEqual(true, _tree.IsEmpty);
            Assert.AreEqual(false, _tree.TryGetAny(out _));
            Assert.Catch(() => Console.WriteLine(_tree.Any));
            
            _tree.Add(5);
            Assert.AreEqual(false, _tree.IsEmpty);
            Assert.AreEqual(5, _tree.Any);

            bool saveAnyResult = _tree.TryGetAny(out var anyValue);
            Assert.AreEqual(true, saveAnyResult);
            Assert.AreEqual(5, anyValue);
        }

#region Utils

        private void FillTree(in bool checkAssertion = false) {
            for (int i = 0; i < _dataCollection.Length; i++) {
                var value = _dataCollection[i];
                var addResult = _tree.TryAdd(value, out var newNode);
                if (checkAssertion) {
                    Assert.AreEqual( true, addResult);
                    Assert.IsNotNull(newNode);
                    Assert.AreEqual(value, newNode.Data);
                }
            }

            if (checkAssertion) {
                Assert.AreEqual(_dataCollection.Length,  _tree.NodesCount);
            }
        }

        private void CheckChild(int data, int? left, int? right) {
            if (_tree.TryFind(data, out var node)) {
                Assert.IsNotNull(node);

                if (left.HasValue) {
                    Assert.IsNotNull(node!.Left);
                    Assert.AreEqual(left.Value, node.Left!.Data);
                } else {
                    Assert.IsNull(node!.Left);
                }
                
                if (right.HasValue) {
                    Assert.IsNotNull(node.Right);
                    Assert.AreEqual(right.Value, node.Right!.Data);
                } else {
                    Assert.IsNull(node.Right);
                }
            } else {
                Assert.Fail($"Value '{data}' wasn't found in tree!");
            }
        }

#endregion
    }
}