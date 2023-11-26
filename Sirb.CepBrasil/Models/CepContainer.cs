using System.Text.Json.Serialization;

namespace Sirb.CepBrasil.Models
{
    /// <summary>
    /// Container para o resultado da busca do CEP
    /// </summary>
    public sealed class CepContainer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="uf"></param>
        /// <param name="cidade"></param>
        /// <param name="bairro"></param>
        /// <param name="complemento"></param>
        /// <param name="logradouro"></param>
        /// <param name="cep"></param>
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
        public string Uf { get; }

        /// <summary>
        /// City name
        ///
        /// Nome da cidade
        /// </summary>
        [JsonPropertyName("localidade")]
        public string Cidade { get; }

        /// <summary>
        /// Neighborhood name
        ///
        /// Nome do bairro
        /// </summary>
        [JsonPropertyName("bairro")]
        public string Bairro { get; }

        /// <summary>
        /// Extra information
        ///
        /// Complemento
        /// </summary>
        [JsonPropertyName("complemento")]
        public string Complemento { get; }

        /// <summary>
        /// Street name
        ///
        /// Logradouro
        /// </summary>
        [JsonPropertyName("logradouro")]
        public string Logradouro { get; }

        /// <summary>
        /// Zip code
        ///
        /// CEP
        /// </summary>
        [JsonPropertyName("cep")]
        public string Cep { get; }
    }
}
