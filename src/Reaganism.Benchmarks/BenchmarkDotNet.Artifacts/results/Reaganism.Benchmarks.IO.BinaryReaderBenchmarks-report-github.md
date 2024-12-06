```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4460/23H2/2023Update/SunValley3)
AMD Ryzen 7 5700G with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2


```
| Method                   | Mean       | Error    | StdDev   | Ratio | RatioSD | Allocated | Alloc Ratio |
|------------------------- |-----------:|---------:|---------:|------:|--------:|----------:|------------:|
| NetBinaryReaderRead      |   730.8 μs | 14.39 μs | 21.97 μs |  1.00 |    0.04 |     120 B |        1.00 |
| CustomBinaryReaderRead   | 1,416.7 μs | 27.91 μs | 33.23 μs |  1.94 |    0.07 |      64 B |        0.53 |
| CustomBinaryReaderLeRead | 1,431.1 μs | 24.42 μs | 23.98 μs |  1.96 |    0.07 |      64 B |        0.53 |
| CustomBinaryReaderBeRead | 1,413.4 μs | 26.83 μs | 23.79 μs |  1.94 |    0.07 |      64 B |        0.53 |
