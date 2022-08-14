using System;
using System.Collections;
using System.Collections.Generic;
using LogicUtils.Structures.BCST;
using Structures.NetStandard.Utils.Collections.Interfaces;


namespace Structures.NetStandard.Structures.BCST {
    public class BinaryCollectionTree<TData, TComparable, TCollection> : IEnumerable<TData> 
        where TComparable : IComparable<TComparable> 
        where TCollection : IAnyElementCollection<TData>, new() {
        
        /// <summary>
        /// Кол-во узлов в дереве
        /// </summary>
        public int NodesCount { get; private set; }
        
        /// <summary>
        /// Кол-во элементов в дереве
        /// </summary>
        public long DataCount { get; private set; }
        
        /// <summary>
        /// Корневой узел дерева
        /// </summary>
        public CollectionNode<TData, TCollection>? Root { get; private set; }

        /// <summary>
        /// Является ли дерево пустым
        /// </summary>
        public bool IsEmpty() => ReferenceEquals(Root, null);

        private readonly Func<TData, TComparable> _compareFieldSelector;

        public BinaryCollectionTree(Func<TData, TComparable> compareFieldSelector) {
            _compareFieldSelector = compareFieldSelector;
        }

        public BinaryCollectionTree(Func<TData, TComparable> compareFieldSelector, TData[] sourceBuffer) {
            _compareFieldSelector = compareFieldSelector;

            AddRange(sourceBuffer);
        }

        /// <summary>
        /// Добавляет данные в дерево
        /// <para>Если подходящий узел с данными уже существует, то данные добавляются в коллекцию этого узла</para>
        /// </summary>
        public void Add(TData data, out CollectionNode<TData, TCollection> resultNode) {
            if (IsEmpty()) {
                Root = new CollectionNode<TData, TCollection>(data);
                resultNode = Root;
                NodesCount++;
                DataCount++;
                
                return;
            }

            var node = Root!;

            void SearchTree(CollectionNode<TData, TCollection> currentNode, out CollectionNode<TData, TCollection> resultNodeInternal) {
                var compareResult = Compare(data, currentNode);
                if (compareResult < 0) {
                    if (currentNode.Left == null) {
                        currentNode.Left = new CollectionNode<TData, TCollection>(data);
                        resultNodeInternal = currentNode.Left;
                        NodesCount++;
                    } else {
                        SearchTree(currentNode.Left, out resultNodeInternal);
                    }
                } else if (compareResult > 0) {
                    if (currentNode.Right == null) {
                        currentNode.Right = new CollectionNode<TData, TCollection>(data);
                        resultNodeInternal = currentNode.Right;
                        NodesCount++;
                    } else {
                        SearchTree(currentNode.Right, out resultNodeInternal);
                    }
                } else {
                    currentNode.Add(data);
                    resultNodeInternal = currentNode;
                }
            }

            SearchTree(node, out resultNode);
            DataCount++;
        }

        /// <summary>
        /// Добавляет массив элементов. Построено на предположении, что <paramref name="sourceBuffer"/> упорядочен по возрастнию. 
        /// </summary>
        /// <returns>Был ли добавлен хотя бы один элемент</returns>
        public void AddRange(TData[] sourceBuffer) {
            void AddElement(int pointIndex, int window) {
                if (pointIndex < 0 || pointIndex >= sourceBuffer.Length) {
                    return;
                }

                Add(sourceBuffer[pointIndex], out _);
                if (window == 1) {
                    return;
                }

                var newWindow = window / 2;

                if (window % 2 == 0) {
                    AddElement(pointIndex + newWindow, newWindow);
                    AddElement(pointIndex - newWindow, newWindow);
                } else {
                    Add(sourceBuffer[pointIndex + newWindow * 2], out _);
                    Add(sourceBuffer[pointIndex - 1], out _);

                    AddElement(pointIndex + newWindow, newWindow);
                    AddElement(pointIndex - newWindow - 1, newWindow);
                }
            }

            AddElement(sourceBuffer.Length / 2, sourceBuffer.Length / 2);
            Add(sourceBuffer[0], out _);
            if (sourceBuffer.Length % 2 == 1) {
                Add(sourceBuffer[^1], out _);
            }
        }

        /// <summary>
        /// Ищет минимальный элемент в дереве
        /// </summary>
        /// <param name="resultNode">Узел дерева с минимальными данными</param>
        /// <returns>Найден такой узел (true) или нет (fasle)</returns>
        public bool TryFindMin(out CollectionNode<TData, TCollection>? resultNode) {
            if (IsEmpty()) {
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
        public bool TryFindMax(out CollectionNode<TData, TCollection>? resultNode) {
            if (IsEmpty()) {
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
        public bool TryFind(TData data, out CollectionNode<TData, TCollection>? resultNode) {
            if (IsEmpty()) {
                resultNode = null;

                return false;
            }

            resultNode = Root!;
            int compareResult;
            while ((compareResult = Compare(data, resultNode)) != 0) {
                if (compareResult < 0) {
                    resultNode = resultNode.Left;
                } else if (compareResult > 0) {
                    resultNode = resultNode.Right;
                } else {
                    throw new ArgumentNullException(nameof(resultNode));
                }

                if (resultNode == null) {
                    return false;
                }
            }

            return true;
        }
        
        /// <summary>
        /// Пытается найти узел по ключу сравнения
        /// </summary>
        /// <param name="comparableKey">Ключ сравнения</param>
        /// <param name="resultNode">Узел с данными (если был найден) или null</param>
        /// <returns>True, если узел найден; False - если узел не найден</returns>
        /// <exception cref="ArgumentNullException">Недопустимая ошибка сравнения при обходе дерева</exception>
        public bool TryFindByKey(TComparable comparableKey, out CollectionNode<TData, TCollection>? resultNode) {
            if (IsEmpty()) {
                resultNode = null;

                return false;
            }

            resultNode = Root!;
            int compareResult;
            while ((compareResult = Compare(comparableKey, resultNode)) != 0) {
                if (compareResult < 0) {
                    resultNode = resultNode.Left;
                } else if (compareResult > 0) {
                    resultNode = resultNode.Right;
                } else {
                    throw new ArgumentNullException(nameof(resultNode));
                }

                if (resultNode == null) {
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// Пытается удалить данные из дерева.
        /// <para>Если после удаления данных из узла других данных в нем не остается - удаляет узел</para>
        /// <param name="data">Данные поиска</param>
        /// </summary>
        public bool TryRemove(TData data) {
            // Удаляет данные из узла и блокирует удаление узла, если в узле остались другие данные
            bool InterruptRemoveCondition(CollectionNode<TData, TCollection> node) {
                if (node.Remove(data)) {
                    DataCount--;
                }
                return node.Count != 0;
            }

            var result = TryRemoveInternal(Root, _compareFieldSelector(data), InterruptRemoveCondition, out var resultNode);
            Root = resultNode;

            return result;
        }

        /// <summary>
        /// Пытается удалить узел из дерева.
        /// <param name="comparableKey">Ключ поиска по дереву</param>
        /// </summary>
        public bool TryRemoveNode(TComparable comparableKey) {
            var result = TryRemoveInternal(Root, comparableKey, null, out var resultNode);
            Root = resultNode;

            return result;
        }
        
        /// <summary>
        /// Удаляет узел <paramref name="node"/> по ключу <paramref name="comparableKey"/>, если условие <paramref name="interruptRemoveCondition"/> не выполняется
        /// </summary>
        bool TryRemoveInternal(CollectionNode<TData, TCollection>? node, TComparable comparableKey, Func<CollectionNode<TData, TCollection>, bool>? interruptRemoveCondition, out CollectionNode<TData, TCollection>? resultNode, bool isRecursiveRemove = false) {
            if (node == null) {
                resultNode = null;

                return false;
            }

            var compareResult = comparableKey.CompareTo(_compareFieldSelector(node.Any));
            bool internalResult;
            CollectionNode<TData, TCollection>? replaceNode;

            if (compareResult < 0) {
                internalResult = TryRemoveInternal(node.Left, comparableKey, interruptRemoveCondition, out replaceNode, isRecursiveRemove);
                node.Left = replaceNode;
                resultNode = node;

                return internalResult;
            }

            if (compareResult > 0) {
                internalResult = TryRemoveInternal(node.Right, comparableKey, interruptRemoveCondition, out replaceNode, isRecursiveRemove);
                node.Right = replaceNode;
                resultNode = node;

                return internalResult;
            }

            if (interruptRemoveCondition == null) {
                if(!isRecursiveRemove) {
                    DataCount -= node.Count;
                }
            } else if (interruptRemoveCondition.Invoke(node)) {
                resultNode = node;
                return true;
            }
                    
            // node has no children
            if (node.Left == null && node.Right == null) {
                resultNode = null;
                NodesCount--;
                return true;
            }

            // node has no left child
            if (node.Left == null) {
                resultNode = node.Right;
                NodesCount--;
                return true;
            }

            // node has no right child
            if (node.Right == null) {
                resultNode = node.Left;
                NodesCount--;
                return true;
            }

            // node has two children
            var tempNode = node.Right;
            while (tempNode.Left != null) {
                tempNode = tempNode.Left;
            }

            node.Clear();
            node.AddRange(tempNode);
            resultNode = node;

            internalResult = TryRemoveInternal(
                                               node.Right, 
                                               _compareFieldSelector(tempNode.Any), 
                                               null, 
                                               out replaceNode, true);
            node.Right = replaceNode;

            return internalResult;
        }

        /// <summary>
        /// Очишает все дерево
        /// </summary>
        public void Clear() {
            Root = null;
            NodesCount = 0;
            DataCount = 0;
        }

        /// <summary>
        /// Возвращает минимальную глубину дерева
        /// </summary>
        /// <returns>Минимальная глубина дерева</returns>
        public int FindMinHeight() {
            int FindMinHeightInternal(CollectionNode<TData, TCollection>? node) {
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
            int FindMaxHeightInternal(CollectionNode<TData, TCollection>? node) {
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

#region Utils

        // private int Compare(TData source, TData to) {
        //     return _compareFieldSelector(source).CompareTo(_compareFieldSelector(to));
        // }
        
        private int Compare(TData source, IAnyElementCollection<TData> to) {
            if (to.TryGetAny(out var toData)) {
                return _compareFieldSelector(source).CompareTo(_compareFieldSelector(toData!));
            }
            
            throw new InvalidOperationException("Can't compare with empty node: empty node shouldn't exists!");
        }
        
        private int Compare(TComparable comparable, IAnyElementCollection<TData> to) {
            if (to.TryGetAny(out var toData)) {
                return comparable.CompareTo(_compareFieldSelector(toData!));
            }

            throw new InvalidOperationException("Can't compare with empty node: empty node shouldn't exists!");
        }

#endregion

#region Enumerable

        public IEnumerable<CollectionNode<TData, TCollection>> Iterate() {
            if (IsEmpty()) {
                yield break;
            }
            
            IEnumerator<CollectionNode<TData, TCollection>> EnumerateTreeInternal(CollectionNode<TData, TCollection> currentNode) {
                if (currentNode == null) throw new ArgumentNullException(nameof(currentNode));
                if (currentNode.Left != null) {
                    var internalEnumerator = EnumerateTreeInternal(currentNode.Left);
                    while (internalEnumerator.MoveNext()) {
                        yield return internalEnumerator.Current;
                    }
                }

                yield return currentNode;
                
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

        public IEnumerator<TData> GetEnumerator() {
            if (IsEmpty()) {
                yield break;
            }

            IEnumerator<TData> EnumerateTreeInternal(CollectionNode<TData, TCollection> currentNode) {
                if (currentNode == null) throw new ArgumentNullException(nameof(currentNode));
                if (currentNode.Left != null) {
                    var internalEnumerator = EnumerateTreeInternal(currentNode.Left);
                    while (internalEnumerator.MoveNext()) {
                        yield return internalEnumerator.Current;
                    }
                }

                foreach (var data in currentNode) {
                    yield return data;
                }

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