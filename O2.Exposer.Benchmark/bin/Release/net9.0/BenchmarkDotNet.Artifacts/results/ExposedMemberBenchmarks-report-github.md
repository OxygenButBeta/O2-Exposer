```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26100.4652/24H2/2024Update/HudsonValley)
Unknown processor
.NET SDK 9.0.300
  [Host]     : .NET 9.0.5 (9.0.525.21509), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  DefaultJob : .NET 9.0.5 (9.0.525.21509), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI


```
| Method                   | Mean      | Error     | StdDev    | Allocated |
|------------------------- |----------:|----------:|----------:|----------:|
| ExposedMemberGet         | 0.2135 ns | 0.0141 ns | 0.0132 ns |         - |
| FieldInfoGet             | 2.3033 ns | 0.0253 ns | 0.0225 ns |         - |
| ExposedMemberSet         | 1.2159 ns | 0.0149 ns | 0.0132 ns |         - |
| FieldInfoSet             | 5.2243 ns | 0.0303 ns | 0.0283 ns |         - |
| ExposedMemberPropertySet | 1.2181 ns | 0.0167 ns | 0.0157 ns |         - |
| PropertyInfoSet          | 9.8410 ns | 0.0709 ns | 0.0663 ns |         - |
| ExposedMemberPropertyGet | 0.8012 ns | 0.0045 ns | 0.0037 ns |         - |
| PropertyInfoGet          | 6.6460 ns | 0.0517 ns | 0.0484 ns |         - |
