using System;
using System.Collections;
using System.Collections.Generic;
using Structures.NetSixZero.Structures.Collections.Utils;


namespace Structures.NetSixZero.Structures.Collections {
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

#endregion
        
        /// <summary>
        /// Добавляет элемент в конец однонаправленного связного спика
        /// </summary>
        public void Enqueue(T value) {
            var node = new LinkedQueueNode<T>(this, value);
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
        public bool TryDequeue(out T? value) {
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
            
            return true;
        }

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

        public IEnumerator<T> GetEnumerator() {
            var node = _head;
            while (node != null) {
                yield return node.Value;
                node = node.NextNode;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

#endregion
        
#endregion
    }
}