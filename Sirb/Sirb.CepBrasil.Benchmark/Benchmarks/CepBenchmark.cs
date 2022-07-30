using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using System.Text;

namespace Sirb.CepBrasil.Benchmark.Benchmarks
{
	[MemoryDiagnoser]
	[Orderer(SummaryOrderPolicy.FastestToSlowest)]
	public class CepBenchmark
	{
		private static readonly string cep = "83040-040";
		private static readonly string body = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:cli=\"http://cliente.bean.master.sigep.bsb.correios.com.br/\"><soapenv:Header/><soapenv:Body><cli:consultaCEP><cep>[cep]</cep></cli:consultaCEP></soapenv:Body></soapenv:Envelope>";

		[Benchmark]
		public static void ReplaceValue()
		{
			_ = body.Replace("[cep]", cep);
		}

		[Benchmark]
		public static void WriteBody()
		{
			var sb = new StringBuilder("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:cli=\"http://cliente.bean.master.sigep.bsb.correios.com.br/\">")
				.Append("<soapenv:Header/>")
				.Append("<soapenv:Body>")
				.Append("<cli:consultaCEP>")
				.Append($"<cep>{cep}</cep>")
				.Append("</cli:consultaCEP>")
				.Append("</soapenv:Body>")
				.Append("</soapenv:Envelope>");

			_ = sb.ToString();
		}
	}
}