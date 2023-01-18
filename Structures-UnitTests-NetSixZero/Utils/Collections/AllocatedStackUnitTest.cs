using System;
using BenchmarkDotNet.Attributes;
using JetBrains.dotMemoryUnit;
using NUnit.Framework;
using Structures.NetSixZero.Structures.Collections.NonAllocated;
using Structures.NetSixZero.Utils.Collections;

namespace Structures_UnitTests_NetSixZero.Utils.Collections; 

internal sealed class AllocatedStackUnitTest {
    private readonly record struct TestStruct(byte Byte, short Short, int Int);

    [TestCase(10)]
    [TestCase(100)]
    [TestCase(1000)]
    public void TestMemoryUsage(int size) {
        Span<TestStruct> span = stackalloc TestStruct[size];
        var stack = new NonAllocatedStack<TestStruct>(ref span);

        TestStruct value = default;
        for (int i = 0; i < size; i++) {
            value = new TestStruct((byte)(i % byte.MaxValue), (short)i, i);
            stack.Push(ref value);
        }

        value = stack.Peek();
        Assert.AreEqual(value.Int, size -1);
        
        if (stack.TryPeek(out value)) {
            Assert.AreEqual(value.Int, size - 1);
        } else {
            Assert.Fail();
        }
        
        for (int i = size - 1; i >= 1; i--) {
            if (stack.TryPop(out value)) {
                Assert.AreEqual(value.Int, i);
            } else {
                Assert.Fail();
            }
        }

        value = stack.Pop();
        Assert.AreEqual(value.Byte, byte.MinValue);
    }
}