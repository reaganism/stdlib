using System;
using System.Collections.Generic;

using BenchmarkDotNet.Attributes;

using Reaganism.IO.Hashing;

namespace Reaganism.Benchmarks.IO.Hashing;

public class Crc32Benchmarks
{
    private byte[][] data = null!;

    private static readonly UncachedPolynomialCrc32Variant1 crc1 = UncachedPolynomialCrc32Variant1.GetInstance(0x82F63B78);
    private static readonly UncachedPolynomialCrc32Variant1 crc2 = UncachedPolynomialCrc32Variant1.GetInstance(0xEDB88320);

    private static readonly List<ICrc32> crcs =
    [
        crc1,
        crc2,
    ];

    [GlobalSetup]
    public void Setup()
    {
        data = GenerateLogarithmicData(1000, 10, 2 * 1024 * 1024);
        
        foreach (var crc in crcs)
        {
            crc.SetupTable();
        }

        return;

        byte[][] GenerateLogarithmicData(int numberOfCases, int lowerBound, int upperBound)
        {
            var logarithmicData = new byte[numberOfCases][];
            var random          = new Random(42);

            for (var i = 0; i < numberOfCases; i++)
            {
                var logarithmicFactor = Math.Pow(upperBound / (double)lowerBound, i / (double)(numberOfCases - 1));

                logarithmicData[i] = new byte[(int)(lowerBound * logarithmicFactor)];
                random.NextBytes(logarithmicData[i]);
            }

            return logarithmicData;
        }
    }

    [Benchmark(Baseline = true)]
    public void SystemIoHashingCrc32()
    {
        foreach (var item in data)
        {
            System.IO.Hashing.Crc32.Hash(item);
        }
    }

    [Benchmark]
    public void Crc1()
    {
        foreach (var item in data)
        {
            crc1.Hash(0, item);
        }
    }

    [Benchmark]
    public void Crc2()
    {
        foreach (var item in data)
        {
            crc2.Hash(0, item);
        }
    }

    [Benchmark]
    public void SlicingBy16()
    {
        return;
    }
}