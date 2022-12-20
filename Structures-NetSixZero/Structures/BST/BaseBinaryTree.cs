using System;
using System.Collections;
using System.Collections.Generic;
using Structures.NetSixZero.Structures.BST.Utils;
using Structures.NetSixZero.Utils.Collections.Interfaces;


namespace Structures.NetSixZero.Structures.BST {
    /// <summary>
    /// Базовая структура Двоичного дерева, где данные типа <typeparamref name="TData"/> отсортированы по ключу <typeparamref name="TComparable"/>
    /// </summary>
    public class BaseBinaryTree<TData, TComparable>: IAnyElementCollection<TData> where TComparable : IComparable<TComparable> {
        /// <summary>
        /// Корневой узел дерева
        /// </summary>
        public Node<TData>? Root { get; private set; }

        /// <summary>
        /// Кол-во узлов в дереве
        /// </summary>
        public int NodesCount { get; private set; }

        /// <summary>
        /// Селектор поля сравнения из объекта данные
        /// </summary>
        public Func<TData, TComparable> CompareFieldSelector { get; }

        public BaseBinaryTree(Func<TData, TComparable> compareFieldSelector) {
            CompareFieldSelector = compareFieldSelector;
        }

        public BaseBinaryTree(TData[] sourceBuffer, Func<TData, TComparable> compareFieldSelector) : this(compareFieldSelector) {
            TryAddRange(sourceBuffer);
        }

        /// <summary>
        /// Пытается добавить новый узел в дерево
        /// </summary>
        /// <param name="data">Данные</param>
        /// <param name="resultNode">Узел, добавленный в случае успеха, или существующий, в случае провала</param>
        /// <returns>Удалось создать новый узел (True) или узел уже  существовал (False)</returns>
        public bool TryAdd(TData data, out Node<TData> resultNode) {
            if (IsEmpty) {
                Root = GetNode(data);
                resultNode = Root;
                NodesCount++;
                return true;
            }

            var node = Root!;
            TComparable dataValue = CompareFieldSelector(data);

            bool SearchTree(Node<TData> currentNode, out Node<TData> resultNodeInternal) {
                var compareResult = Compare(dataValue, currentNode.Data);
                switch (compareResult) {
                    case < 0:
                        if (currentNode.Left == null) {
                            currentNode.Left = GetNode(data);
                            resultNodeInternal = currentNode.Left;
                            return true;
                        }
                        return SearchTree(currentNode.Left, out resultNodeInternal);
                    case > 0:
                        if (currentNode.Right == null) {
                            currentNode.Right = GetNode(data);
                            resultNodeInternal = currentNode.Right;
                            return true;
                        }
                        return SearchTree(currentNode.Right, out resultNodeInternal);
                    default:
                        resultNodeInternal = currentNode;
                        return false;   
                }
            }
            
            var result = SearchTree(node, out resultNode);
            if (result) {
                NodesCount++;
            }

            return result;
        }

        /// <summary>
        /// Добавляет массив элементов. Построено на предположении, что <paramref name="sourceBuffer"/> упорядочен по возрастнию. 
        /// </summary>
        /// <returns>Был ли добавлен хотя бы один элемент</returns>
        public bool TryAddRange(TData[] sourceBuffer) {
            bool anyAdd = false;
            void AddElement(int pointIndex, int window) {
                if (pointIndex < 0 || pointIndex >= sourceBuffer.Length) {
                    return;
                }
                
                anyAdd |= TryAdd(sourceBuffer[pointIndex], out _);
                if (window == 1) {
                    return;
                }
                
                var newWindow = window / 2;
                
                if (window % 2 == 0) {
                    AddElement(pointIndex + newWindow, newWindow);
                    AddElement(pointIndex - newWindow, newWindow);
                } else {
                    anyAdd |= TryAdd(sourceBuffer[pointIndex + newWindow * 2], out _);
                    anyAdd |= TryAdd(sourceBuffer[pointIndex -1], out _);
                    
                    AddElement(pointIndex + newWindow, newWindow);
                    AddElement(pointIndex - newWindow - 1, newWindow);
                }
            }

            switch (sourceBuffer.Length) {
                case 0:
                    return false;
                case 1:
                    anyAdd |= TryAdd(sourceBuffer[0], out _);
                    break;
                default:
                    AddElement(sourceBuffer.Length / 2, sourceBuffer.Length / 2);
                    anyAdd |= TryAdd(sourceBuffer[0], out _);
                    if (sourceBuffer.Length % 2 == 1) {
                        anyAdd |= TryAdd(sourceBuffer[^1], out _);
                    }
                    break;
            }
            return anyAdd;
        }

