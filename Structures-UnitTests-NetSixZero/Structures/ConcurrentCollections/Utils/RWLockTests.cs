using System;
using NUnit.Framework;
using Structures.NetSixZero.Structures.ConcurrentCollections.Utils;

namespace Structures_UnitTests_NetSixZero.Structures.ConcurrentCollections.Utils {
    public class RWLockTests {
        [Test]
        public void WaitTimeCastTest() {
            RWLock.WaitTime waitTime1 = TimeSpan.FromMilliseconds(1000);
            RWLock.WaitTime waitTime2 = 1000;
            Assert.AreEqual(waitTime1, waitTime2);

            RWLock.WaitTime waitTime3 = -1;
            Assert.AreEqual(-1, (int) waitTime3);
            
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(1000);
            RWLock.WaitTime waitTime4 = timeSpan;
            Assert.AreEqual((int) timeSpan.TotalMilliseconds, (int) waitTime4);
        }
    }
}