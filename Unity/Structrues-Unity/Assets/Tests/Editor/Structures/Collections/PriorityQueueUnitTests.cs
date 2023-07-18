using NuclearGames.StructuresUnity.Structures.Collections;
using NUnit.Framework;
using System.Collections.Generic;

namespace Tests.Editor.Structures.Collections {
    public class PriorityQueueUnitTests {
        [TestCaseSource(nameof(TestCaseSource))]
        public void DequeuePriorityTest(KeyValuePair<string, int>[] forEnqueue, int[] priorityOutput) {
            var queue = new PriorityQueue<string, int>();

            foreach(var pair  in forEnqueue) {
                queue.Enqueue(pair.Key, pair.Value);
            }

            int counter = 0;
            while (queue.TryDequeue(out _, out var priority)) {
                Assert.AreEqual(priorityOutput[counter], priority);
                counter++;
            }
        }

        private static object[] TestCaseSource = {
            new object[] {
                // Enqueued elements.
                new KeyValuePair<string, int>[] {
                    new KeyValuePair<string, int>("Third", 3),
                    new KeyValuePair<string, int>("First", 1),
                    new KeyValuePair<string, int>("Second", 2),
                    new KeyValuePair<string, int>("Fifth", 5),
                    new KeyValuePair<string, int>("Fourth", 4)
                },

                // Priority expected sequence.
                new int[] {
                    1, 2, 3, 4, 5
                }
            }
        };
    }
}