using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Structures.NetSixZero.Structures.ConcurrentCollections.Utils;

namespace Structures.NetSixZero.Structures.ConcurrentCollections {
    public class ConcurrentHashSet<T> : HashSet<T> {
        private readonly RWLock _rwLock;

        public ConcurrentHashSet(TimeSpan time) {
            _rwLock = new RWLock(time, LockRecursionPolicy.NoRecursion);
        }
        
        public new bool TryGetValue(T equalValue, [MaybeNullWhen(false)] out T actualValue) {
            using (_rwLock.ReadLock()) {
                return base.TryGetValue(equalValue, out actualValue);
            }
        }
        
        public new int RemoveWhere(Predicate<T> match) {
            using (_rwLock.WriteLock()) {
                return base.RemoveWhere(match);
            }
        }

        public new int EnsureCapacity(int capacity) {
            using (_rwLock.ReadLock()) {
                return base.EnsureCapacity(capacity);
            }
        }
        
        public new void TrimExcess() {
            using (_rwLock.WriteLock()) {
                base.TrimExcess();
            }
        }

#region ISerializable

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            using (_rwLock.ReadLock()) {
                base.GetObjectData(info, context);
            }
        }
        
        public override void OnDeserialization(object? sender) {
            using (_rwLock.ReadLock()) {
                base.OnDeserialization(sender);
            }
        }

#endregion

#region Boolean operations

        public new void UnionWith(IEnumerable<T> other) {
            using (_rwLock.WriteLock()) {
                base.UnionWith(other);
            }
        }
        
        public new void IntersectWith(IEnumerable<T> other) {
            using (_rwLock.WriteLock()) {
                base.IntersectWith(other);
            }
        }
        
        public new void ExceptWith(IEnumerable<T> other) {
            using (_rwLock.WriteLock()) {
                base.ExceptWith(other);
            }
        }
        
        public new void SymmetricExceptWith(IEnumerable<T> other) {
            using (_rwLock.WriteLock()) {
                base.SymmetricExceptWith(other);
            }
        }

#endregion

#region Set Comparison

        public new bool IsSubsetOf(IEnumerable<T> other) {
            using (_rwLock.ReadLock()) {
                return base.IsSubsetOf(other);
            }
        }
        
        public new bool IsProperSubsetOf(IEnumerable<T> other) {
            using (_rwLock.ReadLock()) {
                return base.IsProperSubsetOf(other);
            }
        }
        
        public new bool IsSupersetOf(IEnumerable<T> other) {
            using (_rwLock.ReadLock()) {
                return base.IsSupersetOf(other);
            }
        }
        
        public new bool IsProperSupersetOf(IEnumerable<T> other) {
            using (_rwLock.ReadLock()) {
                return base.IsProperSupersetOf(other);
            }
        }

        public new bool Overlaps(IEnumerable<T> other) {
            using (_rwLock.ReadLock()) {
                return base.Overlaps(other);
            }
        }
        
        public new bool SetEquals(IEnumerable<T> other) {
            using (_rwLock.ReadLock()) {
                return base.SetEquals(other);
            }
        }

#endregion

#region CopyTo

        public new void CopyTo(T[] array) {
            using (_rwLock.ReadLock()) {
                base.CopyTo(array);
            }
        }

        public new void CopyTo(T[] array, int arrayIndex) {
            using (_rwLock.ReadLock()) {
                base.CopyTo(array, arrayIndex);
            }
        }

        public new void CopyTo(T[] array, int arrayIndex, int count) {
            using (_rwLock.ReadLock()) {
                base.CopyTo(array, arrayIndex, count);
            }
        }

#endregion

#region ICollection<T>

        public new bool Add(T item) {
            using (_rwLock.WriteLock()) {
                return base.Add(item);
            }
        }

        public new void Clear() {
            using (_rwLock.WriteLock()) {
                base.Clear();
            }
        }

        public new bool Contains(T item) {
            using (_rwLock.ReadLock()) {
                return base.Contains(item);
            }
        }

        public new bool Remove(T item) {
            using (_rwLock.WriteLock()) {
                return base.Remove(item);
            }
        }

        public new int Count {
            get {
                using (_rwLock.ReadLock()) {
                    return base.Count;
                }
            }
        }

#endregion

#region IEnumerable

        public new IEnumerator<T> GetEnumerator() {
            using (_rwLock.ReadLock()) {
                return base.GetEnumerator();
            }
        }

#endregion

#region Dispose

        ~ConcurrentHashSet() {
            _rwLock?.Dispose();
        }

#endregion
    }
}