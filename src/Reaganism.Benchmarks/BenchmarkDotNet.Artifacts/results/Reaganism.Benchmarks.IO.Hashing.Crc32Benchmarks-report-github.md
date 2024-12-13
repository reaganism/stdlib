```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4602/23H2/2023Update/SunValley3)
AMD Ryzen 7 5700G with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2


```
| Method               | Mean     | Error    | StdDev   | Ratio | RatioSD |
|--------------------- |---------:|---------:|---------:|------:|--------:|
| SystemIoHashingCrc32 | 13.88 ms | 0.277 ms | 0.577 ms |  1.00 |    0.06 |
