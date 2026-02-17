﻿using Xunit;

namespace Sirb.CepBrasil.Test.Extensions
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

            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Theory]
        [InlineData( /*lang=json,strict*/ "{\"uf\":\"TEST\",\"localidade\":\"TEST\",\"bairro\":\"TEST\",\"complemento\":\"TEST\",\"logradouro\":\"TEST\",\"cep\":\"TEST\"}")]
        public void FromJson_Test(string value)
        {
            var result = value.FromJson<CepContainer>();

            Assert.NotNull(result);
        }
    }
}
