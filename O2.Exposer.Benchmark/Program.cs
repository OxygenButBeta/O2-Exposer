using BenchmarkDotNet.Running;
internal class Program {
    public static void Main(string[] args) {
        BenchmarkRunner.Run<ExposedMemberBenchmarks>();
    }
}

