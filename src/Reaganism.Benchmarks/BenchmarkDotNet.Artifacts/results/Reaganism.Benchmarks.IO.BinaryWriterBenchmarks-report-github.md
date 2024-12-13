```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4602/23H2/2023Update/SunValley3)
AMD Ryzen 7 5700G with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2


```
| Method                    | Mean     | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
|-------------------------- |---------:|----------:|----------:|------:|----------:|------------:|
| NetBinaryWriterWrite      | 1.722 ms | 0.0098 ms | 0.0076 ms |  1.00 |     384 B |        1.00 |
| CustomBinaryWriterWrite   | 1.069 ms | 0.0209 ms | 0.0205 ms |  0.62 |     344 B |        0.90 |
| CustomBinaryWriterLeWrite | 1.034 ms | 0.0138 ms | 0.0122 ms |  0.60 |     344 B |        0.90 |
| CustomBinaryWriterBeWrite | 1.048 ms | 0.0116 ms | 0.0103 ms |  0.61 |     344 B |        0.90 |
