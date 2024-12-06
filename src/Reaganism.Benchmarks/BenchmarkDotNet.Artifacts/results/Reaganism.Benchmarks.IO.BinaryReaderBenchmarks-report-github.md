```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4460/23H2/2023Update/SunValley3)
AMD Ryzen 7 5700G with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2


```
| Method                   | Mean       | Error    | StdDev   | Ratio | RatioSD | Allocated | Alloc Ratio |
|------------------------- |-----------:|---------:|---------:|------:|--------:|----------:|------------:|
| NetBinaryReaderRead      |   719.5 μs | 14.37 μs | 27.00 μs |  1.00 |    0.05 |     120 B |        1.00 |
| CustomBinaryReaderRead   | 1,704.6 μs | 32.92 μs | 36.59 μs |  2.37 |    0.10 |      33 B |        0.28 |
| CustomBinaryReaderLeRead | 1,859.6 μs | 35.19 μs | 41.89 μs |  2.59 |    0.11 |      32 B |        0.27 |
| CustomBinaryReaderBeRead | 1,878.2 μs | 26.71 μs | 23.68 μs |  2.61 |    0.10 |      32 B |        0.27 |
