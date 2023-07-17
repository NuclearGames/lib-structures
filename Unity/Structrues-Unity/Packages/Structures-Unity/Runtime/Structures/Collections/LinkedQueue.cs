using System;
using System.Collections;
using System.Collections.Generic;
using NuclearGames.StructuresUnity.Structures.Collections.Utils;

namespace NuclearGames.StructuresUnity.Structures.Collections {
    public class LinkedQueue<T> : ICollection, IEnumerable<T> {
        /// <summary>
        /// Первый элемент в очереди
        /// </summary>
        public LinkedQueueNode<T>? First => _head;
        
        /// <summary>
        /// Последний элемент в очереди
        /// </summary>
        public LinkedQueueNode<T>? Last => _tail;
        
        private LinkedQueueNode<T>? _head = null;
        private LinkedQueueNode<T>? _tail = null;
        
#region ctors

        public LinkedQueue() { }

        public LinkedQueue(T value) {
            Enqueue(value);
        }

        public LinkedQueue(IEnumerable<T> values) {
            foreach (var value in values) {
                Enqueue(value);
            }
        }

        public LinkedQueue(int capacity, T defaultValue = default) {
            for (int i = 0; i < capacity; i++) {
                Enqueue(defaultValue);
            }
        }

#endregion
        
        /// <summary>
        /// Добавляет элемент в конец однонаправленного связного спика
        /// </summary>
        public void Enqueue(T value) {
            var node = GetNode(value);
            if (Count == 0) {
                _head = node;
                _tail = node;
            } else {
                _tail!.NextNode = node;
                _tail = node;
            }

            Count += 1;
        }

        /// <summary>
        /// Пытается получить следующий элемент из очереди (с удалением из очереди)
        /// <returns>FALSE - если список пустой</returns>
        /// </summary>
        public bool TryDequeue(out T value) {
            value = default;
            
            if (Count == 0) {
                return false;
            }

            var removeNode = _head!;
            value = removeNode.Value;

            _head = removeNode.NextNode;
            if (--Count == 0) {
                _tail = null;
            }

            ReleaseNode(removeNode);
            return true;
        }
        
        /// <summary>
        /// Пытается получить следующий элемент из очереди (с удалением из очереди) сразу вместе с добавлением нового элемнета в очередь
        /// </summary>
        /// <param name="newValue">Новое значение</param>
        /// <param name="dequeuedValue">Значение, которое попытались достать</param>
        /// <returns>TRUE - если удалось достать значение <paramref name="dequeuedValue"/></returns>
        public bool TryEnqueueWithDequeue(in T newValue, out T dequeuedValue) {
            dequeuedValue = default;
            
            if (Count == 0) {
                Enqueue(newValue);
                return false;
            }

            dequeuedValue = _head!.Value;
            _head.Value = newValue;
            if (Count == 1) {
                return true;
            }
            
            var currentNode = _head;
            _head = currentNode.NextNode;
            _tail!.NextNode = currentNode;
            _tail = currentNode;

            return true;
        }

        protected virtual LinkedQueueNode<T> GetNode(T value) {
            return new LinkedQueueNode<T>(this, value);
        }

        protected virtual void ReleaseNode(LinkedQueueNode<T> node) { }

#region Inherits
        bool ICollection.IsSynchronized => false;
        
#nullable enable
        object ICollection.SyncRoot => this;
        
#nullable disable
        
        public int Count { get; private set; }
        
        public void CopyTo(Array array, int index) {
            if (array.Length < Count + index) {
                throw new ArgumentOutOfRangeException(nameof(array), "Invalid size!");
            }

            var node = _head;
            for (int i = 0; i < Count; i++) {
                array.SetValue(node!.Value, i + index);
                node = node.NextNode;
            }
        }
        
        
#region Enumerable
        
        public struct Enumerator : IEnumerator<T> {
            
            /// <summary>
            /// Current element
            /// </summary>
            object IEnumerator.Current => Current;
            
            /// <summary>
            /// Current element
            /// </summary>
            public T Current { get; private set; }

            
            private readonly LinkedQueue<T> _source; 
            private LinkedQueueNode<T> _currentNode;

            public Enumerator(LinkedQueue<T> source) {
                _source = source;
                
                _currentNode = _source.First;
                Current = _currentNode != null ? _currentNode.Value : default;
            }

            /// <summary>
            /// Try Move to next element
            /// </summary>
            /// <returns></returns>
            /// <exception cref="NotImplementedException"></exception>
            public bool MoveNext() {
                if (_currentNode?.Next == null) {
                    return false;
                }
                
                _currentNode = _currentNode.NextNode;
                Current = _currentNode.Value;
                
                return true;
            }

            public void Reset() {
                _currentNode = _source.First;
                Current = _currentNode != null ? _currentNode.Value : default;
            }

            public void Dispose() { }
        }

        public IEnumerator<T> GetEnumerator() {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        
        public static Enumerator GetEnumerator(LinkedQueue<T> source) {
            return new Enumerator(source);
        }

#endregion
        
#endregion
    }
}