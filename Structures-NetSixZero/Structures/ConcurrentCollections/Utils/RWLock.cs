namespace Structures.NetSixZero.Structures.ConcurrentCollections.Utils {
    /// <summary>
    /// Обертка над <see cref="ReaderWriterLockSlim"/>.
    /// </summary>
    public class RWLock : IDisposable {
        private readonly ReaderWriterLockSlim _lock;
        private readonly TimeSpan _time;
    
        public RWLock(TimeSpan time, LockRecursionPolicy policy) {
            _time = time;
            _lock = new ReaderWriterLockSlim(policy);
        }

        public ReadLockToken ReadLock() {
            return new(_lock, _time);
        }

        public WriteLockToken WriteLock() {
            return new(_lock, _time);
        }
        
        public ReadWriteLockToken ReadWriteLock() {
            return new(_lock, _time);
        }

        public void Dispose() {
            _lock.Dispose();
        }

        public readonly struct WriteLockToken : IDisposable {
            private readonly ReaderWriterLockSlim _rwLock;

            public WriteLockToken(ReaderWriterLockSlim rwLock, TimeSpan time) {
                _rwLock = rwLock;
                if (!_rwLock.TryEnterWriteLock(time)) {
                    throw new TimeoutException("Enter write lock timeout");
                }
            }

            public void Dispose() {
                if (_rwLock.IsWriteLockHeld) {
                    _rwLock.ExitWriteLock();
                }
            }
        }

        public readonly struct ReadLockToken : IDisposable {
            private readonly ReaderWriterLockSlim _rwLock;

            public ReadLockToken(ReaderWriterLockSlim rwLock, TimeSpan time) {
                _rwLock = rwLock;
                if (!_rwLock.TryEnterReadLock(time)) {
                    throw new TimeoutException("Enter read lock timeout");
                }
            }

            public void Dispose() {
                if (_rwLock.IsReadLockHeld) {
                    _rwLock.ExitReadLock();
                }
            }
        }
        
        public readonly struct ReadWriteLockToken : IDisposable {
            private readonly ReaderWriterLockSlim _rwLock;

            public ReadWriteLockToken(ReaderWriterLockSlim rwLock, TimeSpan time) {
                _rwLock = rwLock;
                if (!_rwLock.TryEnterUpgradeableReadLock(time)) {
                    throw new TimeoutException("Enter upgradeable read lock timeout");
                }
                
                if (!_rwLock.TryEnterWriteLock(time)) {
                    throw new TimeoutException("Enter write lock timeout");
                }
            }

            public void Dispose() {
                if (_rwLock.IsUpgradeableReadLockHeld) {
                    _rwLock.ExitUpgradeableReadLock();
                }
                
                if (_rwLock.IsWriteLockHeld) {
                    _rwLock.ExitWriteLock();
                }
            }
        }
    }
}