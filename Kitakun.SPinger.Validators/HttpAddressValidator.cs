namespace Kitakun.SPinger.Validators
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    public static class HttpAddressValidator
    {
        public static async Task<ValidationResponse> ValidateAsync(string address)
        {
            var uriAddressIsCorrect = Uri.TryCreate(address, UriKind.RelativeOrAbsolute, out var uriResult);

            if (!uriAddressIsCorrect)
            {
                return ValidationResponse.InvalidAddress;
            }

            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(uriResult);
                    response.EnsureSuccessStatusCode();

                    return ValidationResponse.Validated;
                }
            }
            catch
            {
                return ValidationResponse.ServerUnavailable;
            }
        }
    }

    public enum ValidationResponse : byte
    {
        /// <summary>
        /// Has error in Address text
        /// </summary>
        InvalidAddress = 0,
        /// <summary>
        /// Address is valid, but server is not online
        /// </summary>
        ServerUnavailable = 1,
        /// <summary>
        /// Server is on, address is fine
        /// </summary>
        Validated = 2
    }
}
