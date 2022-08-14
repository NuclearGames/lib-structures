using System;
using Structures.NetStandard.Structures.BCST;
using Structures.NetStandard.Structures.BST;


namespace LogicUtils.Structures.BCST {
    /// <summary>
    /// Двоичное дерево, в узлах которого находится оллекция, построеннная на двоичном дереве
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TComparable"></typeparam>
    public class LinkedBinaryCollectionTree<TData, TComparable> : BinaryCollectionTree<TData, TComparable, BinaryTree<TData>> 
        where TData: IComparable<TData>
        where TComparable : IComparable<TComparable> {
        
        public LinkedBinaryCollectionTree(Func<TData, TComparable> compareFieldSelector) : base(compareFieldSelector) { }
        public LinkedBinaryCollectionTree(Func<TData, TComparable> compareFieldSelector, TData[] sourceBuffer) : base(compareFieldSelector, sourceBuffer) { }
    }
}