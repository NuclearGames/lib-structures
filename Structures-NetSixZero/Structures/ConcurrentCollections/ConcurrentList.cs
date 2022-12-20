using System.Collections.ObjectModel;
using Structures.NetSixZero.Structures.ConcurrentCollections.Utils;

namespace Structures.NetSixZero.Structures.ConcurrentCollections {
    public class ConcurrentList<T> : List<T> {
        private readonly ReaderWriterLockSlim _lock = new(LockRecursionPolicy.NoRecursion);
        private readonly RWLock _rwLock;
        private readonly TimeSpan _time;

        public new int Capacity {
            get {
                using (_rwLock.ReadLock()) {
                    return base.Capacity;
                }
            }
        }
        
        public ConcurrentList(TimeSpan time) {
            _time = time;
            _rwLock = new RWLock(_time, LockRecursionPolicy.NoRecursion);
        }

        public new T this[int index] {
            get {
                _lock.TryEnterReadLock(_time);
                try {
                    return base[index];
                }
                finally {
                    if (_lock.IsReadLockHeld) {
                        _lock.ExitReadLock();
                    }
                }
            }
            set {
                _lock.TryEnterWriteLock(_time);
                try {
                    base[index] = value;
                }
                finally {
                    if (_lock.IsWriteLockHeld) {
                        _lock.ExitWriteLock();
                    }
                }
            }
        }

