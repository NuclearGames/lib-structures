using NuclearGames.StructuresUnity.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Editor.Extensions {
    internal class FlagsExtensionsUnitTests {
        [Flags]
        private enum TestFlags {
            None = 0x0,
            First = 0x1,
            Second = 0x2,
            All = ~0
        }

        [Test]
        public void GetFlagsTest() {
            var maskS = TestFlags.Second;
            var maskFS = TestFlags.First | TestFlags.Second;
            var maskNone = TestFlags.None;

            var flagsS = maskS.GetFlags().ToArray();
            var flagsFS = maskFS.GetFlags().ToArray();
            var flagsNone = maskNone.GetFlags().ToArray();

            Assert.That(flagsS.Length, Is.EqualTo(1));
            Assert.That(flagsFS.Length, Is.EqualTo(2));
            Assert.That(flagsNone.Length, Is.EqualTo(0));

            Assert.That(flagsS[0], Is.EqualTo(TestFlags.Second));
            Assert.That(flagsFS[0], Is.EqualTo(TestFlags.First));
            Assert.That(flagsFS[1], Is.EqualTo(TestFlags.Second));
        }
    }
}
