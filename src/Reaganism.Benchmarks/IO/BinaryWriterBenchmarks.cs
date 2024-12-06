using System;
using System.IO;

using BenchmarkDotNet.Attributes;

using BinaryWriter = Reaganism.IO.BinaryWriter;

namespace Reaganism.Benchmarks.IO;

[MemoryDiagnoser]
public class BinaryWriterBenchmarks
{
    private const int iterations = 100_000;

    private byte[] testData = [];

    [GlobalSetup]
    public void Setup()
    {
        testData = new byte[iterations];
        new Random(42).NextBytes(testData);
    }

#region Writing
    [Benchmark(Baseline = true)]
    public void NetBinaryWriterWrite()
    {
        using var ms     = new MemoryStream();
        using var writer = new System.IO.BinaryWriter(ms);

        for (var i = 0; i < iterations; i++)
        {
            ms.Position = 0;
            writer.Write(42);
            writer.Write(1234567890UL);
            writer.Write(3.14159);
        }
    }

    [Benchmark]
    public void CustomBinaryWriterWrite()
    {
        using var ms     = new MemoryStream();
        using var writer = new BinaryWriter(ms, false);

        for (var i = 0; i < iterations; i++)
        {
            writer.Position = 0;
            writer.Write(42);
            writer.Write(1234567890UL);
            writer.Write(3.14159);
        }
    }

    [Benchmark]
    public void CustomBinaryWriterLeWrite()
    {
        using var ms     = new MemoryStream();
        using var writer = new BinaryWriter(ms, false);

        for (var i = 0; i < iterations; i++)
        {
            writer.Position = 0;
            writer.Le.Write(42);
            writer.Le.Write(1234567890UL);
            writer.Le.Write(3.14159);
        }
    }

    [Benchmark]
    public void CustomBinaryWriterBeWrite()
    {
        using var ms     = new MemoryStream();
        using var writer = new BinaryWriter(ms, false);

        for (var i = 0; i < iterations; i++)
        {
            writer.Position = 0;
            writer.Be.Write(42);
            writer.Be.Write(1234567890UL);
            writer.Be.Write(3.14159);
        }
    }
#endregion
}