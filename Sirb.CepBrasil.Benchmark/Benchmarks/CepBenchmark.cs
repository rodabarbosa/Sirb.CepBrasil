using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace Sirb.CepBrasil.Benchmark.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public sealed class CepBenchmark
{
    private const string Cep = "83040-040";

    private const string Body =
        "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:cli=\"http://cliente.bean.master.sigep.bsb.correios.com.br/\"><soapenv:Header/><soapenv:Body><cli:consultaCEP><cep>[cep]</cep></cli:consultaCEP></soapenv:Body></soapenv:Envelope>";

    protected CepBenchmark()
    {
    }

    [Benchmark]
    public void ReplaceValue()
    {
        _ = Body.Replace("[cep]", Cep);
    }

    [Benchmark]
    public void WriteBody()
    {
        var sb = new StringBuilder("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:cli=\"http://cliente.bean.master.sigep.bsb.correios.com.br/\">")
            .Append("<soapenv:Header/>")
            .Append("<soapenv:Body>")
            .Append("<cli:consultaCEP>")
            .Append($"<cep>{Cep}</cep>")
            .Append("</cli:consultaCEP>")
            .Append("</soapenv:Body>")
            .Append("</soapenv:Envelope>");

        _ = sb.ToString();
    }
}
