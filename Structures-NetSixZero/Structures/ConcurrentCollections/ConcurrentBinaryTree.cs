using System.Diagnostics.CodeAnalysis;
using Structures.NetSixZero.Structures.BST;
using Structures.NetSixZero.Structures.BST.Utils;
using Structures.NetSixZero.Structures.ConcurrentCollections.Utils;

namespace Structures.NetSixZero.Structures.ConcurrentCollections {
    public class ConcurrentBinaryTree<T> : BinaryTree<T> where T : IComparable<T> {
        private readonly RWLock _rwLock;
        
        public ConcurrentBinaryTree(TimeSpan time) {
            _rwLock = new RWLock(time);
        }

        /// <summary>
        /// Корневой узел дерева
        /// </summary>
        public override Node<T>? Root {
            get {
                using (_rwLock.ReadLock()) {
                    return base.Root;
                }
            }
            private protected set {
                using (_rwLock.WriteLock()) {
                    base.Root = value;
                }
            }
        }

        /// <summary>
        /// Кол-во узлов в дереве
        /// </summary>
        public override int NodesCount {
            get {
                using (_rwLock.ReadLock()) {
                    return base.NodesCount;
                }
            }
            private protected set {
                using (_rwLock.WriteLock()) {
                    base.NodesCount = value;
                }
            }
        }
        
        private protected override bool TrySetLeftValue(Node<T>? value, T data) {
            using (_rwLock.WriteLock()) {
                return base.TrySetLeftValue(value, data);
            }
        }
        
        private protected override bool TrySetRightValue(Node<T>? value, T data) {
            using (_rwLock.WriteLock()) {
                return base.TrySetRightValue(value, data);
            }
        }

        /// <summary>
        /// Пытается добавить новый узел в дерево
        /// </summary>
        /// <param name="data">Данные</param>
        /// <param name="resultNode">Узел, добавленный в случае успеха, или существующий, в случае провала</param>
        /// <returns>Удалось создать новый узел (True) или узел уже  существовал (False)</returns>
        public override bool TryAdd(T data, out Node<T> resultNode) {
            using (_rwLock.UpgradableReadLock()) {
                return base.TryAdd(data, out resultNode);
            }
        }

        // /// <summary>
        // /// Добавляет массив элементов. Построено на предположении, что <paramref name="sourceBuffer"/> упорядочен по возрастнию. 
        // /// </summary>
        // /// <returns>Был ли добавлен хотя бы один элемент</returns>
        // private protected override bool TryAddRangeInternal(T[] sourceBuffer) {
        //     using (_rwLock.UpgradableReadLock()) {
        //         return base.TryAddRangeInternal(sourceBuffer);
        //     }
        // }

        /// <summary>
        /// Ищет минимальный элемент в дереве
        /// </summary>
        /// <param name="resultNode">Узел дерева с минимальными данными</param>
        /// <returns>Найден такой узел (true) или нет (fasle)</returns>
        public override bool TryFindMin(out Node<T>? resultNode) {
            using (_rwLock.ReadLock()) {
                return base.TryFindMin(out resultNode);
            }
        }
        
        /// <summary>
        /// Ищет максимальный элемент в дереве
        /// </summary>
        /// <param name="resultNode">Узел дерева с максимальными данными</param>
        /// <returns>Найден такой узел (true) или нет (fasle)</returns>
        public override bool TryFindMax(out Node<T>? resultNode) {
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
        public override bool TryFind(T data, [MaybeNullWhen(false)] out Node<T> resultNode) {
            using (_rwLock.ReadLock()) {
                return base.TryFind(data, out resultNode);
            }
        }

        /// <summary>
        /// Пытается удалить узел дерева по данным
        /// </summary>
        /// <param name="data">Данные, необходмые удалить из дерева</param>
        /// <returns>True - данные найдены и удалить получилось. False - данные найдены не были </returns>
        public override bool Remove(T data) {
            using (_rwLock.WriteLock()) {
                return base.Remove(data);
            }
        }

        /// <summary>
        /// Пытается извлечь нод с минимальным значением.
        /// </summary>
        /// <param name="resultNode">Извлеченный нод или NULL.</param>
        /// <returns>True - нод был извлечен; False - нет нод.</returns>
        public override bool TryDequeue(out Node<T>? resultNode) {
            using (_rwLock.WriteLock()) {
                return base.TryDequeue(out resultNode);
            }
        }
        
        /// <summary>
        /// Очишает все дерево
        /// </summary>
        public override void Clear() {
            using (_rwLock.WriteLock()) {
                base.Clear();
            }
        }


        /// <summary>
        /// Возвращает минимальную глубину дерева
        /// </summary>
        /// <returns>Минимальная глубина дерева</returns>
        public override int FindMinHeight() {
            using (_rwLock.ReadLock()) {
                return base.FindMinHeight();
            }
        }
        
        /// <summary>
        /// Возвращает максимальную глубину дерева
        /// </summary>
        /// <returns>максимальную глубина дерева</returns>
        public override int FindMaxHeight() {
            using (_rwLock.ReadLock()) {
                return base.FindMaxHeight();
            }
        }

        /// <summary>
        /// сбалансировано ли дерево?
        /// </summary>
        public override bool IsBalanced() {
            using (_rwLock.ReadLock()) {
                return base.IsBalanced();
            }
        }

#region IAnyCollection

        // /// <summary>
        // /// Является ли дерево пустым
        // /// </summary>
        // public override bool IsEmpty {
        //     get {
        //         using (_rwLock.ReadLock()) {
        //             return base.IsEmpty;
        //         }
        //     }
        // }

        // /// <summary>
        // /// Любой элемент из коллекции
        // /// </summary>
        // public override T Any {
        //     get {
        //         using (_rwLock.ReadLock()) {
        //             return base.Any;
        //         }
        //     }
        // }

        // /// <summary>
        // /// Есть ли в коллекции хотя бы один элемент
        // /// </summary>
        // /// <param name="value">Любой элемент, если он существует в коллекции</param>
        // public override bool TryGetAny(out T? value) {
        //     using (_rwLock.ReadLock()) {
        //         return base.TryGetAny(out value);
        //     }
        // }
        
#endregion

#region ICollection

        public override int Count => NodesCount;

        public override bool IsReadOnly => false;

        public override void Add(T item) {
            if (!TryAdd(item, out var node)) {
                throw new ArgumentException($"Node with value '{item}' has been already exists!");
            }
        }

        public override bool Contains(T item) {
            return TryFind(item, out var _);
        }

        public override void CopyTo(T[] array, int arrayIndex) {
            using (_rwLock.ReadLock()) {
                base.CopyTo(array, arrayIndex);
            }
        }

#endregion

#region Enumerable

        public override IEnumerator<T> GetEnumerator() {
            using (_rwLock.ReadLock()) {
                return base.GetEnumerator();
            }
        }

#endregion

#region Dispose

        ~ConcurrentBinaryTree() {
            _rwLock.Dispose();
        }

#endregion
    }
}