using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Structures.NetSixZero.Structures.ConcurrentCollections {
    public class ConcurrentHashSet<T> : HashSet<T> {
        private readonly ReaderWriterLockSlim _lock = new(LockRecursionPolicy.NoRecursion);
        private readonly TimeSpan _time;

        public ConcurrentHashSet(TimeSpan time) {
            _time = time;
        }
        
        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            _lock.TryEnterReadLock(_time);
            try {
                base.GetObjectData(info, context);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }
        
        public override void OnDeserialization(object? sender) {
            _lock.TryEnterReadLock(_time);
            try {
                base.OnDeserialization(sender);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }
        
        public new bool TryGetValue(T equalValue, [MaybeNullWhen(false)] out T actualValue) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.TryGetValue(equalValue, out actualValue);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }
        
        public new void UnionWith(IEnumerable<T> other) {
            _lock.TryEnterWriteLock(_time);
            try {
                base.UnionWith(other);
            }
            finally {
                if (_lock.IsWriteLockHeld) {
                    _lock.ExitWriteLock();
                }
            }
        }
        
        public new void IntersectWith(IEnumerable<T> other) {
            _lock.TryEnterWriteLock(_time);
            try {
                base.IntersectWith(other);
            }
            finally {
                if (_lock.IsWriteLockHeld) {
                    _lock.ExitWriteLock();
                }
            }
        }
        
        public new void ExceptWith(IEnumerable<T> other) {
            _lock.TryEnterWriteLock(_time);
            _lock.TryEnterReadLock(_time);
            try {
                base.ExceptWith(other);
            }
            finally {
                if (_lock.IsWriteLockHeld) {
                    _lock.ExitWriteLock();
                }

                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }
        
        public new void SymmetricExceptWith(IEnumerable<T> other) {
            _lock.TryEnterWriteLock(_time);
            try {
                base.SymmetricExceptWith(other);
            }
            finally {
                if (_lock.IsWriteLockHeld) {
                    _lock.ExitWriteLock();
                }
            }
        }
        
        public new bool IsSubsetOf(IEnumerable<T> other) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.IsSubsetOf(other);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }
        
        public new bool IsProperSubsetOf(IEnumerable<T> other) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.IsProperSubsetOf(other);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }
        
        public new bool IsSupersetOf(IEnumerable<T> other) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.IsSupersetOf(other);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }
        
        public new bool IsProperSupersetOf(IEnumerable<T> other) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.IsProperSupersetOf(other);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

        public new bool Overlaps(IEnumerable<T> other) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.Overlaps(other);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }
        
        public new bool SetEquals(IEnumerable<T> other) {
            _lock.TryEnterReadLock(_time);
            try {
                return base.SetEquals(other);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }

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

        public new void CopyTo(T[] array, int arrayIndex, int count) {
            _lock.TryEnterReadLock(_time);
            try {
                base.CopyTo(array, arrayIndex, count);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }
            }
        }
        
        public new int RemoveWhere(Predicate<T> match) {
            _lock.TryEnterReadLock(_time);
            _lock.TryEnterWriteLock(_time);
            try {
                return base.RemoveWhere(match);
            }
            finally {
                if (_lock.IsReadLockHeld) {
                    _lock.ExitReadLock();
                }

                if (_lock.IsWriteLockHeld) {
                    _lock.ExitWriteLock();
                }
            }
        }
        
        public new IEqualityComparer<T> Comparer {
            get {
                _lock.TryEnterReadLock(_time);
                try {
                    return base.Comparer;
                }
                finally {
                    if (_lock.IsReadLockHeld) {
                        _lock.ExitReadLock();
                    }
                }
            }
        }
        
        public new int EnsureCapacity(int capacity)
        {
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

#region ICollection<T>

        public new bool Add(T item) {
            _lock.TryEnterWriteLock(_time);
            try {
                return base.Add(item);
            }
            finally {
                if (_lock.IsWriteLockHeld) {
                    _lock.ExitWriteLock();
                }
            }
        }

        public new void Clear() {
            List<int> list = new List<int>();

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

        public new bool Remove(T item) {
            _lock.TryEnterUpgradeableReadLock(_time);
            _lock.TryEnterWriteLock(_time);
            try {
                return base.Remove(item);
            }
            finally {
                if (_lock.IsWriteLockHeld) {
                    _lock.ExitWriteLock();
                }

                if (_lock.IsUpgradeableReadLockHeld) {
                    _lock.ExitUpgradeableReadLock();
                }
            }
        }

        public new int Count {
            get {
                _lock.TryEnterReadLock(_time);
                try {
                    return base.Count;
                }
                finally {
                    if (_lock.IsReadLockHeld) {
                        _lock.ExitReadLock();
                    }
                }
            }
        }

#endregion

#region IEnumerable

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

#endregion

#region Dispose

        ~ConcurrentHashSet() {
            _lock.Dispose();
        }

#endregion
    }
}