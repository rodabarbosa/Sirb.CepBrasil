﻿namespace Sirb.CepBrasil.Test.Extensions
{
    public class JsonExtensionTest
    {
        [Fact]
        public void ToJson_Test()
        {
            var container = new CepContainer("TEST",
                "TEST",
                "TEST",
                "TEST",
                "TEST",
                "TEST"
            );

            var result = container.ToJson();

            result.Should()
                .NotBeNull()
                .And
                .NotBeEmpty();
        }

        [Theory]
        [InlineData( /*lang=json,strict*/ "{\"uf\":\"TEST\",\"localidade\":\"TEST\",\"bairro\":\"TEST\",\"complemento\":\"TEST\",\"logradouro\":\"TEST\",\"cep\":\"TEST\"}")]
        public void FromJson_Test(string value)
        {
            var result = value.FromJson<CepContainer>();

            result.Should()
                .NotBeNull();
        }
    }
}
