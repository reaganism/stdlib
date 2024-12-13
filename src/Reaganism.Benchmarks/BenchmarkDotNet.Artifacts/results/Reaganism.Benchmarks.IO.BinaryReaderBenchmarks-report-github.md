```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4602/23H2/2023Update/SunValley3)
AMD Ryzen 7 5700G with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2


```
| Method                   | Mean     | Error    | StdDev   | Ratio | RatioSD | Allocated | Alloc Ratio |
|------------------------- |---------:|---------:|---------:|------:|--------:|----------:|------------:|
| NetBinaryReaderRead      | 680.2 μs |  8.00 μs |  7.48 μs |  1.00 |    0.02 |     120 B |        1.00 |
| CustomBinaryReaderRead   | 860.1 μs |  7.60 μs |  6.74 μs |  1.26 |    0.02 |      64 B |        0.53 |
| CustomBinaryReaderLeRead | 895.2 μs | 17.71 μs | 15.70 μs |  1.32 |    0.03 |      64 B |        0.53 |
| CustomBinaryReaderBeRead | 905.6 μs |  7.42 μs |  7.28 μs |  1.33 |    0.02 |      64 B |        0.53 |
