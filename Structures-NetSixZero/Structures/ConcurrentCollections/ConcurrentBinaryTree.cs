using Structures.NetSixZero.Structures.BST;
using Structures.NetSixZero.Structures.BST.Utils;

namespace Structures.NetSixZero.Structures.ConcurrentCollections {
    public class ConcurrentBinaryTree<T> : BinaryTree<T> where T : IComparable<T> {
        private readonly ReaderWriterLockSlim _lock = new(LockRecursionPolicy.NoRecursion);
        private readonly TimeSpan _time;
        
        public ConcurrentBinaryTree(TimeSpan time) {
            _time = time;
        }

        /// <summary>
        /// Корневой узел дерева
        /// </summary>
        public new Node<T>? Root {
            get {
                _lock.TryEnterReadLock(_time);
                try {
                    return base.Root;
                }
                finally {
                    if (_lock.IsReadLockHeld) {
                        _lock.ExitReadLock();
                    }
                }
            }
        }

        /// <summary>
        /// Кол-во узлов в дереве
        /// </summary>
        public new int NodesCount {
            get {
                _lock.TryEnterReadLock(_time);
                try {
                    return base.NodesCount;
                }
                finally {
                    if (_lock.IsReadLockHeld) {
                        _lock.ExitReadLock();
                    }
                }
            }
        }

        /// <summary>
        /// Пытается добавить новый узел в дерево
        /// </summary>
        /// <param name="data">Данные</param>
        /// <param name="resultNode">Узел, добавленный в случае успеха, или существующий, в случае провала</param>
        /// <returns>Удалось создать новый узел (True) или узел уже  существовал (False)</returns>
        public new bool TryAdd(T data, out Node<T> resultNode) {
            _lock.TryEnterWriteLock(_time);
            try {
                return base.TryAdd(data, out resultNode);
            }
            finally {
                if (_lock.IsWriteLockHeld) {
                    _lock.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// Добавляет массив элементов. Построено на предположении, что <paramref name="sourceBuffer"/> упорядочен по возрастнию. 
        /// </summary>
        /// <returns>Был ли добавлен хотя бы один элемент</returns>
        public new bool TryAddRange(T[] sourceBuffer) {
            _lock.TryEnterWriteLock(_time);
            try {
                return base.TryAddRange(sourceBuffer);
            }
            finally {
                if (_lock.IsWriteLockHeld) {
                    _lock.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// Ищет минимальный элемент в дереве
        /// </summary>
        /// <param name="resultNode">Узел дерева с минимальными данными</param>
        /// <returns>Найден такой узел (true) или нет (fasle)</returns>
        public new bool TryFindMin(out Node<T>? resultNode) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.TryFindMin(out resultNode);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }
        
        /// <summary>
        /// Ищет максимальный элемент в дереве
        /// </summary>
        /// <param name="resultNode">Узел дерева с максимальными данными</param>
        /// <returns>Найден такой узел (true) или нет (fasle)</returns>
        public new bool TryFindMax(out Node<T>? resultNode) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.TryFindMax(out resultNode);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// Пытается найти узел по данным
        /// </summary>
        /// <param name="data">Данные поиска</param>
        /// <param name="resultNode">Узел с данными (если был найден) или null</param>
        /// <returns>True, если узел найден; False - если узел не найден</returns>
        /// <exception cref="ArgumentNullException">Недопустимая ошибка сравнения при обходе дерева</exception>
        public new bool TryFind(T data, out Node<T>? resultNode) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.TryFind(data, out resultNode);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// Пытается удалить узел дерева по данным
        /// </summary>
        /// <param name="data">Данные, необходмые удалить из дерева</param>
        /// <returns>True - данные найдены и удалить получилось. False - данные найдены не были </returns>
        public new bool Remove(T data) {
            _lock.TryEnterUpgradeableReadLock(_time);
            _lock.TryEnterWriteLock(_time);
            try {
                return base.Remove(data);
            }
            finally {
                if (_lock.IsUpgradeableReadLockHeld) {
                    _lock.ExitUpgradeableReadLock();
                }
                if (_lock.IsWriteLockHeld) {
                    _lock.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// Очишает все дерево
        /// </summary>
        public new void Clear() {
            _lock.TryEnterWriteLock(_time);
            try {
                base.Clear();
            }
            finally {
                if (_lock.IsWriteLockHeld) {
                    _lock.ExitWriteLock();
                }
            }
        }


        /// <summary>
        /// Возвращает минимальную глубину дерева
        /// </summary>
        /// <returns>Минимальная глубина дерева</returns>
        public new int FindMinHeight() {
            _lock.TryEnterReadLock(_time);
            try {
                return base.FindMinHeight();
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }
        
        /// <summary>
        /// Возвращает максимальную глубину дерева
        /// </summary>
        /// <returns>максимальную глубина дерева</returns>
        public new int FindMaxHeight() {
            _lock.TryEnterReadLock(_time);
            try {
                return base.FindMaxHeight();
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// сбалансировано ли дерево?
        /// </summary>
        public new bool IsBalanced() {
            _lock.TryEnterReadLock(_time);
            try {
                return base.IsBalanced();
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

#region IAnyCollection

        /// <summary>
        /// Является ли дерево пустым
        /// </summary>
        public new bool IsEmpty {
            get {
                _lock.TryEnterReadLock(_time);
                try {
                    return base.IsEmpty;
                }
                finally {
                    if (_lock.IsReadLockHeld) {
                        _lock.ExitReadLock();
                    }
                }
            }
        }

        /// <summary>
        /// Любой элемент из коллекции
        /// </summary>
        public new T Any {
            get {
                if (!_lock.TryEnterReadLock(_time)){}
                try {
                    return base.Any;
                }
                finally {
                    if (_lock.IsReadLockHeld) {
                        _lock.ExitReadLock();
                    }
                }
            }
        }

        /// <summary>
        /// Есть ли в коллекции хотя бы один элемент
        /// </summary>
        /// <param name="value">Любой элемент, если он существует в коллекции</param>
        public new bool TryGetAny(out T? value) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.TryGetAny(out value);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }
        
#endregion

#region ICollection

        public new int Count => NodesCount;

        public new bool IsReadOnly => false;

        public new void Add(T item) {
            _lock.TryEnterWriteLock(_time);
            try {
                base.Add(item);
            }
            finally {
                if (_lock.IsWriteLockHeld) {
                    _lock.ExitWriteLock();
                }
            }
        }

        public new bool Contains(T item) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.Contains(item);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

        public new void CopyTo(T[] array, int arrayIndex) {
            _lock.TryEnterReadLock(_time);
            try {
                base.CopyTo(array, arrayIndex);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

#endregion

#region Enumerable

        public new IEnumerator<T> GetEnumerator() {
            _lock.TryEnterReadLock(_time);
            try {
                return base.GetEnumerator();
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

#endregion

#region Dispose

        ~ConcurrentBinaryTree() {
            _lock.Dispose();
        }

#endregion
    }
}