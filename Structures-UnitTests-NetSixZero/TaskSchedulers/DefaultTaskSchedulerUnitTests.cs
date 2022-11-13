using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Structures.NetSixZero.TaskSchedulers;
using TaskExtensions = Structures.NetSixZero.Extension.TaskExtensions;

namespace Structures_UnitTests_NetSixZero.TaskSchedulers {
    internal sealed class DefaultTaskSchedulerUnitTests {

        private DefaultTaskScheduler _taskScheduler;

        private readonly Random _random = new Random();
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


        [Test]
        public async Task ScheduleSingleTest() {
            _source.Enqueue(_random.Next(0, 5));
            await ExecuteTest(_source);
        }
        
        [Test]
        public async Task ScheduleMultipleTest() {
            _source.Enqueue(15);
            _source.Enqueue(6);
            _source.Enqueue(13);
            _source.Enqueue(4);
            _source.Enqueue(10);
            _source.Enqueue(1);
            await ExecuteTest(_source);
        }

        private async Task ExecuteTest(IEnumerable<int> values) {
            // Debug.Log("Create callers");
            var taskCallers = values
                              .Select(v => new AsyncTaskCaller(_taskScheduler, _results, v))
                              .ToArray();
            
            // Debug.Log("Schedule tasks");
            foreach (var taskCaller in taskCallers) {
                taskCaller.ScheduleTask();
            }

            // Debug.Log("Await all callers ready");
            await TaskExtensions.WaitUntil(() => taskCallers.All(t => t.Ready));
            
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

            internal async void ScheduleTask() {
                await _taskScheduler.Execute(ConcurrentTask);
            }
            
            private async Task ConcurrentTask() {
                await Task.Delay(_value * 50);
                _results.Enqueue(_value);

                Ready = true;
            }
        }

#endregion
        
    }
}