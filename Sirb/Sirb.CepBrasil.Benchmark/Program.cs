using BenchmarkDotNet.Running;
using Sirb.CepBrasil.Benchmark.Benchmarks;

namespace Sirb.CepBrasil.Benchmark
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkRunner.Run<CepBenchmark>();
        }
    }
}
