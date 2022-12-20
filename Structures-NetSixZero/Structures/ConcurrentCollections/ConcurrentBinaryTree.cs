using Structures.NetSixZero.Structures.BST;
using Structures.NetSixZero.Structures.BST.Utils;
using Structures.NetSixZero.Structures.ConcurrentCollections.Utils;

namespace Structures.NetSixZero.Structures.ConcurrentCollections {
    public class ConcurrentBinaryTree<T> : BinaryTree<T> where T : IComparable<T> {
        private readonly RWLock _rwLock;
        
        public ConcurrentBinaryTree(TimeSpan time) {
            _rwLock = new RWLock(time, LockRecursionPolicy.NoRecursion);
        }

        /// <summary>
        /// Корневой узел дерева
        /// </summary>
        public new Node<T>? Root {
            get {
                using (_rwLock.ReadLock()) {
                    return base.Root;
                }
            }
        }

        /// <summary>
        /// Кол-во узлов в дереве
        /// </summary>
        public new int NodesCount {
            get {
                using (_rwLock.ReadLock()) {
                    return base.NodesCount;
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
            using (_rwLock.WriteLock()) {
                return base.TryAdd(data, out resultNode);
            }
        }

        /// <summary>
        /// Добавляет массив элементов. Построено на предположении, что <paramref name="sourceBuffer"/> упорядочен по возрастнию. 
        /// </summary>
        /// <returns>Был ли добавлен хотя бы один элемент</returns>
        public new bool TryAddRange(T[] sourceBuffer) {
            using (_rwLock.WriteLock()) {
                return base.TryAddRange(sourceBuffer);
            }
        }

        /// <summary>
        /// Ищет минимальный элемент в дереве
        /// </summary>
        /// <param name="resultNode">Узел дерева с минимальными данными</param>
        /// <returns>Найден такой узел (true) или нет (fasle)</returns>
        public new bool TryFindMin(out Node<T>? resultNode) {
            using (_rwLock.ReadLock()) {
                return base.TryFindMin(out resultNode);
            }
        }
        
        /// <summary>
        /// Ищет максимальный элемент в дереве
        /// </summary>
        /// <param name="resultNode">Узел дерева с максимальными данными</param>
        /// <returns>Найден такой узел (true) или нет (fasle)</returns>
        public new bool TryFindMax(out Node<T>? resultNode) {
            using (_rwLock.ReadLock()) {
                return base.TryFindMax(out resultNode);
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
            using (_rwLock.ReadLock()) {
                return base.TryFind(data, out resultNode);
            }
        }

        /// <summary>
        /// Пытается удалить узел дерева по данным
        /// </summary>
        /// <param name="data">Данные, необходмые удалить из дерева</param>
        /// <returns>True - данные найдены и удалить получилось. False - данные найдены не были </returns>
        public new bool Remove(T data) {
            using (_rwLock.WriteLock()) {
                return base.Remove(data);
            }
        }

        /// <summary>
        /// Очишает все дерево
        /// </summary>
        public new void Clear() {
            using (_rwLock.WriteLock()) {
                base.Clear();
            }
        }


        /// <summary>
        /// Возвращает минимальную глубину дерева
        /// </summary>
        /// <returns>Минимальная глубина дерева</returns>
        public new int FindMinHeight() {
            using (_rwLock.ReadLock()) {
                return base.FindMinHeight();
            }
        }
        
        /// <summary>
        /// Возвращает максимальную глубину дерева
        /// </summary>
        /// <returns>максимальную глубина дерева</returns>
        public new int FindMaxHeight() {
            using (_rwLock.ReadLock()) {
                return base.FindMaxHeight();
            }
        }

        /// <summary>
        /// сбалансировано ли дерево?
        /// </summary>
        public new bool IsBalanced() {
            using (_rwLock.ReadLock()) {
                return base.IsBalanced();
            }
        }

#region IAnyCollection

        /// <summary>
        /// Является ли дерево пустым
        /// </summary>
        public new bool IsEmpty {
            get {
                using (_rwLock.ReadLock()) {
                    return base.IsEmpty;
                }
            }
        }

        /// <summary>
        /// Любой элемент из коллекции
        /// </summary>
        public new T Any {
            get {
                using (_rwLock.ReadLock()) {
                    return base.Any;
                }
            }
        }

        /// <summary>
        /// Есть ли в коллекции хотя бы один элемент
        /// </summary>
        /// <param name="value">Любой элемент, если он существует в коллекции</param>
        public new bool TryGetAny(out T? value) {
            using (_rwLock.ReadLock()) {
                return base.TryGetAny(out value);
            }
        }
        
#endregion

#region ICollection

        public new int Count => NodesCount;

        public new bool IsReadOnly => false;

        public new void Add(T item) {
            using (_rwLock.WriteLock()) {
                base.Add(item);
            }
        }

        public new bool Contains(T item) {
            using (_rwLock.ReadLock()) {
                return base.Contains(item);
            }
        }

        public new void CopyTo(T[] array, int arrayIndex) {
            using (_rwLock.ReadLock()) {
                base.CopyTo(array, arrayIndex);
            }
        }

#endregion

#region Enumerable

        public new IEnumerator<T> GetEnumerator() {
            using (_rwLock.ReadLock()) {
                return base.GetEnumerator();
            }
        }

#endregion

#region Dispose

        ~ConcurrentBinaryTree() {
            _rwLock?.Dispose();
        }

#endregion
    }
}