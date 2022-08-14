using System;
using System.Collections;
using System.Collections.Generic;


namespace Structures.NetStandard.Structures.Collections {
    public class BinaryHeap<TData, TComparable> : IEnumerable<TData> where TComparable : IComparable<TComparable> {
        /// <summary>
        ///     Comparer to use when comparing elements.
        /// </summary>
        private readonly Comparer<TComparable> _comparer;
        
        /// <summary>
        ///     Gets the number of elements in the heap.
        /// </summary>
        public int Count => _data.Count;

        /// <summary>
        ///     List to hold the elements of the heap.
        /// </summary>
        private readonly List<TData> _data;
        
        /// <summary>
        ///     Selector comparable field from object 
        /// </summary>
        private readonly Func<TData, TComparable> _compareFieldSelector;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BinaryHeap{TData, TComparable}" /> class.
        /// </summary>
        /// <param name="compareFieldSelector">Selector field from <typeparamref name="TData"/>-Object</param>
        /// <param name="size">The initial size of list</param>
        public BinaryHeap(Func<TData, TComparable> compareFieldSelector, int size = 0) : 
            this(compareFieldSelector, Comparer<TComparable>.Default, size) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BinaryHeap{TData, TComparable}" /> class.
        /// </summary>
        /// <param name="compareFieldSelector">Selector field from <typeparamref name="TData"/>-Object</param>
        /// <param name="comparer">Comparer to use when comparing elements.</param>
        /// <param name="size">The initial size of list</param>
        public BinaryHeap(Func<TData, TComparable> compareFieldSelector, Comparer<TComparable> comparer, int size = 0) {
            _data = size != 0 
                ? new List<TData>(size) 
                : new List<TData>();

            _compareFieldSelector = compareFieldSelector;
            _comparer = comparer;
        }

        /// <summary>
        ///     Add an element to the binary heap.
        /// </summary>
        /// <remarks>
        ///     Adding to the heap is done by append the element to the end of the backing list,
        ///     and pushing the added element up until the heap property is restored.
        /// </remarks>
        /// <param name="element">The element to add to the heap.</param>
        /// <exception cref="ArgumentException">Thrown if element is already in heap.</exception>
        public void Push(TData element) {
            _data.Add(element);
            HeapifyUp(_data.Count - 1);
        }

        /// <summary>
        ///     Remove the top/root of the binary heap (ie: the largest/smallest element).
        /// </summary>
        /// <remarks>
        ///     Removing from the heap is done by swapping the top/root with the last element in
        ///     the backing list, removing the last element, and pushing the new root down
        ///     until the heap property is restored.
        /// </remarks>
        /// <returns>The top/root of the heap.</returns>
        /// <exception cref="InvalidOperationException">Thrown if heap is empty.</exception>
        public TData Pop() {
            if (Count == 0) {
                throw new InvalidOperationException("Heap is empty!");
            }

            var elem = _data[0];
            _data[0] = _data[^1];
            _data.RemoveAt(_data.Count - 1);
            HeapifyDown(0);

            return elem;
        }

        /// <summary>
        ///     Return the top/root of the heap without removing it.
        /// </summary>
        /// <returns>The top/root of the heap.</returns>
        /// <exception cref="InvalidOperationException">Thrown if heap is empty.</exception>
        public TData Peek() {
            if (Count == 0) {
                throw new InvalidOperationException("Heap is empty!");
            }

            return _data[0];
        }

        /// <summary>
        ///     Returns element if it compares larger to the top/root of the heap, else
        ///     inserts element into the heap and returns the top/root of the heap.
        /// </summary>
        /// <param name="element">The element to check/insert.</param>
        /// <returns>element if element compares larger than top/root of heap, top/root of heap otherwise.</returns>
        public TData PushPop(TData element) {
            if (Count == 0) {
                return element;
            }

            if (Compare(element, _data[0]) < 0) {
                var tmp = _data[0];
                _data[0] = element;
                HeapifyDown(0);

                return tmp;
            }

            return element;
        }

        /// <summary>
        ///     Check if element is in the heap.
        /// </summary>
        /// <param name="element">The element to check for.</param>
        /// <returns>true if element is in the heap, false otherwise.</returns>
        public bool Contains(TData element) => _data.Contains(element);

        /// <summary>
        ///     Check if element with <paramref name="key"/> exists
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(TComparable key) {
            var searchResult = _data.Find(el => _compareFieldSelector(el).CompareTo(key) == 0);
            return !ReferenceEquals(searchResult, null);
        }

