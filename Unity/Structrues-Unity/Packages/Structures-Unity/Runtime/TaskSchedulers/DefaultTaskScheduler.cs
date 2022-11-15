using Cysharp.Threading.Tasks;
using NuclearGames.StructuresUnity.Structures.Collections;
using System;

namespace NuclearGames.StructuresUnity.TaskSchedulers {
    public sealed class DefaultTaskScheduler {
        private readonly struct QueuedTaskEntity {
            internal readonly int Id;
            internal readonly Func<UniTask> Func;
            
            public QueuedTaskEntity(int id, Func<UniTask> func) {
                Id = id;
                Func = func;
            }
        }

        private readonly object _locker = new object(); 
        private bool _executingTask = false;

        private QueuedTaskEntity _currentTask = default;

        private int _idIncrementor;
        private readonly LinkedQueue<QueuedTaskEntity> _identifierQueue = new LinkedQueue<QueuedTaskEntity>();


        /// <summary>
        /// Проситв выполнить задачу:
        /// <para>1. Выясняет, есть ли текущая задачу, которую требуется выполнить</para>
        /// <para>2. Если есть, то ставит в очереди и дожидается, когда наступит ее очередь</para>
        /// <para>3. Если нет, то выполняет</para>
        /// <para>4. Дожидается выполнения</para>
        /// <para>5. Если есть следующая задача в очереди, то вытаскивает ее</para>
        /// </summary>
        /// <param name="requestedTask">Задача, которую требуется выполнить в текущем контексте</param>
        public async UniTask Execute(Func<UniTask> requestedTask) {
            int id;
            lock (_locker) {
                unchecked {
                    id = ++_idIncrementor;
                }
                
                var taskEntity = new QueuedTaskEntity(id, requestedTask);
                _identifierQueue.Enqueue(taskEntity);
            }

            if (_executingTask) {
                // Debug.Log($"Await task execution: '{id}'");
                await UniTask.WaitUntil(() => _currentTask.Id == id);
            } else {
                lock (_locker) {
                    if (_identifierQueue.TryDequeue(out var nextTaskEntity)) {
                        _currentTask = nextTaskEntity;
                    }
                }
            }

            lock (_locker) {
                // Debug.Log($"Lock before task execution: '{id}'");
                _executingTask = true;
            }
            
            //Debug.Log($"Start task execution: '{id}'");
            await _currentTask.Func();
            // Debug.Log($"End task execution: '{id}'");

            lock (_locker) {
                // Debug.Log($"Unlock before task execution: '{id}'");
                _executingTask = false;
                if (_identifierQueue.TryDequeue(out var nextTaskEntity)) {
                    _currentTask = nextTaskEntity;
                }
            }
        }
    }
}