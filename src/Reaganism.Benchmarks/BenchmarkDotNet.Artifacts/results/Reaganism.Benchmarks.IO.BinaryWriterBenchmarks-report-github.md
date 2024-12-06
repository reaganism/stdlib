```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4460/23H2/2023Update/SunValley3)
AMD Ryzen 7 5700G with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2


```
| Method                    | Mean     | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
|-------------------------- |---------:|----------:|----------:|------:|--------:|----------:|------------:|
| NetBinaryWriterWrite      | 1.815 ms | 0.0351 ms | 0.0456 ms |  1.00 |    0.03 |     385 B |        1.00 |
| CustomBinaryWriterWrite   | 1.039 ms | 0.0135 ms | 0.0126 ms |  0.57 |    0.02 |     344 B |        0.89 |
| CustomBinaryWriterLeWrite | 1.056 ms | 0.0205 ms | 0.0192 ms |  0.58 |    0.02 |     345 B |        0.90 |
| CustomBinaryWriterBeWrite | 1.094 ms | 0.0186 ms | 0.0222 ms |  0.60 |    0.02 |     345 B |        0.90 |
