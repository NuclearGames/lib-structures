using System.Diagnostics.CodeAnalysis;

namespace Sandbox {
    public ref struct AllocatedStack<T> where T : unmanaged {
        public int Count => _count;
        
        private readonly Span<T> _buffer;
        private int _count;

        public AllocatedStack(ref Span<T> span) {
            _buffer = span;
            _count = 0;
        }

        /// <summary>Returns the object at the top of the <see cref="T:System.Collections.Generic.Stack`1" /> without removing it.</summary>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Collections.Generic.Stack`1" /> is empty.</exception>
        /// <returns>The object at the top of the <see cref="T:System.Collections.Generic.Stack`1" />.</returns>
        public ref T Peek() {
            if (_count == 0) {
                ThrowForOverFilledStack();
            }

            return ref _buffer[_count - 1];
        }

        /// <summary>Removes and returns the object at the top of the <see cref="T:System.Collections.Generic.Stack`1" />.</summary>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The <see cref="T:System.Collections.Generic.Stack`1" /> is empty.</exception>
        /// <returns>The object removed from the top of the <see cref="T:System.Collections.Generic.Stack`1" />.</returns>
        public ref T Pop() {
            if (_count == 0) {
                ThrowForOverFilledStack();
            }

            return ref _buffer[--_count];
        }

        /// <summary>Returns a value that indicates whether there is an object at the top of the <see cref="T:System.Collections.Generic.Stack`1" />, and if one is present, copies it to the <paramref name="result" /> parameter. The object is not removed from the <see cref="T:System.Collections.Generic.Stack`1" />.</summary>
        /// <param name="result">If present, the object at the top of the <see cref="T:System.Collections.Generic.Stack`1" />; otherwise, the default value of <typeparamref name="T" />.</param>
        /// <returns>
        /// <see langword="true" /> if there is an object at the top of the <see cref="T:System.Collections.Generic.Stack`1" />; <see langword="false" /> if the <see cref="T:System.Collections.Generic.Stack`1" /> is empty.</returns>
        public bool TryPeek([MaybeNullWhen(false)] out T result) {
            if (_count == 0) {
                result = default(T);

                return false;
            }

            result = _buffer[_count - 1];

            return true;
        }

        /// <summary>Returns a value that indicates whether there is an object at the top of the <see cref="T:System.Collections.Generic.Stack`1" />, and if one is present, copies it to the <paramref name="result" /> parameter, and removes it from the <see cref="T:System.Collections.Generic.Stack`1" />.</summary>
        /// <param name="result">If present, the object at the top of the <see cref="T:System.Collections.Generic.Stack`1" />; otherwise, the default value of <typeparamref name="T" />.</param>
        /// <returns>
        /// <see langword="true" /> if there is an object at the top of the <see cref="T:System.Collections.Generic.Stack`1" />; <see langword="false" /> if the <see cref="T:System.Collections.Generic.Stack`1" /> is empty.</returns>
        public bool TryPop([MaybeNullWhen(false)] out T result) {
            if (_count == 0) {
                result = default(T);

                return false;
            }

            result = _buffer[--_count];

            return true;
        }

        /// <summary>Inserts an object at the top of the <see cref="T:System.Collections.Generic.Stack`1" />.</summary>
        /// <param name="item">The object to push onto the <see cref="T:System.Collections.Generic.Stack`1" />. The value can be <see langword="null" /> for reference types.</param>
        public void Push(T item) {
            if (_count == _buffer.Length) {
                ThrowForOverFilledStack();
            }

            _buffer[_count++] = item;
        }
        
        /// <summary>Inserts an object at the top of the <see cref="T:System.Collections.Generic.Stack`1" />.</summary>
        /// <param name="item">The object to push onto the <see cref="T:System.Collections.Generic.Stack`1" />. The value can be <see langword="null" /> for reference types.</param>
        public void Push(ref T item) {
            if (_count == _buffer.Length) {
                ThrowForOverFilledStack();
            }

            _buffer[_count++] = item;
        }

        private static void ThrowForOverFilledStack() => throw new ArgumentOutOfRangeException();
    }
}