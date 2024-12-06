```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4460/23H2/2023Update/SunValley3)
AMD Ryzen 7 5700G with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2


```
| Method                    | Mean     | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
|-------------------------- |---------:|----------:|----------:|------:|--------:|----------:|------------:|
| NetBinaryWriterWrite      | 1.802 ms | 0.0341 ms | 0.0405 ms |  1.00 |    0.03 |     384 B |        1.00 |
| CustomBinaryWriterWrite   | 1.043 ms | 0.0153 ms | 0.0136 ms |  0.58 |    0.01 |     344 B |        0.90 |
| CustomBinaryWriterLeWrite | 1.040 ms | 0.0060 ms | 0.0053 ms |  0.58 |    0.01 |     344 B |        0.90 |
| CustomBinaryWriterBeWrite | 1.057 ms | 0.0173 ms | 0.0162 ms |  0.59 |    0.02 |     345 B |        0.90 |
