namespace Structures.NetSixZero.Structures.ConcurrentCollections.Utils {
    /// <summary>
    /// Обертка над <see cref="ReaderWriterLockSlim"/>.
    /// </summary>
    public class RWLock : IDisposable {
        private readonly ReaderWriterLockSlim _lock;
        private readonly TimeSpan _time;
    
        public RWLock(TimeSpan time, LockRecursionPolicy policy = LockRecursionPolicy.NoRecursion) {
            _time = time;
            _lock = new ReaderWriterLockSlim(policy);
        }

        public ReadLockToken ReadLock() {
            return new ReadLockToken(_lock, _time);
        }
        
        public UpgradableReadLockToken UpgradableReadLock() {
            return new UpgradableReadLockToken(_lock, _time);
        }

        public WriteLockToken WriteLock() {
            return new WriteLockToken(_lock, _time);
        }

#region Tokens

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
        
        public readonly struct UpgradableReadLockToken : IDisposable {
            private readonly ReaderWriterLockSlim _rwLock;

            public UpgradableReadLockToken(ReaderWriterLockSlim rwLock, TimeSpan time) {
                _rwLock = rwLock;
                if (!_rwLock.TryEnterUpgradeableReadLock(time)) {
                    throw new TimeoutException("Enter upgradable read lock timeout");
                }
            }

            public void Dispose() {
                if (_rwLock.IsUpgradeableReadLockHeld) {
                    _rwLock.ExitUpgradeableReadLock();
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

#endregion
        
#region Dispoable

        private bool _isDisposed;
        public void Dispose() {
            if(!_isDisposed) {
                _lock.Dispose();
                
                _isDisposed = true;
            }
        }

#endregion
    }
}