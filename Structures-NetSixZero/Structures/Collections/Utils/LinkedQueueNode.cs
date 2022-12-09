namespace Structures.NetSixZero.Structures.Collections.Utils {
    public class LinkedQueueNode<T> {
#nullable disable
        internal readonly LinkedQueue<T> Collection;
        
        internal LinkedQueueNode<T> NextNode;
        
        internal T item;
        
#nullable enable

#nullable disable

        public LinkedQueueNode(LinkedQueue<T> queue, T value) {
            Collection = queue;
            item = value;
        }
        
#nullable enable

        public LinkedQueue<T>? Queue => Collection;

        public LinkedQueueNode<T>? Next => NextNode;

        public T Value {
            get => item;
            set => item = value;
        }
        
        public ref T ValueRef => ref item;
    }
}