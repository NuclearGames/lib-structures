using Cysharp.Threading.Tasks;
using NuclearGames.StructuresUnity.Structures.Collections;

namespace NuclearGames.StructuresUnity.TaskSchedulers {
    public sealed class DefaultTaskScheduler {
        private readonly struct QueuedTaskEntity {
            internal readonly int Id;
            internal readonly UniTask Task;
            
            public QueuedTaskEntity(int id, UniTask task) {
                Id = id;
                Task = task;
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
        public async UniTask Execute(UniTask requestedTask) {
            int id;
            
            lock (_locker) {
                unchecked {
                    id = ++_idIncrementor;
                }
                
                var taskEntity = new QueuedTaskEntity(id, requestedTask);
                _identifierQueue.Enqueue(taskEntity);
            }

            if (_executingTask) {
                await UniTask.WaitUntil(() => _currentTask.Id == id);
            }

            lock (_locker) {
                _executingTask = true;
            }
            
            await _currentTask.Task;

            lock (_locker) {
                _executingTask = false;
                if (_identifierQueue.TryDequeue(out var nextTaskEntity)) {
                    _currentTask = nextTaskEntity;
                }
            }
        }
    }
}