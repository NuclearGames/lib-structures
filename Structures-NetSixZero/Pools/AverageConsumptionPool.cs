using Structures.NetSixZero.Extension;

namespace Structures.NetSixZero.Pools {
    public sealed class AverageConsumptionPool<T> {
        private const int SIZE_CONTROL_DEPTH_DEFAULT = 5;
        private const int SIZE_DEFAULT = 0;

        public int Size { get; private set; }
                
        private readonly Queue<T> _container;
        private readonly Func<T> _createFunction;
        private readonly Action<T>? _removeAction;
        private readonly int[] _sizeControl;

        private int _currentCycleIndex;

        public AverageConsumptionPool(Func<T> createFunction, Action<T>? removeAction, 
            int startSize = SIZE_DEFAULT, int sizeControlDepth = SIZE_CONTROL_DEPTH_DEFAULT) {

            if (startSize <= 0) {
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

        public void StartCycle() {
            Size = (int)Math.Ceiling(_sizeControl.Average());
            _currentCycleIndex = (_currentCycleIndex + 1).Loop(_sizeControl.Length);
            _sizeControl[_currentCycleIndex] = 0;
        }

        public T Get() {
            _sizeControl[_currentCycleIndex]++;

            if (_container.Count > 0) {
                return _container.Dequeue();
            }
            return CreateNewInstance();
        }

        public void Return(T instance) {
            if(_container.Count < Size) {
                _container.Enqueue(instance);
                return;
            }

            _removeAction?.Invoke(instance);
        }

        private T CreateNewInstance() {
            return _createFunction();
        }
    }
}
