using MarketPrice.Domain.Authentication.Commands;
using System.Net.Http.Json;

namespace MarketPrice.Ui.Services.Api
{
    public class AuthenticationApiService
    {
        private readonly HttpClient _httpClient;

        public AuthenticationApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> RegisterUserAsync(RegisterCommand registerCommand)
        {
            var response = await _httpClient.PostAsJsonAsync("ApplicationUsers/auth/register", registerCommand);
            return response;
        }

        public async Task<HttpResponseMessage> LoginUserAsync(LoginCommand loginCommand)
        {
            var response = await _httpClient.PostAsJsonAsync("ApplicationUsers/auth/login", loginCommand);
            return response;
        }

        public async Task<HttpResponseMessage> LogoutUserAsync(LogoutCommand logoutCommand)
        {
            var response = await _httpClient.PostAsJsonAsync("ApplicationUsers/auth/logout", logoutCommand);
            return response;
        }
    }
}
