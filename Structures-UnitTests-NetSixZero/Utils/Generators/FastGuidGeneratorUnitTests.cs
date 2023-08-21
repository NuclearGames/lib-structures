using NUnit.Framework;
using Structures.NetSixZero.Utils.Generators;
using System;
using System.Collections.Generic;

namespace Structures_UnitTests_NetSixZero.Utils.Generators {
    internal class FastGuidGeneratorUnitTests {
        [TestCase(10_000)]
        public void Generate_ReturnsUniqueGuids(int count) {
            var guids = new HashSet<Guid>();
            for (int i = 0; i < count; i++) {
                Assert.That(guids.Add(FastGuidGenerator.Generate()));
            }
        }
    }
}
