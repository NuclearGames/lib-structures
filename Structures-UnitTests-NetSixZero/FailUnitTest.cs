using NUnit.Framework;


namespace Structures_UnitTests_NetSixZero; 


public class FailUnitTest {
    
    [Test]
    public void FailTest() {
        Assert.Fail();
    }
}