using System;
using System.IO;

using BenchmarkDotNet.Attributes;

using BinaryReader = Reaganism.IO.BinaryReader;

namespace Reaganism.Benchmarks.IO;

[MemoryDiagnoser]
public class BinaryReaderBenchmarks
{
    private const int iterations = 100_000;

    private byte[] testData = [];

    [GlobalSetup]
    public void Setup()
    {
        testData = new byte[iterations];
        new Random(42).NextBytes(testData);
    }

#region Reading
    [Benchmark(Baseline = true)]
    public void NetBinaryReaderRead()
    {
        using var ms     = new MemoryStream(testData);
        using var reader = new System.IO.BinaryReader(ms);

        for (var i = 0; i < iterations; i++)
        {
            ms.Position = 0;
            reader.ReadInt32();
            reader.ReadUInt64();
            reader.ReadDouble();
        }
    }

    [Benchmark]
    public void CustomBinaryReaderRead()
    {
        using var ms     = new MemoryStream(testData);
        using var reader = new BinaryReader(ms, true);

        for (var i = 0; i < iterations; i++)
        {
            ms.Position = 0;
            reader.Read<int>();
            reader.Read<ulong>();
            reader.Read<double>();
        }
    }

    [Benchmark]
    public void CustomBinaryReaderLeRead()
    {
        using var ms     = new MemoryStream(testData);
        using var reader = new BinaryReader(ms, true);

        for (var i = 0; i < iterations; i++)
        {
            ms.Position = 0;
            reader.Le.Read<int>();
            reader.Le.Read<ulong>();
            reader.Le.Read<double>();
        }
    }

    [Benchmark]
    public void CustomBinaryReaderBeRead()
    {
        using var ms     = new MemoryStream(testData);
        using var reader = new BinaryReader(ms, true);

        for (var i = 0; i < iterations; i++)
        {
            ms.Position = 0;
            reader.Be.Read<int>();
            reader.Be.Read<ulong>();
            reader.Be.Read<double>();
        }
    }
#endregion
}