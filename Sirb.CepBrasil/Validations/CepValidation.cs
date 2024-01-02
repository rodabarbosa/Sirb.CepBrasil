using Sirb.CepBrasil.Exceptions;
using Sirb.CepBrasil.Extensions;
using Sirb.CepBrasil.Messages;

namespace Sirb.CepBrasil.Validations
{
    static internal class CepValidation
    {
        private const int ZipCodeLength = 8;

        /// <summary>
        ///     Validate brazilian zip code to its minimum value standard.
        /// </summary>
        /// <param name="zipCode"></param>
        static public void Validate(string zipCode)
        {
            var value = zipCode?.RemoveMask();

            var valueLength = value?.Length ?? 0;

            ServiceException.ThrowIf(valueLength != ZipCodeLength, CepMessages.ZipCodeInvalidMessage);
        }
    }
}
