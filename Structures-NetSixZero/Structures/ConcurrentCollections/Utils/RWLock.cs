namespace Structures.NetSixZero.Structures.ConcurrentCollections.Utils {
    /// <summary>
    /// Обертка над <see cref="ReaderWriterLockSlim"/>.
    /// </summary>
    public class RWLock : IDisposable {
        private readonly ReaderWriterLockSlim _lock;
        private readonly WaitTime _time;
    
        public RWLock(WaitTime waitTime, LockRecursionPolicy policy) {
            _time = waitTime;
            _lock = new ReaderWriterLockSlim(policy);
        }

        public ReadLockToken ReadLock() {
            return new(_lock, _time);
        }

        public WriteLockToken WriteLock() {
            return new(_lock, _time);
        }

        public void Dispose() {
            _lock.Dispose();
        }

        public readonly struct WriteLockToken : IDisposable {
            private readonly ReaderWriterLockSlim _rwLock;

            public WriteLockToken(ReaderWriterLockSlim rwLock, int milliseconds) {
                _rwLock = rwLock;
                if (!_rwLock.TryEnterWriteLock(milliseconds)) {
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

            public ReadLockToken(ReaderWriterLockSlim rwLock, int milliseconds) {
                _rwLock = rwLock;
                if (!_rwLock.TryEnterReadLock(milliseconds)) {
                    throw new TimeoutException("Enter read lock timeout");
                }
            }

            public void Dispose() {
                if (_rwLock.IsReadLockHeld) {
                    _rwLock.ExitReadLock();
                }
            }
        }
        
        public readonly struct WaitTime {
            private readonly int _milliseconds;

            private WaitTime(int milliseconds) {
                _milliseconds = milliseconds;
            }

            public static implicit operator WaitTime(int time) {
                return new WaitTime(time);
            }

            public static implicit operator int(WaitTime waitTime) {
                return waitTime._milliseconds;
            }
            
            public static implicit operator WaitTime(TimeSpan waitTime) {
                return new WaitTime((int) waitTime.TotalMilliseconds);
            }
        }
        
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