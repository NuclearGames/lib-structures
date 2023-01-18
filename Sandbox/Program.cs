// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Sandbox;

[MemoryDiagnoser]
public class Program {
    
    [Benchmark()]
    public void TestMemoryUsage() {
        int size = 100000;
        
        Span<TestStruct> span = stackalloc TestStruct[size];
        var stack = new AllocatedStack<TestStruct>(ref span);

        TestStruct value = default;
        for (int i = 0; i < size; i++) {
            value = new TestStruct((byte)(i % byte.MaxValue), (short)i, i);
            stack.Push(ref value);
        }

        value = stack.Peek();
        if (value.Int != size - 1) {
            throw new ArgumentException();
        }

        if (stack.TryPeek(out value)) {
            if (value.Int != size - 1) {
                throw new ArgumentException();
            }   
        } else {
            throw new ArgumentException();
        }
        

        for (int i = size - 1; i >= 1; i--) {
            if (stack.TryPop(out value)) {
                if (value.Int != i) {
                    throw new ArgumentException();
                }
            } else {
                throw new ArgumentException();
            }
        }

        value = stack.Pop();
        if (value.Byte != 0) {
            throw new ArgumentException();
        }
    }

    public record struct TestStruct(byte Byte, short Short, int Int);
    
    static void Main(string[] args) {
        var summary = BenchmarkRunner.Run<Program>();
    }
}