        public new ReadOnlyCollection<T> AsReadOnly() {
            _lock.TryEnterReadLock(_time);
            try {
                return base.AsReadOnly();
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

#region Add

        public new void Add(T item) {
            using (_rwLock.WriteLock()) {
                base.Add(item);
            }
            
            //_lock.TryEnterWriteLock(_time);
            //try {
            //    base.Add(item);
            //}
            //finally {
            //    if (_lock.IsWriteLockHeld) {
            //        _lock.ExitWriteLock();
            //    }
            //}
        }

        public new void AddRange(IEnumerable<T> collection) {
            _lock.TryEnterWriteLock(_time);
            try {
                base.AddRange(collection);
            }
            finally {
                if (_lock.IsWriteLockHeld) {
                    _lock.ExitWriteLock();
                }
            }
        }

#endregion

#region BinarySearch

        public new int BinarySearch(int index, int count, T item, IComparer<T>? comparer) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.BinarySearch(index, count, item, comparer);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

        public new int BinarySearch(T item) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.BinarySearch(item);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

        public new int BinarySearch(T item, IComparer<T>? comparer) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.BinarySearch(item, comparer);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

#endregion

        public new bool Contains(T item) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.Contains(item);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }
        
        public new List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.ConvertAll(converter);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

#region CopyTo

        public new void CopyTo(T[] array) {
            _lock.TryEnterReadLock(_time);
            try {
                base.CopyTo(array);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

        public new void CopyTo(int index, T[] array, int arrayIndex, int count) {
            _lock.TryEnterReadLock(_time);
            try {
                base.CopyTo(index, array, arrayIndex, count);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

        public new void CopyTo(T[] array, int arrayIndex) {
            _lock.TryEnterReadLock(_time);
            try {
                base.CopyTo(array, arrayIndex);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

#endregion
        
        public new int EnsureCapacity(int capacity) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.EnsureCapacity(capacity);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

        public new bool Exists(Predicate<T> match) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.Exists(match);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

#region Find

        public new T? Find(Predicate<T> match) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.Find(match);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

        public new List<T> FindAll(Predicate<T> match) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.FindAll(match);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

        public new int FindIndex(Predicate<T> match) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.FindIndex(match);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

        public new int FindIndex(int startIndex, Predicate<T> match) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.FindIndex(startIndex, match);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

        public new int FindIndex(int startIndex, int count, Predicate<T> match) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.FindIndex(startIndex, count, match);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

        public new T? FindLast(Predicate<T> match) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.FindLast(match);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

        public new int FindLastIndex(Predicate<T> match) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.FindLastIndex(match);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

        public new int FindLastIndex(int startIndex, Predicate<T> match) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.FindLastIndex(startIndex, match);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

        public new int FindLastIndex(int startIndex, int count, Predicate<T> match) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.FindLastIndex(startIndex, count, match);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

#endregion

        public new void ForEach(Action<T> action) {
            _lock.TryEnterReadLock(_time);
            try {
                base.ForEach(action);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

        public new IEnumerator<T> GetEnumerator() {
            _lock.TryEnterReadLock(_time);
            try {
                return base.GetEnumerator();
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

        public new List<T> GetRange(int index, int count) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.GetRange(index, count);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }
        
        public new T[] ToArray() {
            _lock.TryEnterReadLock(_time);
            try {
                return base.ToArray();
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }
        
        public new void TrimExcess() {
            _lock.TryEnterReadLock(_time);
            try {
                base.TrimExcess();
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

        public new bool TrueForAll(Predicate<T> match) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.TrueForAll(match);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

#region IndexOf

        public new int IndexOf(T item) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.IndexOf(item);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

        public new int IndexOf(T item, int index) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.IndexOf(item, index);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }
        
        public new int IndexOf(T item, int index, int count) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.IndexOf(item, index, count);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

#endregion

#region Insert

        public new void Insert(int index, T item) {
            _lock.TryEnterWriteLock(_time);
            try {
                base.Insert(index, item);
            }
            finally {
                if (_lock.IsWriteLockHeld) {
                    _lock.ExitWriteLock();
                }
            }
        }
        
        public new void InsertRange(int index, IEnumerable<T> collection) {
            _lock.TryEnterWriteLock(_time);
            try {
                base.InsertRange(index, collection);
            }
            finally {
                if (_lock.IsWriteLockHeld) {
                    _lock.ExitWriteLock();
                }
            }
        }

#endregion

#region LastIndexOf

        public new int LastIndexOf(T item) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.LastIndexOf(item);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }
        
        public new int LastIndexOf(T item, int index) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.LastIndexOf(item, index);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }
        
        public new int LastIndexOf(T item, int index, int count) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.LastIndexOf(item, index, count);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

#endregion

#region Remove

        public new bool Remove(T item) {
            using (_rwLock.WriteLock()) {
                return base.Remove(item);
            }
        }
        
        public new int RemoveAll(Predicate<T> match) {
            _lock.TryEnterUpgradeableReadLock(_time);
            _lock.TryEnterWriteLock(_time);
            try {
                return base.RemoveAll(match);
            }
            finally {
                if (_lock.IsUpgradeableReadLockHeld) {
                    _lock.ExitUpgradeableReadLock();
                }

                if (_lock.IsWriteLockHeld) {
                    _lock.ExitWriteLock();
                }
            }
        }
        
        public new void RemoveAt(int index) {
            _lock.TryEnterUpgradeableReadLock(_time);
            _lock.TryEnterWriteLock(_time);
            try {
                base.RemoveAt(index);
            }
            finally {
                if (_lock.IsUpgradeableReadLockHeld) {
                    _lock.ExitUpgradeableReadLock();
                }

                if (_lock.IsWriteLockHeld) {
                    _lock.ExitWriteLock();
                }
            }
        }

        public new void RemoveRange(int index, int count) {
            _lock.TryEnterUpgradeableReadLock(_time);
            _lock.TryEnterWriteLock(_time);
            try {
                base.RemoveRange(index, count);
            }
            finally {
                if (_lock.IsUpgradeableReadLockHeld) {
                    _lock.ExitUpgradeableReadLock();
                }

                if (_lock.IsWriteLockHeld) {
                    _lock.ExitWriteLock();
                }
            }
        }
        
        public new void Clear() {
            _lock.TryEnterWriteLock(_time);
            try {
                base.Clear();
            }
            finally {
                if (_lock.IsWriteLockHeld) {
                    _lock.ExitWriteLock();
                }
            }
        }

#endregion

#region Reorder

        public new void Reverse() {
            _lock.TryEnterWriteLock(_time);
            try {
                base.Reverse();
            }
            finally {
                if (_lock.IsWriteLockHeld) {
                    _lock.ExitWriteLock();
                }
            }
        }
        
        public new void Reverse(int index, int count) {
            _lock.TryEnterWriteLock(_time);
            try {
                base.Reverse(index, count);
            }
            finally {
                if (_lock.IsWriteLockHeld) {
                    _lock.ExitWriteLock();
                }
            }
        }
        
        public new void Sort() {
            _lock.TryEnterWriteLock(_time);
            try {
                base.Sort();
            }
            finally {
                if (_lock.IsWriteLockHeld) {
                    _lock.ExitWriteLock();
                }
            }
        }

        public new void Sort(IComparer<T>? comparer) {
            _lock.TryEnterReadLock(_time);
            try {
                base.Sort(comparer);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }
        
        public new void Sort(int index, int count, IComparer<T>? comparer) {
            _lock.TryEnterWriteLock(_time);
            try {
                base.Sort(index, count, comparer);
            }
            finally {
                if (_lock.IsWriteLockHeld) {
                    _lock.ExitWriteLock();
                }
            }
        }

        public new void Sort(Comparison<T> comparison) {
            _lock.TryEnterWriteLock(_time);
            try {
                base.Sort(comparison);
            }
            finally {
                if (_lock.IsWriteLockHeld) {
                    _lock.ExitWriteLock();
                }
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