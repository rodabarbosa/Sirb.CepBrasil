using BenchmarkDotNet.Running;
using Sirb.CepBrasil.Benchmark.Benchmarks;

namespace Sirb.CepBrasil.Benchmark;

static internal class Program
{
    static private void Main()
    {
        BenchmarkRunner.Run<CepBenchmark>();
    }
}
