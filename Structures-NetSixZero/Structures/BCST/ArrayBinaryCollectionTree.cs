using System;
using Structures.NetSixZero.Utils.Collections;


namespace Structures.NetSixZero.Structures.BCST {
    /// <summary>
    /// Двоичное дерево, в узлах которого находится коллекция, постороенная на массиве
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TComparable"></typeparam>
    public class ArrayBinaryCollectionTree<TData, TComparable> : BinaryCollectionTree<TData, TComparable, AnyElementList<TData>> 
        where TComparable : IComparable<TComparable>  {
        
        public ArrayBinaryCollectionTree(Func<TData, TComparable> compareFieldSelector) : base(compareFieldSelector) { }
        public ArrayBinaryCollectionTree(Func<TData, TComparable> compareFieldSelector, TData[] sourceBuffer) : base(compareFieldSelector, sourceBuffer) { }
    }
}