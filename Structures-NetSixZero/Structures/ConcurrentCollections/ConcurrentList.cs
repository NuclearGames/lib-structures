using System.Collections.ObjectModel;
using Structures.NetSixZero.Structures.ConcurrentCollections.Utils;

namespace Structures.NetSixZero.Structures.ConcurrentCollections {
    public class ConcurrentList<T> : List<T> {
        public new int Capacity {
            get {
                using (_lock.ReadLock()) {
                    return base.Capacity;
                }
            }
        }

        public new int Count {
            get {
                using (_lock.ReadLock()) {
                    return base.Count;
                }
            }
        }
        
        private readonly RWLock _lock;
        
        public ConcurrentList(RWLock.WaitTime time) {
            _lock = new RWLock(time, LockRecursionPolicy.NoRecursion);
        }

        public new T this[int index] {
            get {
                using (_lock.ReadLock()) {
                    return base[index];
                }
            }
            set {
                using (_lock.WriteLock()) {
                    base[index] = value;
                }
            }
        }

#region Add

        public new void Add(T item) {
            using (_lock.WriteLock()) {
                base.Add(item);
            }
        }

        public new void AddRange(IEnumerable<T> collection) {
            using (_lock.WriteLock()) {
                base.AddRange(collection);
            }
        }

#endregion

#region BinarySearch

        public new int BinarySearch(int index, int count, T item, IComparer<T>? comparer) {
            using (_lock.ReadLock()) {
                return base.BinarySearch(index, count, item, comparer);
            }
        }

        public new int BinarySearch(T item) {
            using (_lock.ReadLock()) {
                return base.BinarySearch(item);
            }
        }

        public new int BinarySearch(T item, IComparer<T>? comparer) {
            using (_lock.ReadLock()) {
                return base.BinarySearch(item, comparer);
            }
        }

#endregion
        
#region CopyTo

        public new void CopyTo(T[] array) {
            using (_lock.ReadLock()) {
                base.CopyTo(array);
            }
        }

        public new void CopyTo(int index, T[] array, int arrayIndex, int count) {
            using (_lock.ReadLock()) {
                base.CopyTo(index, array, arrayIndex, count);
            }
        }

        public new void CopyTo(T[] array, int arrayIndex) {
            using (_lock.ReadLock()) {
                base.CopyTo(array, arrayIndex);
            }
        }

#endregion

#region Utils

        public new int EnsureCapacity(int capacity) {
            using (_lock.ReadLock()) {
                return base.EnsureCapacity(capacity);
            }
        }

        public new bool Exists(Predicate<T> match) {
            using (_lock.ReadLock()) {
                return base.Exists(match);
            }
        }
        
        public new void TrimExcess() {
            using (_lock.WriteLock()) {
                base.TrimExcess();
            }
        }
        
        public new List<T> GetRange(int index, int count) {
            using (_lock.ReadLock()) {
                return base.GetRange(index, count);
            }
        }
        
        public new bool Contains(T item) {
            using (_lock.ReadLock()) {
                return base.Contains(item);
            }
        }
        
#endregion

#region Find

        public new T? Find(Predicate<T> match) {
            using (_lock.ReadLock()) {
                return base.Find(match);
            }
        }

        public new List<T> FindAll(Predicate<T> match) {
            using (_lock.ReadLock()) {
                return base.FindAll(match);
            }
        }

        public new int FindIndex(Predicate<T> match) {
            using (_lock.ReadLock()) {
                return base.FindIndex(match);
            }
        }

        public new int FindIndex(int startIndex, Predicate<T> match) {
            using (_lock.ReadLock()) {
                return base.FindIndex(startIndex, match);
            }
        }

        public new int FindIndex(int startIndex, int count, Predicate<T> match) {
            using (_lock.ReadLock()) {
                return base.FindIndex(startIndex, count, match);
            }
        }

        public new T? FindLast(Predicate<T> match) {
            using (_lock.ReadLock()) {
                return base.FindLast(match);
            }
        }

        public new int FindLastIndex(Predicate<T> match) {
            using (_lock.ReadLock()) {
                return base.FindLastIndex(match);
            }
        }

