```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4460/23H2/2023Update/SunValley3)
AMD Ryzen 7 5700G with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2


```
| Method                    | Mean     | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
|-------------------------- |---------:|----------:|----------:|------:|--------:|----------:|------------:|
| NetBinaryWriterWrite      | 1.779 ms | 0.0296 ms | 0.0277 ms |  1.00 |    0.02 |     384 B |        1.00 |
| CustomBinaryWriterWrite   | 4.861 ms | 0.0939 ms | 0.0922 ms |  2.73 |    0.06 |     377 B |        0.98 |
| CustomBinaryWriterLeWrite | 4.955 ms | 0.0937 ms | 0.0921 ms |  2.79 |    0.07 |     377 B |        0.98 |
| CustomBinaryWriterBeWrite | 4.975 ms | 0.0947 ms | 0.1090 ms |  2.80 |    0.07 |     377 B |        0.98 |
