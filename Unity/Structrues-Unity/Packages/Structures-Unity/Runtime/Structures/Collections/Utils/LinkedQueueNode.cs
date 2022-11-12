namespace NuclearGames.StructuresUnity.Structures.Collections.Utils {
    public class LinkedQueueNode<T> {
#nullable disable
        internal readonly LinkedQueue<T> Collection;
        
        internal LinkedQueueNode<T> NextNode;
        
        internal T Item;
        
#nullable enable

#nullable disable

        internal LinkedQueueNode(LinkedQueue<T> queue, T value) {
            Collection = queue;
            Item = value;
        }
        
#nullable enable

        public LinkedQueue<T>? Queue => Collection;

        public LinkedQueueNode<T>? Next => NextNode;

        public T Value {
            get => Item;
            set => Item = value;
        }
        
        public ref T ValueRef => ref Item;
    }
}