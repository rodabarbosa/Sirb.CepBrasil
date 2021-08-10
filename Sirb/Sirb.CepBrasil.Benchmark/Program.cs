using BenchmarkDotNet.Running;
using Sirb.CepBrasil.Benchmark.Benchmarks;

namespace Sirb.CepBrasil.Benchmark
{
	class Program
	{
		static void Main(string[] args)
		{
			BenchmarkRunner.Run<CepBenchmark>();
		}
	}
}
