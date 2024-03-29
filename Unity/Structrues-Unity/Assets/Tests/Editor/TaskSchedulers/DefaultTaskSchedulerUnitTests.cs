﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using NuclearGames.StructuresUnity.TaskSchedulers;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace NuclearGames.StructuresUnity.Tests.Editor.TaskSchedulers {
    internal sealed class DefaultTaskSchedulerUnitTests {

        private DefaultTaskScheduler _taskScheduler;
        private readonly Queue<int> _source = new Queue<int>();
        private readonly Queue<int> _results = new Queue<int>();

        [SetUp]
        public void SetUp() {
            _taskScheduler = new DefaultTaskScheduler();
        }

        [TearDown]
        public void TearsDown() {
            _source.Clear();
            _results.Clear();
        }


        [UnityTest]
        public IEnumerator ScheduleSingleTest() {
            _source.Enqueue(Random.Range(0, 5));
            return UniTask.ToCoroutine(async () => await ExecuteTest(_source));
        }
        
        [UnityTest]
        public IEnumerator ScheduleMultipleTest() {
            _source.Enqueue(15);
            _source.Enqueue(6);
            _source.Enqueue(13);
            _source.Enqueue(4);
            _source.Enqueue(10);
            _source.Enqueue(1);
            return UniTask.ToCoroutine(async () => await ExecuteTest(_source));
        }

        [UnityTest]
        public IEnumerator ScheduleTest() {
            bool done = false;
            int taskChecker = 0;
            object locker = new object();

            void AssertTask(int taskNumber, bool end) {
                lock (locker) {
                    Assert.AreEqual(end ? taskChecker : (taskChecker+1), taskNumber);
                    taskChecker = taskNumber;
                }
            }

            async UniTask Internal() {
                var scheduler = new DefaultTaskScheduler();

                await scheduler.Execute(async () => {
                    AssertTask(1, false);
                    await UniTask.Delay(20);
                    AssertTask(1, true);
                });

                var task2 = scheduler.Execute(async () => {
                    AssertTask(2, false);
                    await UniTask.Delay(10);
                    AssertTask(2, true);
                });

                var task3 = scheduler.Execute(async () => {
                    AssertTask(3, false);
                    await UniTask.Delay(10);
                    AssertTask(3, true);
                });

                await UniTask.WhenAll(task2, task3);
                done = true;
            }

            Internal().Forget();
            yield return new WaitUntil(() => done);
        }

        private async UniTask ExecuteTest(IEnumerable<int> values) {
            // Debug.Log("Create callers");
            var taskCallers = values
                              .Select(v => new AsyncTaskCaller(_taskScheduler, _results, v))
                              .ToArray();
            
            // Debug.Log("Schedule tasks");
            foreach (var taskCaller in taskCallers) {
                taskCaller.ScheduleTask().Forget();
            }

            // Debug.Log("Await all callers ready");
            await UniTask.WaitUntil(() => taskCallers.All(t => t.Ready));
            
            // Debug.Log("Checkout");
            Assert.AreEqual(_source.Count, _results.Count);
            while (_source.Count > 0) {
                var sourceValue = _source.Dequeue();
                var resultValue = _results.Dequeue();
                Assert.AreEqual(sourceValue, resultValue);
            }
        }

#region Utils

        private class AsyncTaskCaller {
            private readonly int _value;
            private readonly Queue<int> _results;
            private readonly DefaultTaskScheduler _taskScheduler;
            
            internal bool Ready { get; private set; }

            internal AsyncTaskCaller(DefaultTaskScheduler taskScheduler, Queue<int> results, int value) {
                _results = results;
                _value = value;
                _taskScheduler = taskScheduler;
                Ready = false;
            }

            internal async UniTaskVoid ScheduleTask() {
                await _taskScheduler.Execute(ConcurrentTask);
            }
            
            private async UniTask ConcurrentTask() {
                await UniTask.Delay(_value * 50);
                _results.Enqueue(_value);

                Ready = true;
            }
        }

#endregion
        
    }
}