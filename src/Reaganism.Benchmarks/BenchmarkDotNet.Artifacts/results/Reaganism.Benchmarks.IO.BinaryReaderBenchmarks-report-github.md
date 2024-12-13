```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4602/23H2/2023Update/SunValley3)
AMD Ryzen 7 5700G with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2


```
| Method                   | Mean     | Error    | StdDev   | Ratio | RatioSD | Allocated | Alloc Ratio |
|------------------------- |---------:|---------:|---------:|------:|--------:|----------:|------------:|
| NetBinaryReaderRead      | 681.6 μs | 10.32 μs |  9.65 μs |  1.00 |    0.02 |     120 B |        1.00 |
| CustomBinaryReaderRead   | 865.8 μs | 17.17 μs | 19.78 μs |  1.27 |    0.03 |      64 B |        0.53 |
| CustomBinaryReaderLeRead | 875.4 μs |  2.37 μs |  1.98 μs |  1.28 |    0.02 |      64 B |        0.53 |
| CustomBinaryReaderBeRead | 883.8 μs | 17.03 μs | 15.93 μs |  1.30 |    0.03 |      64 B |        0.53 |
