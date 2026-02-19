using System;
using Sirb.CepBrasil.Extensions;

Console.WriteLine("CepExtension quick check");
var inputs = new[] { "01310-100", "01310100", "   ", "", null, "123", "01-310.100" };

foreach (var input in inputs)
{
    Console.WriteLine($"Input: '{input ?? "(null)"}'");
    Console.WriteLine($"  RemoveMask: '{input.RemoveMask()}'");
    Console.WriteLine($"  CepMask: '{input.CepMask()}'");
    Console.WriteLine($"  IsValidCep: {input.IsValidCep()}");
    Console.WriteLine($"  Format: '{input.Format()}'");
    Console.WriteLine($"  Normalize: '{input.Normalize()}'");
    Console.WriteLine();
}
