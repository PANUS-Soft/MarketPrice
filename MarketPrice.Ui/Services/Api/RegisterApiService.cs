using MarketPrice.Domain.Authentication.Commands;
using System.Net.Http.Json;

namespace MarketPrice.Ui.Services.Api
{
    public class RegisterApiService
    {
        private readonly HttpClient _httpClient;

        public RegisterApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> RegisterUserAsync(RegisterCommand registerCommand)
        {
            var response = await _httpClient.PostAsJsonAsync("api/ApplicationUsers/auth/register", registerCommand);
            return response;
        }
    }
}
