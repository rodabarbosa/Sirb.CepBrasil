using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using System.Text;

namespace Sirb.CepBrasil.Benchmark.Benchmarks
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    public sealed class CepBenchmark
    {
        private const string Cep = "83040-040";

        private const string Body =
            "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:cli=\"http://cliente.bean.master.sigep.bsb.correios.com.br/\"><soapenv:Header/><soapenv:Body><cli:consultaCEP><cep>[cep]</cep></cli:consultaCEP></soapenv:Body></soapenv:Envelope>";

        [Benchmark]
#pragma warning disable CA1822
        public void ReplaceValue()
#pragma warning restore CA1822
        {
            _ = Body.Replace("[cep]", Cep);
        }

        [Benchmark]
#pragma warning disable CA1822
        public void WriteBody()
#pragma warning restore CA1822
        {
            var sb = new StringBuilder("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:cli=\"http://cliente.bean.master.sigep.bsb.correios.com.br/\">")
                .Append("<soapenv:Header/>")
                .Append("<soapenv:Body>")
                .Append("<cli:consultaCEP>")
                .AppendFormat("<cep>{0}</cep>", Cep)
                .Append("</cli:consultaCEP>")
                .Append("</soapenv:Body>")
                .Append("</soapenv:Envelope>");

            _ = sb.ToString();
        }
    }
}
