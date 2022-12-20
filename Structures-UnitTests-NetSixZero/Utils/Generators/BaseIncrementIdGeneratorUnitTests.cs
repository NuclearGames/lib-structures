using NUnit.Framework;
using Structures.NetSixZero.Utils.Generators;
using System.Collections.Generic;

namespace Structures_UnitTests_NetSixZero.Utils.Generators {
    internal class BaseIncrementIdGeneratorUnitTests {
        private class TestIncrementGenerator : BaseIncrementIdGenerator<int> {
            internal TestIncrementGenerator(int initial) : base(initial) { }
            protected override int Increment(int x) {
                unchecked {
                    return x + 1;
                }
            }
        }

        [Test]
        public void GetNextTest() {
            var generator = new TestIncrementGenerator(0);

            var ids = new HashSet<int>();
            for(int i = 0; i < 100; i++) {
                Assert.That(ids.Add(generator.GetNext()));
            }

            generator = new TestIncrementGenerator(int.MaxValue - 1);
            ids.Clear();

            Assert.That(generator.GetNext(), Is.EqualTo(int.MaxValue - 1));
            Assert.That(generator.GetNext(), Is.EqualTo(int.MaxValue));
            Assert.That(generator.GetNext(), Is.EqualTo(int.MinValue));
            Assert.That(generator.GetNext(), Is.EqualTo(int.MinValue + 1));
        }
    }
}
