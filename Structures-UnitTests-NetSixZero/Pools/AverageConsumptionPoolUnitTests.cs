using NUnit.Framework;
using Structures.NetSixZero.Pools;

namespace Structures_UnitTests_NetSixZero.Pools {
    internal class AverageConsumptionPoolUnitTests {
        private class TestElement {
            public int Value { get; set; }
        }

        /// <summary>
        /// Проверяет получение, возврат и удаление элементов.
        /// </summary>
        [Test]
        public void GetReturnTest() {
            int counter = 0;
            TestElement CreateElement() {
                return new TestElement() { Value = counter++ };
            }

            void RemoveElement(TestElement element) {
                element.Value = -1;
            }

            var pool = new AverageConsumptionPool<TestElement>(new AverageConsumptionPool<TestElement>.Settings() { 
                CreateFunction = CreateElement,
                RemoveAction = RemoveElement,
                SizeControlDepth = 4,
                StartSize = 1
            });

            var p0 = pool.Get();
            var p1 = pool.Get();
            Assert.AreEqual(0, p0.Value);
            Assert.AreEqual(1, p1.Value);

            pool.Return(p1);
            var p1NextUse = pool.Get();
            Assert.AreEqual(1, p1NextUse.Value);

            pool.Return(p0); // Возвращается.
            pool.Return(p1NextUse); // Удаляется.
            Assert.AreEqual(-1, p1NextUse.Value);

            pool.Dispose();
            Assert.AreEqual(-1, p0.Value);
        }

        /// <summary>
        /// Проверяет вызов Get действия.
        /// </summary>
        [Test]
        public void GetActionTest() {
            void GetAction(TestElement element) {
                element.Value = -1;
            }

            var pool = new AverageConsumptionPool<TestElement>(new AverageConsumptionPool<TestElement>.Settings() {
                CreateFunction = () => new TestElement() { Value = 1 },
                GetAction = GetAction,
                SizeControlDepth = 4,
                StartSize = 2
            });

            var p = pool.Get();
            Assert.AreEqual(-1, p.Value);
        }

        /// <summary>
        /// Проверяет вызов Release действия.
        /// </summary>
        [Test]
        public void ReleaseActionTest() {
            void ReleaseAction(TestElement element) {
                element.Value = -1;
            }

            var pool = new AverageConsumptionPool<TestElement>(new AverageConsumptionPool<TestElement>.Settings() {
                CreateFunction = () => new TestElement() { Value = 1 },
                ReleaseAction = ReleaseAction,
                SizeControlDepth = 4,
                StartSize = 2
            });

            var p = pool.Get();
            Assert.AreEqual(1, p.Value);

            pool.Return(p);
            var pNext = pool.Get();
            Assert.AreEqual(-1, p.Value);
        }

        /// <summary>
        /// Проверяет пересчет размера.
        /// </summary>
        [Test]
        public void SizeTest() {
            var pool = new AverageConsumptionPool<int>(new AverageConsumptionPool<int>.Settings() {
                CreateFunction = () => 2,
                SizeControlDepth = 3,
                StartSize = 2
            });

            pool.Get();
            pool.Return(2);
            pool.ResetCycle();

            // (2) / 1 = 2.
            Assert.AreEqual(2, pool.Size);

            pool.Get();
            pool.Get();
            pool.Return(2);
            pool.Return(2);
            pool.ResetCycle();

            // (2 + 2) / 2 = 2.
            Assert.AreEqual(2, pool.Size);

            pool.Get();
            pool.Get();
            pool.Get();
            pool.Get();
            pool.Return(2);
            pool.Return(2);
            pool.ResetCycle();

            // (4 + 2 + 2) / 3 = 3.
            Assert.AreEqual(3, pool.Size);

            pool.Get();
            pool.Get();
            pool.Get();
            pool.Return(2);
            pool.Return(2);
            pool.Return(2);
            pool.Return(2);
            pool.Return(2);
            pool.ResetCycle();

            // (5 + 4 + 2) / 3 = 11/3 = 4.
            Assert.AreEqual(4, pool.Size);


            pool.ResetCycle();

            // (0 + 5 + 4) / 3 = 3.
            Assert.AreEqual(3, pool.Size);
        }
    }
}
