using Structures.NetSixZero.Randoms.Interfaces;
using Structures.NetSixZero.Structures.BST;
using Structures.NetSixZero.Structures.BST.Utils;

namespace Structures.NetSixZero.Extensions {
    public static class BinaryTreeExtensions {
        /// <summary>
        /// Возвращает случайный элемент дерева.
        /// <para>
        /// Использует рандомайзер для получаения случайного индекса,
        /// после чего итерируется по дереву пока не достигнет целевого индекса.
        /// </para>
        /// </summary>
        public static TData GetRandom<TData, TComparable>(this BaseBinaryTree<TData, TComparable> tree, IRandomizer randomizer) 
            where TComparable : IComparable<TComparable> {

            int randomIndex = randomizer.Next(0, tree.Count);
            int currentIndex = -1;
            Node<TData>? result = null;

            bool Iteration(Node<TData> node) {
                currentIndex++;

                if (currentIndex == randomIndex) {
                    result = node;
                    return true;
                }

                if(node.Left != null && Iteration(node.Left)) {
                    return true;
                }

                if (node.Right != null && Iteration(node.Right)) {
                    return true;
                }

                return false;
            }

            if (!Iteration(tree.Root!)) {
                throw new Exception();
            }

            return result!.Data;
        }
    }
}
