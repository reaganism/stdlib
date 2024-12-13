using System;
using System.Collections.Generic;

using BenchmarkDotNet.Attributes;

namespace Reaganism.Benchmarks.IO.Hashing;

public class Crc32Benchmarks
{
    private byte[][] data = null!;

    [GlobalSetup]
    public void Setup()
    {
        data = GenerateLogarithmicData(1000, 10, 2 * 1024 * 1024);

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
}