        /// <summary>
        ///     Remove an element from the heap.
        /// </summary>
        /// <remarks>
        ///     In removing an element from anywhere in the heap, we only need to push down or up
        ///     the replacement value depending on how the removed value compares to its
        ///     replacement value.
        /// </remarks>
        /// <param name="element">The element to remove from the heap.</param>
        /// <exception cref="ArgumentException">Thrown if element is not in heap.</exception>
        public void Remove(TData element) {
            var idx = _data.IndexOf(element);
            RemoveAt(idx);
        }

        private void RemoveAt(int idx) {
            if (idx == -1) {
                throw new ArgumentException($"Invalid heap index!");
            }

            Swap(idx, _data.Count - 1);
            var tmp = _data[^1];
            _data.RemoveAt(_data.Count - 1);

            if (idx < _data.Count) {
                if (Compare(tmp, _data[idx]) > 0) {
                    HeapifyDown(idx);
                } else {
                    HeapifyUp(idx);
                }
            }
        }
        
        /// <summary>
        ///     Firstly search any element by it's key. When found - use <seealso cref="Remove"/> to remove element 
        /// </summary>
        /// <param name="key">Key of element using while sorting through heap</param>
        /// <param name="searchResult">First found element (if it exists) that will be removed</param>
        /// <returns>True, if element was found by key and removed; False - in other way</returns>
        public bool TryRemoveByKey(TComparable key, out TData searchResult) {
            var searchIndex = _data.FindIndex(el => _compareFieldSelector(el).CompareTo(key) == 0);
            if (searchIndex == -1) {
                searchResult = default;
                return false;
            }

            searchResult = _data[searchIndex];
            if (ReferenceEquals(searchResult, null)) {
                return false;
            }
            
            RemoveAt(searchIndex);
            return true;
        }

        /// <summary>
        ///     Swap the elements in the heap array at the given indices.
        /// </summary>
        /// <param name="idx1">First index.</param>
        /// <param name="idx2">Second index.</param>
        private void Swap(int idx1, int idx2) {
            (_data[idx1], _data[idx2]) = (_data[idx2], _data[idx1]);
        }

        /// <summary>
        ///     Compare two elements of type <typeparamref name="TData"/> by their fields of <typeparamref name="TComparable"/>
        ///     with help of selector <seealso cref="_compareFieldSelector"/>
        /// </summary>
        /// <param name="first">First element</param>
        /// <param name="second">Second element</param>
        /// <returns>The result of comparison</returns>
        private int Compare(TData first, TData second) {
            return _comparer.Compare(_compareFieldSelector(first), _compareFieldSelector(second));
        }

        /// <summary>
        ///     Recursive function to restore heap properties.
        /// </summary>
        /// <remarks>
        ///     Restores heap property by swapping the element at <paramref name="elemIdx" />
        ///     with its parent if the element compares greater than its parent.
        /// </remarks>
        /// <param name="elemIdx">The element to check with its parent.</param>
        private void HeapifyUp(int elemIdx) {
            var parent = (elemIdx - 1) / 2;

            if (parent >= 0 && Compare(_data[elemIdx], _data[parent]) > 0) {
                Swap(elemIdx, parent);
                HeapifyUp(parent);
            }
        }

        /// <summary>
        ///     Recursive function to restore heap properties.
        /// </summary>
        /// <remarks>
        ///     Restores heap property by swapping the element at <paramref name="elemIdx" />
        ///     with the larger of its children.
        /// </remarks>
        /// <param name="elemIdx">The element to check with its children.</param>
        private void HeapifyDown(int elemIdx) {
            var left = 2 * elemIdx + 1;
            var right = 2 * elemIdx + 2;

            var leftLargerThanElem = left < Count && Compare(_data[elemIdx], _data[left]) < 0;
            var rightLargerThanElem = right < Count && Compare(_data[elemIdx], _data[right]) < 0;
            var leftLargerThanRight = left < Count && right < Count && Compare(_data[left], _data[right]) > 0;

            if (leftLargerThanElem && leftLargerThanRight) {
                Swap(elemIdx, left);
                HeapifyDown(left);
            } else if (rightLargerThanElem && !leftLargerThanRight) {
                Swap(elemIdx, right);
                HeapifyDown(right);
            }
        }

#region Enumerable

        public IEnumerator<TData> GetEnumerator() {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

#endregion
    }
}