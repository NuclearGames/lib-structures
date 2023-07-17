using System;
using System.Collections.Generic;


namespace NuclearGames.StructuresUnity.Pools {
    /// <summary>
    /// Стандартный пул элементов
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DefaultPool<T> : IDisposable {
        private const int START_DEFAULT_SIZE = 4;
        
        private readonly Queue<T> _container;
        private readonly Func<T> _createFunction;
        private readonly Action<T>? _removeAction;
        private readonly Action<T>? _getAction;
        private readonly Action<T>? _releaseAction;
        
        public DefaultPool(Settings settings) {
            if (settings.StartSize != null && settings.StartSize < 0) {
                throw new ArgumentException($"{nameof(settings.StartSize)} has invalid value.");
            }

            if (settings.CreateFunction == null) {
                throw new ArgumentException($"{nameof(settings.CreateFunction)} can not be null.");
            }

            _createFunction = settings.CreateFunction!;
            _removeAction = settings.RemoveAction;
            _getAction = settings.GetAction;
            _releaseAction = settings.ReleaseAction;

            int capacity = settings.StartSize ?? START_DEFAULT_SIZE;
            _container = new Queue<T>(capacity);
            for (var i = 0; i < capacity; i++) {
                _container.Enqueue(_createFunction());
            }
        }
        
        /// <summary>
        /// Возвращает элемент из пула или создает новый.
        /// </summary>
        /// <returns></returns>
        public T Get() {
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
            _releaseAction?.Invoke(instance);
            _container.Enqueue(instance);
        }
        
        public void Dispose() {
            while (_container.Count > 0) {
                _removeAction?.Invoke(_container.Dequeue());
            }
        }

#region Utils

        public class Settings {
            public Func<T> CreateFunction { get; set; }
            public Action<T> RemoveAction { get; set; }
            public Action<T> GetAction { get; set; }
            public Action<T> ReleaseAction { get; set; }
            public int? StartSize { get; set; }
        }

#endregion
    }
}