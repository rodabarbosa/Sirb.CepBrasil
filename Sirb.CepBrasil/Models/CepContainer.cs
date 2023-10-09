using System.Text.Json.Serialization;

namespace Sirb.CepBrasil.Models
{
    /// <summary>
    /// Container para o resultado da busca do CEP
    /// </summary>
    public sealed class CepContainer
    {
        public CepContainer()
        {
        }

        public CepContainer(string uf, string cidade, string bairro, string complemento, string logradouro, string cep)
        {
            Uf = uf;
            Cidade = cidade;
            Bairro = bairro;
            Complemento = complemento;
            Logradouro = logradouro;
            Cep = cep;
        }

        /// <summary>
        /// State abbreviation
        ///
        /// Sigla do estado
        /// </summary>
        [JsonPropertyName("uf")]
        public string Uf { get; set; }

        /// <summary>
        /// City name
        ///
        /// Nome da cidade
        /// </summary>
        [JsonPropertyName("localidade")]
        public string Cidade { get; set; }

        /// <summary>
        /// Neighborhood name
        ///
        /// Nome do bairro
        /// </summary>
        [JsonPropertyName("bairro")]
        public string Bairro { get; set; }

        /// <summary>
        /// Extra information
        ///
        /// Complemento
        /// </summary>
        [JsonPropertyName("complemento")]
        public string Complemento { get; set; }

        /// <summary>
        /// Street name
        ///
        /// Logradouro
        /// </summary>
        [JsonPropertyName("logradouro")]
        public string Logradouro { get; set; }

        /// <summary>
        /// Zip code
        ///
        /// CEP
        /// </summary>
        [JsonPropertyName("cep")]
        public string Cep { get; set; }
    }
}
