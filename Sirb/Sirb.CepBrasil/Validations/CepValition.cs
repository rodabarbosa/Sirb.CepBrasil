using Sirb.CepBrasil.Exceptions;
using Sirb.CepBrasil.Extensions;
using Sirb.CepBrasil.Messages;

namespace Sirb.CepBrasil.Validations
{
    public static class CepValition
    {
        private const int ZipCodeLength = 8;

        /// <summary>
        /// Validate brazilian zip code to its minimum value standard.
        /// </summary>
        /// <param name="zipCode"></param>
        public static void Validate(string zipCode)
        {
            string value = zipCode?.RemoveMask();
            int valueLength = value?.Length ?? 0;
            ServiceException.When(valueLength != ZipCodeLength, CepMessages.ZipCodeInvalidMessage);
        }
    }
}