        public new int FindLastIndex(int startIndex, Predicate<T> match) {
            using (_lock.ReadLock()) {
                return base.FindLastIndex(startIndex, match);
            }
        }

        public new int FindLastIndex(int startIndex, int count, Predicate<T> match) {
            using (_lock.ReadLock()) {
                return base.FindLastIndex(startIndex, count, match);
            }
        }

#endregion

#region ForEach

        public new void ForEach(Action<T> action) {
            using (_lock.ReadLock()) {
                base.ForEach(action);
            }
        }
        
        public new bool TrueForAll(Predicate<T> match) {
            using (_lock.ReadLock()) {
                return base.TrueForAll(match);
            }
        }
        
        public new IEnumerator<T> GetEnumerator() {
            using (_lock.ReadLock()) {
                return base.GetEnumerator();
            }
        }

#endregion

#region Convert

        public new T[] ToArray() {
            using (_lock.ReadLock()) {
                return base.ToArray();
            }
        }
        
        public new List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter) {
            using (_lock.ReadLock()) {
                return base.ConvertAll<TOutput>(converter);
            }
        }
        
        public new ReadOnlyCollection<T> AsReadOnly() {
            using (_lock.ReadLock()) {
                return base.AsReadOnly();
            }
        }

#endregion

#region IndexOf

        public new int IndexOf(T item) {
            using (_lock.ReadLock()) {
                return base.IndexOf(item);
            }
        }

        public new int IndexOf(T item, int index) {
            using (_lock.ReadLock()) {
                return base.IndexOf(item, index);
            }
        }
        
        public new int IndexOf(T item, int index, int count) {
            using (_lock.ReadLock()) {
                return base.IndexOf(item, index, count);
            }
        }

#endregion

#region Insert

        public new void Insert(int index, T item) {
            using (_lock.WriteLock()) {
                base.Insert(index, item);
            }
        }
        
        public new void InsertRange(int index, IEnumerable<T> collection) {
            using (_lock.WriteLock()) {
                base.InsertRange(index, collection);
            }
        }

#endregion

#region LastIndexOf

        public new int LastIndexOf(T item) {
            using (_lock.ReadLock()) {
                return base.LastIndexOf(item);
            }
        }
        
        public new int LastIndexOf(T item, int index) {
            using (_lock.ReadLock()) {
                return base.LastIndexOf(item, index);
            }
        }
        
        public new int LastIndexOf(T item, int index, int count) {
            using (_lock.ReadLock()) {
                return base.LastIndexOf(item, index, count);
            }
        }

#endregion

#region Remove

        public new bool Remove(T item) {
            using (_lock.WriteLock()) {
                return base.Remove(item);
            }
        }
        
        public new int RemoveAll(Predicate<T> match) {
            using (_lock.WriteLock()) {
                return base.RemoveAll(match);
            }
        }
        
        public new void RemoveAt(int index) {
            using (_lock.WriteLock()) {
                base.RemoveAt(index);
            }
        }

        public new void RemoveRange(int index, int count) {
            using (_lock.WriteLock()) {
                base.RemoveRange(index, count);
            }
        }
        
        public new void Clear() {
            using (_lock.WriteLock()) {
                base.Clear();
            }
        }

#endregion

#region Reorder

        public new void Reverse() {
            using (_lock.WriteLock()) {
                base.Reverse();
            }
        }
        
        public new void Reverse(int index, int count) {
            using (_lock.WriteLock()) {
                base.Reverse(index, count);
            }
        }
        
        public new void Sort() {
            using (_lock.WriteLock()) {
                base.Sort();
            }
        }

        public new void Sort(IComparer<T>? comparer) {
            using (_lock.WriteLock()) {
                base.Sort(comparer);
            }
        }
        
        public new void Sort(int index, int count, IComparer<T>? comparer) {
            using (_lock.WriteLock()) {
                base.Sort(index, count, comparer);
            }
        }

        public new void Sort(Comparison<T> comparison) {
            using (_lock.WriteLock()) {
                base.Sort(comparison);
            }
        }

#endregion

#region Dispose

        ~ConcurrentList() {
            _lock?.Dispose();
        }

#endregion
    }
}