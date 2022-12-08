using Structures.NetSixZero.Extension;

namespace Structures.NetSixZero.Pools {
    public class AverageConsumptionPool<T> {
        private const int SIZE_CONTROL_DEPTH_DEFAULT = 4;
        private const int SIZE_DEFAULT = 0;

        public int Size { get; private set; }
                
        private readonly Queue<T> _container;
        private readonly Func<T> _createFunction;
        private readonly Action<T>? _removeAction;
        private readonly int[] _sizeControl;

        /// <summary>
        /// Индекс текущего цикла из <see cref="_sizeControl"/>.
        /// </summary>
        private int _currentCycleIndex;

        /// <summary>
        /// Кол-во используемых в данный момент элементов.
        /// </summary>
        private int _inUse;

        public AverageConsumptionPool(Func<T> createFunction, Action<T>? removeAction, 
            int startSize = SIZE_DEFAULT, int sizeControlDepth = SIZE_CONTROL_DEPTH_DEFAULT) {

            if (startSize < 0) {
                throw new ArgumentException($"{nameof(startSize)} has invalid value.");
            }

            if (sizeControlDepth <= 0) {
                throw new ArgumentException($"{nameof(sizeControlDepth)} has invalid value.");
            }

            _container = new Queue<T>();
            _createFunction = createFunction;
            _removeAction = removeAction;
            _sizeControl = new int[sizeControlDepth];
            for(int i = 0; i < _sizeControl.Length; i++) {
                _sizeControl[i] = startSize;
            }
            StartCycle();
        }

        /// <summary>
        /// Вычисляет новый размер.
        /// Начинает новый отсчет потребления.
        /// </summary>
        public void StartCycle() {
            TryUpdateCurrentConsumption();
            Size = (int)Math.Ceiling(_sizeControl.Average());
            _currentCycleIndex = (_currentCycleIndex + 1).Loop(_sizeControl.Length);
            _sizeControl[_currentCycleIndex] = 0;
        }

        public T Get() {
            _inUse++;
            TryUpdateCurrentConsumption();

            if (_container.Count > 0) {
                return _container.Dequeue();
            }

            return _createFunction();
        }

        public void Return(T instance) {
            _inUse = Math.Max(_inUse - 1, 0);

            if (_container.Count < Size) {
                _container.Enqueue(instance);
                return;
            }

            _removeAction?.Invoke(instance);
        }

        private void TryUpdateCurrentConsumption() {
            if (_inUse > _sizeControl[_currentCycleIndex]) {
                _sizeControl[_currentCycleIndex] = _inUse;
            }
        }
    }
}
