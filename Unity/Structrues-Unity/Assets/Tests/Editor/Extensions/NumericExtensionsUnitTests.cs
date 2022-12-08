using NUnit.Framework;
using NuclearGames.StructuresUnity.Extensions;

namespace Tests.Editor.Extensions {
    internal class NumericExtensionsUnitTests {
        [Test]
        public void LoopTest() {
            const int count = 2;
            Assert.AreEqual(0, 0.Loop(count));
            Assert.AreEqual(1, 1.Loop(count));
            Assert.AreEqual(0, 2.Loop(count));
            Assert.AreEqual(1, (-1).Loop(count));
            Assert.AreEqual(0, (-2).Loop(count));
            Assert.AreEqual(1, (count * 7 + 1).Loop(count));
            Assert.AreEqual(1, (-count * 7 - 1).Loop(count));
        }
    }
}
