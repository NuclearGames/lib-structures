using NUnit.Framework;
using Structures.NetSixZero.Pools;

namespace Structures_UnitTests_NetSixZero.Pools {
    internal class AverageConsumptionPoolUnitTests {
        [Test]
        public void Test() {
            var pool = new AverageConsumptionPool<int>(() => 2, null);

            pool.Get();
            pool.Get();
            pool.Return(2);
            pool.Return(2);
            pool.StartCycle();

            // (0 + 0 + 0 + 2) / 4 = 1/2 = 1.
            Assert.AreEqual(1, pool.Size);

            pool.Get();
            pool.Get();
            pool.Get();
            pool.Get();
            pool.Return(2);
            pool.Return(2);
            pool.StartCycle();

            // (0 + 0 + 4 + 2) / 4 = 3/2 = 2.
            Assert.AreEqual(2, pool.Size);

            pool.Get();
            pool.Get();
            pool.Get();
            pool.Get();
            pool.Return(2);
            pool.Return(2);
            pool.Return(2);
            pool.Return(2);
            pool.Return(2);
            pool.Return(2);
            pool.StartCycle();

            // (0 + 6 + 4 + 2) / 4 = 3.
            Assert.AreEqual(3, pool.Size);
        }
    }
}
