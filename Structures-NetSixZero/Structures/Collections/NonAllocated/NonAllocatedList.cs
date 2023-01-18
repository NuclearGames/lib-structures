namespace Structures.NetSixZero.Structures.Collections.NonAllocated {
    public ref struct NonAllocatedList<T> where T : unmanaged, IComparable<T> {
        private readonly Span<T> _buffer;
        private int _count;

        public NonAllocatedList(ref Span<T> span) {
            _buffer = span;
            _count = 0;
        }

#region ICollection

        /// <summary>
        /// Current count of elements if buffer
        /// </summary>
        public int Count => _count;

        public bool IsReadOnly => false;

        /// <summary>
        /// Append value to the end with operation <seealso cref="Insert"/>
        /// </summary>
        public void Add(in T item) {
            Insert(_count, in item);
        }

        /// <summary>
        /// Returns if buffer contains value of <paramref name="item"/>
        /// </summary>
        public bool Contains(in T item) {
            return IndexOf(in item) != -1;
        }

        /// <summary>
        /// CopyTo copies a collection into an Array, starting at a particular index into the array.
        /// </summary>
        public void CopyTo(T[] destArray, in int destArrayStartIndex) {
            if (destArray.Length - destArrayStartIndex < _count) {
                throw new ArgumentOutOfRangeException(nameof(destArray),
                                                      $"Can't insert {_count} elements to array (sizeof {destArray.Length}) at index {destArrayStartIndex}");
            }

            for (int i = 0; i < _count; i++) {
                destArray[destArrayStartIndex + i] = _buffer[i];
            }
        }

        /// <summary>
        /// Remove element <paramref name="item"/> if it's exists 
        /// </summary>
        public bool Remove(in T item) {
            var index = IndexOf(in item);
            if (index == -1) {
                return false;
            }

            try {
                AssertCanRemove(in index);
                RemoveWithPopInternal(in index);

                return true;
            } catch {
                return false;
            }
        }

        /// <summary>
        /// Clear buffer
        /// </summary>
        public void Clear() {
            for (int i = 0; i < _count; i++) {
                _buffer[i] = default;
            }

            _count = 0;
        }

#endregion


#region IList

        /// <summary>
        /// The Item property provides methods to read and edit entries in the List.
        /// </summary>
        public ref T this[int index] {
            get {
                AssertIndexOutOfRange(in index);

                return ref _buffer[index];
            }
        }


        /// <summary>
        /// Returns the index of a particular item, if it is in the list.
        /// Returns -1 if the item isn't in the list.
        /// </summary>
        public int IndexOf(in T item) {
            int index = 0;
            while (index < _count) {
                if (_buffer[index].CompareTo(item) == 0) {
                    return index;
                }

                index++;
            }

            return -1;
        }

        /// <summary>
        /// Inserts value into the list at position index.
        /// index must be non-negative and less than or equal to the
        /// number of elements in the list.  If index equals the number
        /// of items in the list, then value is appended to the end.
        /// </summary>
        public void Insert(in int index, in T item) {
            AssertCanInsert(in index);
            InsertWithPushInternal(in index, in item);
        }

        /// <summary>
        /// Removes the item at position index.
        /// </summary>
        public void RemoveAt(in int index) {
            try {
                AssertCanRemove(in index);
                RemoveWithPopInternal(in index);
            } catch {
                // ignored
            }
        }

#endregion

#region Utils

        /// <summary>
        /// Вставляет элемент со сдвигом вправо
        /// </summary>
        private void InsertWithPushInternal(in int sourceIndex, in T sourceValue) {
            var index = sourceIndex;
            var value = sourceValue;

            while (index <= _count) {
                // var tempValue = _buffer[index];
                // _buffer[index] = value;
                // value = tempValue;

                (_buffer[index], value) = (value, _buffer[index]);
                index++;
            }

            _count += 1;
        }

        /// <summary>
        /// Удаляет элемент со сдвигом влево
        /// </summary>
        private T RemoveWithPopInternal(in int sourceIndex) {
            var index = sourceIndex;
            T value = _buffer[sourceIndex];

            while (index < _count) {
                var nextIndex = index + 1;
                _buffer[index] = nextIndex >= _buffer.Length
                    ? default
                    : _buffer[nextIndex];
                index = nextIndex;
            }

            _count -= 1;

            return value;
        }

#endregion

#region Asserts

        private void AssertIndexOutOfRange(in int index) {
            if (index < 0 || index >= _count) {
                throw new ArgumentOutOfRangeException(nameof(index), $"Index '{index}' is out of range!");
            }
        }

        private void AssertCanInsert(in int index) {
            if (_count == _buffer.Length) {
                throw new OverflowException($"Can't insert new values, as Buffer iw overfilled!");
            }

            if (index > _count) {
                throw new ArgumentOutOfRangeException(nameof(index),
                                                      $"Can't insert value at index '{index}' as count of elements is '{_count}'");
            }
        }

        private void AssertCanRemove(in int index) {
            if (_count == 0) {
                throw new OverflowException($"Can't remove element fom EMPTY Buffer!");
            }

            if (index >= _count) {
                throw new ArgumentOutOfRangeException(nameof(index),
                                                      $"Can't remove value at index '{index}' as count of elements is '{_count}'");
            }
        }

#endregion

#region Enumerable

        public Enumerator GetEnumerator() {
            // return _buffer.GetEnumerator();
            return new Enumerator(this);
        }

        public ref struct Enumerator {
            /// <summary>The span being enumerated.</summary>
            private readonly NonAllocatedList<T> _source;

            /// <summary>The next index to yield.</summary>
            private int _index;

            /// <summary>Initialize the enumerator.</summary>
            internal Enumerator(NonAllocatedList<T> source) {
                _source = source;
                _index = -1;
            }

            /// <summary>Advances the enumerator to the next element of the span.</summary>
            public bool MoveNext() {
                int index = _index + 1;
                if (index < _source.Count) {
                    _index = index;

                    return true;
                }

                return false;
            }

            /// <summary>Gets the element at the current position of the enumerator.</summary>
            public ref T Current => ref _source[_index];
        }

#endregion
    }
}