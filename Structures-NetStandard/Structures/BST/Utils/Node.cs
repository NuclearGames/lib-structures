namespace Structures.NetStandard.Structures.BST.Utils {
    /// <summary>
    /// Узел двочиного дерева
    /// </summary>
    public class Node<T> { // where T : IComparable<T> {
        /// <summary>
        /// Данные узла
        /// </summary>
        public T Data { get; set; }
        
        /// <summary>
        /// Левый дочерний узел элемента
        /// </summary>
        public Node<T>? Left { get; set; }
        
        /// <summary>
        /// Правый дочерний узел элемента
        /// </summary>
        public Node<T>? Right { get; set; }

        public Node(T data, Node<T>? left = null, Node<T>? right = null) {
            Data = data;
            Left = left;
            Right = right;
        }
    }
}