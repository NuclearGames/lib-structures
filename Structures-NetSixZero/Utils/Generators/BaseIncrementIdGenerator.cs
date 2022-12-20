namespace Structures.NetSixZero.Utils.Generators {
    /// <summary>
    /// Базовый тип циклического генератора, инкрементирующего значение.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseIncrementIdGenerator<T> where T : struct {
        private readonly object _locker;
        private T _next;

        protected BaseIncrementIdGenerator(T initial) {
            _locker = new object();
            _next = initial;
        }

        /// <summary>
        /// Возвращает следующее значение.
        /// </summary>
        public T GetNext() {
            lock (_locker) {
                T current = _next;
                _next = Increment(_next);
                return current;
            }
        }

        /// <summary>
        /// Должен увеличить значение циклически.
        /// </summary>
        protected abstract T Increment(T x);
    }
}
