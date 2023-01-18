using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using NuclearGames.StructuresUnity.Utils.Collections.Interfaces;

namespace NuclearGames.StructuresUnity.Structures.BCST {
    /// <summary>
    /// Узел двочиного дерева, включающий в себя несколько элементов
    /// </summary>
    public class CollectionNode<TData, TCollection> : IAnyElementCollection<TData> where TCollection : IAnyElementCollection<TData>, new() {

        private readonly TCollection _dataCollection;
        
        /// <summary>
        /// Левый дочерний узел элемента
        /// </summary>
        public CollectionNode<TData, TCollection>? Left { get; set; }
        
        /// <summary>
        /// Правый дочерний узел элемента
        /// </summary>
        public CollectionNode<TData, TCollection>? Right { get; set; }

        public CollectionNode(TData data, CollectionNode<TData, TCollection>? left = null, CollectionNode<TData, TCollection>? right = null) {
            _dataCollection = new TCollection {
                data
            };
            Left = left;
            Right = right;
        }

        public override string ToString() {
            if (!_dataCollection.TryGetAny(out var value)) {
                return $"#-- x {_dataCollection.Count}; L{(Left == null ? "-" : "+")}; R{(Right == null ? "-" : "+")}"; 
            }
            
            return $"#{(value)} x {_dataCollection.Count}; L{(Left == null ? "-" : "+")}; R{(Right == null ? "-" : "+")}";
        }

#region IAny

        /// <summary>
        /// Пустая ли коллекция
        /// </summary>
        public bool IsEmpty => Count == 0;

        /// <summary>
        /// Любой элемент из коллекции
        /// </summary>
        public TData Any => _dataCollection.Any;
        
        /// <summary>
        /// Есть ли в коллекции хотя бы один элемент
        /// </summary>
        /// <param name="value">Любой элемент, если он существует в коллекции</param>
        public bool TryGetAny([CanBeNull] out TData value) {
            return _dataCollection.TryGetAny(out value);
        }
        
        public void CopyTo(IAnyElementCollection<TData> destinationCollection) {
            foreach (var data in _dataCollection) {
                destinationCollection.Add(data);
            }
        }

#endregion

#region ICollection

        public int Count => _dataCollection.Count;
        public bool IsReadOnly => false;

        public void Add(TData item) => _dataCollection.Add(item);

        public void AddRange(IEnumerable<TData> dataEnumerable) {
            foreach (var data in dataEnumerable) {
                _dataCollection.Add(data);
            }
        }

        public void Clear() => _dataCollection.Clear();

        public bool Contains(TData item) => _dataCollection.Contains(item);

        public bool Remove(TData item) => _dataCollection.Remove(item);
        
        void ICollection<TData>.CopyTo(TData[] array, int arrayIndex) => _dataCollection.CopyTo(array, arrayIndex);

#region Enumerable

        public IEnumerator<TData> GetEnumerator() {
            return _dataCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

#endregion

#endregion
    }
}