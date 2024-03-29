﻿using Structures.NetSixZero.Extensions;

namespace Structures.NetSixZero.Pools {
    /// <summary>
    /// Пул, вычисляющий размер автоматически, как среднее из нескольких последних пиков потребления.
    /// </summary>
    public class AverageConsumptionPool<T> : IDisposable {
        private const int SIZE_CONTROL_DEPTH_DEFAULT = 4;

        /// <summary>
        /// Текущий размер пула.
        /// </summary>
        public int Size { get; private set; }

        private bool ShouldRemoveInstance => _container.Count >= Size;

        private readonly Queue<T> _container;
        private readonly List<int> _sizeControl;
        private readonly int _sizeControlDepth;
        private readonly Func<T> _createFunction;
        private readonly Action<T>? _removeAction;
        private readonly Action<T>? _getAction;
        private readonly Action<T>? _releaseAction;
        private readonly int _minSize;

        /// <summary>
        /// Индекс текущего цикла из <see cref="_sizeControl"/>.
        /// </summary>
        private int _currentCycleIndex;

        /// <summary>
        /// Кол-во используемых в данный момент элементов.
        /// </summary>
        private int _inUse;

        /// <summary>
        /// Сумма элементов <see cref="_sizeControl"/>.
        /// </summary>
        private int _currentSizeControlSum;

        public AverageConsumptionPool(Settings settings) {
            if (settings.StartSize != null && settings.StartSize < 0) {
                throw new ArgumentException($"{nameof(settings.StartSize)} has invalid value.");
            }

            if (settings.SizeControlDepth != null && settings.SizeControlDepth <= 0) {
                throw new ArgumentException($"{nameof(settings.SizeControlDepth)} has invalid value.");
            }

            if(settings.CreateFunction == null) {
                throw new ArgumentException($"{nameof(settings.CreateFunction)} can not be null.");
            }

            _sizeControlDepth = settings.SizeControlDepth ?? SIZE_CONTROL_DEPTH_DEFAULT;
            _createFunction = settings.CreateFunction!;
            _removeAction = settings.RemoveAction;
            _getAction = settings.GetAction;
            _releaseAction = settings.ReleaseAction;
            _minSize = settings.MinSize;

            _container = new Queue<T>();
            _sizeControl = new List<int>(_sizeControlDepth);

            Size = settings.StartSize ?? _minSize;
            _sizeControl.Add(Size);
        }

        /// <summary>
        /// Вычисляет новый размер.
        /// Начинает новый отсчет потребления.
        /// </summary>
        public void ResetCycle() {
            TryUpdateCurrentConsumption();

            _currentSizeControlSum += _sizeControl[_currentCycleIndex];

            Size = Math.Max((int)Math.Ceiling(_currentSizeControlSum / (float)_sizeControl.Count), _minSize);

            _currentSizeControlSum -= IncrementCycleIndex();
        }

        /// <summary>
        /// Возвращает элемент из пула или создает новый.
        /// </summary>
        /// <returns></returns>
        public T Get() {
            _inUse++;
            TryUpdateCurrentConsumption();

            T instance;
            if (_container.Count > 0) {
                instance = _container.Dequeue();
            } else {
                instance = _createFunction();
            }

            _getAction?.Invoke(instance);
            return instance;
        }

        /// <summary>
        /// Возвращает элемент в пул.
        /// Если места нет - удаляется.
        /// </summary>
        public void Return(T instance) {
            _inUse = Math.Max(_inUse - 1, 0);

            if (!ShouldRemoveInstance) {
                _releaseAction?.Invoke(instance);
                _container.Enqueue(instance);
                return;
            }

            _removeAction?.Invoke(instance);
        }

        private void TryUpdateCurrentConsumption() {
            if(_currentCycleIndex == -1) {
                return;
            }

            if (_inUse > _sizeControl[_currentCycleIndex]) {
                _sizeControl[_currentCycleIndex] = _inUse;
            }
        }

        /// <summary>
        /// Передвигает индекс текущего цикла.
        /// Добавляет или очищает ячейку массива.
        /// </summary>
        /// <returns>Значение которое было затерто.</returns>
        private int IncrementCycleIndex() {
            _currentCycleIndex = (_currentCycleIndex + 1).Loop(_sizeControlDepth);
            if (_sizeControl.Count <= _currentCycleIndex) {
                _sizeControl.Add(0);
                return 0;
            } else {
                int valueToRemove = _sizeControl[_currentCycleIndex];
                _sizeControl[_currentCycleIndex] = 0;
                return valueToRemove;
            }
        }

        public void Dispose() {
            while(_container.Count > 0) {
                _removeAction?.Invoke(_container.Dequeue());
            }
        }

        public class Settings {
            public Func<T>? CreateFunction { get; set; }
            public Action<T>? RemoveAction { get; set; }
            public Action<T>? GetAction { get; set; }
            public Action<T>? ReleaseAction { get; set; }
            public int? SizeControlDepth { get; set; }
            public int? StartSize { get; set; }
            public int MinSize { get; set; }
        }
    }
}