        /// <summary>
        /// Ищет минимальный элемент в дереве
        /// </summary>
        /// <param name="resultNode">Узел дерева с минимальными данными</param>
        /// <returns>Найден такой узел (true) или нет (fasle)</returns>
        public bool TryFindMin(out Node<TData>? resultNode) {
            if (IsEmpty) {
                resultNode = null;
                return false;
            }
            
            resultNode = Root!;
            while (resultNode.Left != null) {
                resultNode = resultNode.Left;
            }

            return true;
        }
        
        /// <summary>
        /// Ищет максимальный элемент в дереве
        /// </summary>
        /// <param name="resultNode">Узел дерева с максимальными данными</param>
        /// <returns>Найден такой узел (true) или нет (fasle)</returns>
        public bool TryFindMax(out Node<TData>? resultNode) {
            if (IsEmpty) {
                resultNode = null;
                return false;
            }
            
            resultNode = Root!;
            while (resultNode.Right != null) {
                resultNode = resultNode.Right;
            }

            return true;
        }

        /// <summary>
        /// Пытается найти узел по данным
        /// </summary>
        /// <param name="data">Данные поиска</param>
        /// <param name="resultNode">Узел с данными (если был найден) или null</param>
        /// <returns>True, если узел найден; False - если узел не найден</returns>
        /// <exception cref="ArgumentNullException">Недопустимая ошибка сравнения при обходе дерева</exception>
        public bool TryFind(TData data, out Node<TData>? resultNode) {
            if (IsEmpty) {
                resultNode = null;
                return false;
            }

            resultNode = Root!;
            TComparable dataValue = CompareFieldSelector(data);
            int compareResult;
            while ((compareResult = Compare(dataValue, resultNode.Data)) != 0) {
                switch (compareResult) {
                    case < 0 :
                        resultNode = resultNode.Left;
                        break;
                    case > 0:
                        resultNode = resultNode.Right;
                        break;
                    default:
                        throw new ArgumentNullException(nameof(resultNode));
                }

                if (resultNode == null) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Пытается удалить узел дерева по данным
        /// </summary>
        /// <param name="data">Данные, необходмые удалить из дерева</param>
        /// <returns>True - данные найдены и удалить получилось. False - данные найдены не были </returns>
        public bool Remove(TData data) {
            bool TryRemoveInternal(Node<TData>? currentNode, TData internalData, out Node<TData>? internalResultNode) {
                if (currentNode == null) {
                    internalResultNode = null;
                    return false;
                }

                var compareResult = Compare(internalData, currentNode.Data);
                bool internalResult;
                Node<TData>? replaceNode;
                switch (compareResult) {
                    case < 0 : {
                        internalResult = TryRemoveInternal(currentNode.Left, internalData, out replaceNode);
                        currentNode.Left = replaceNode;
                        internalResultNode = currentNode;

                        return internalResult;
                    }
                    case > 0 : {
                        internalResult = TryRemoveInternal(currentNode.Right, internalData, out replaceNode);
                        currentNode.Right = replaceNode;
                        internalResultNode = currentNode;

                        return internalResult;
                    }
                    default: {
                        // node has no children
                        if (currentNode.Left == null && currentNode.Right == null) {
                            internalResultNode = null;
                            ReleaseNode(currentNode);
                            return true;
                        }

                        // node has no left child
                        if (currentNode.Left == null) {
                            internalResultNode = currentNode.Right;
                            ReleaseNode(currentNode);
                            return true;
                        }

                        // node has no right child
                        if (currentNode.Right == null) {
                            internalResultNode = currentNode.Left;
                            ReleaseNode(currentNode);
                            return true;
                        }

                        // node has two children
                        var tempNode = currentNode.Right;
                        while (tempNode.Left != null) {
                            tempNode = tempNode.Left;
                        }

                        currentNode.Data = tempNode.Data;
                        internalResultNode = currentNode;

                        internalResult = TryRemoveInternal(currentNode.Right, tempNode.Data, out replaceNode);
                        currentNode.Right = replaceNode;

                        ReleaseNode(tempNode);

                        return internalResult;
                    }
                }
            }

            var result = TryRemoveInternal(Root, data, out var resultNode);
            Root = resultNode;
            if (result) {
                NodesCount--;
            }
            
            return result;
        }

        /// <summary>
        /// Пытается извлечь нод с минимальным значением.
        /// </summary>
        /// <param name="resultNode">Извлеченный нод или NULL.</param>
        /// <returns>True - нод был извлечен; False - нет нод.</returns>
        public bool TryDequeue(out Node<TData>? resultNode) {
            if (IsEmpty) {
                resultNode = null;
                return false;
            }

            if(Root == null) {
                Console.WriteLine($"null: {IsEmpty}, {NodesCount}");
            }

            // Добегаем до последнего левого нода (минимальное значение).
            Node<TData>? prev = null;
            Node<TData> current = Root!;
            while(current.Left != null) {
                prev = current;
                current = current.Left;
            }

            if (prev == null) {
                // Делаем правый нод корня новым корневым.
                // Если правого нода не было - прекинется NULL.
                Root = current.Right; 
            } else {
                // Перекидываем правый нод наименьшего на его родителя.
                // Если правого нода не было - прекинется NULL.
                prev.Left = current.Right;
            }
            NodesCount--;

            ReleaseNode(current);

            resultNode = current;
            return true;
        }

        /// <summary>
        /// Очишает все дерево
        /// </summary>
        public void Clear() {
            Root = null;
            NodesCount = 0;
        }


        /// <summary>
        /// Возвращает минимальную глубину дерева
        /// </summary>
        /// <returns>Минимальная глубина дерева</returns>
        public int FindMinHeight() {
            int FindMinHeightInternal(Node<TData>? node) {
                if (node == null) {
                    return -1;
                }
                var left = FindMinHeightInternal(node.Left);
                var right = FindMinHeightInternal(node.Right);
                if (left < right) {
                    return left + 1;
                }
                return right + 1;
            }

            return FindMinHeightInternal(Root);
        }
        
        /// <summary>
        /// Возвращает максимальную глубину дерева
        /// </summary>
        /// <returns>максимальную глубина дерева</returns>
        public int FindMaxHeight() {
            int FindMaxHeightInternal(Node<TData>? node) {
                if (node == null) {
                    return -1;
                }
                var left = FindMaxHeightInternal(node.Left);
                var right = FindMaxHeightInternal(node.Right);
                if (left > right) {
                    return left + 1;
                }
                return right + 1;
            }

            return FindMaxHeightInternal(Root);
        }

        /// <summary>
        /// сбалансировано ли дерево?
        /// </summary>
        public bool IsBalanced() => FindMinHeight() >= FindMaxHeight() - 1;

        protected virtual Node<TData> GetNode(TData data) {
            return new Node<TData>(data);
        }

        protected virtual void ReleaseNode(Node<TData> node) { }

#region IAnyCollection
        
        /// <summary>
        /// Является ли дерево пустым
        /// </summary>
        public bool IsEmpty => NodesCount == 0;

        /// <summary>
        /// Любой элемент из коллекции
        /// </summary>
        public TData Any {
            get {
                if (IsEmpty) {
                    throw new NullReferenceException("Tree is empty!");
                }

                return Root!.Data;
            }
        }

        /// <summary>
        /// Есть ли в коллекции хотя бы один элемент
        /// </summary>
        /// <param name="value">Любой элемент, если он существует в коллекции</param>
        public bool TryGetAny(out TData? value) {
            if (NodesCount == 0) {
                value = default;
                return false;   
            }

            value = Root!.Data;
            return true;
        }
        
#endregion

#region Utils

        private int Compare(TComparable value1, TData data2) {
            return value1.CompareTo(CompareFieldSelector(data2));
        }

        private int Compare(TData data1, TData data2) {
            return CompareFieldSelector(data1).CompareTo(CompareFieldSelector(data2));
        }

#endregion
        
#region ICollection

        public int Count => NodesCount;

        public bool IsReadOnly => false;

        public void Add(TData item) {
            if (!TryAdd(item, out var node)) {
                throw new Exception($"Node with value '{item}' has been already exists!");
            }
        }

        public bool Contains(TData item) {
            return TryFind(item, out var _);
        }

        public void CopyTo(TData[] array, int arrayIndex) {
            if (array.Length < Count + arrayIndex) {
                throw new ArgumentOutOfRangeException(nameof(array), $"Invalid array size: it's length should be >= '{Count + arrayIndex}'");
            }

            var index = 0;
            foreach (var node in this) {
                array[index + arrayIndex] = node;
                index++;
            }
        }

#endregion

#region Enumerable

        public IEnumerator<TData> GetEnumerator() {
            if (IsEmpty) {
                yield break;
            }
            
            IEnumerator<TData> EnumerateTreeInternal(Node<TData> currentNode) {
                if (currentNode == null) throw new ArgumentNullException(nameof(currentNode));
                if (currentNode.Left != null) {
                    var internalEnumerator = EnumerateTreeInternal(currentNode.Left);
                    while (internalEnumerator.MoveNext()) {
                        yield return internalEnumerator.Current;
                    }
                }

                yield return currentNode.Data;
                
                if (currentNode.Right != null) {
                    var internalEnumerator = EnumerateTreeInternal(currentNode.Right);
                    while (internalEnumerator.MoveNext()) {
                        yield return internalEnumerator.Current;
                    }
                } 
            }

            var enumerator = EnumerateTreeInternal(Root!);
            while (enumerator.MoveNext()) {
                yield return enumerator.Current;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

#endregion
    }
}