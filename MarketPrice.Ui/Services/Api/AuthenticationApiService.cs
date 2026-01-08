using System.Net.Http.Headers;
using MarketPrice.Domain.Authentication.Commands;
using System.Net.Http.Json;
using MarketPrice.Domain;
using MarketPrice.Ui.Services.Session;

namespace MarketPrice.Ui.Services.Api
{
    public class AuthenticationApiService(HttpClient httpClient, SessionStorage sessionStorage) : BaseApiService(httpClient, sessionStorage)
    {
        public async Task<HttpResponseMessage> RegisterUserAsync(RegisterCommand registerCommand)
        {
            var url = ApiControllers.ApplicationUsers.AppendRoute(ApiRoutes.AUTH_REGISTER);
            var response = await PostAsync(url, registerCommand);
            return response;
        }

        public async Task<HttpResponseMessage> LoginUserAsync(LoginCommand loginCommand)
        {
            var url = ApiControllers.ApplicationUsers.AppendRoute(ApiRoutes.AUTH_LOGIN);
            var response = await PostAsync(url, loginCommand);
            return response;
        }

        public async Task<HttpResponseMessage> LogoutUserAsync(LogoutCommand logoutCommand)
        {
            var url = ApiControllers.ApplicationUsers.AppendRoute(ApiRoutes.AUTH_LOGOUT);
            var response = await PostAsync(url, logoutCommand);
            return response;
        }

        public async Task<HttpResponseMessage> RefreshTokenAsync(string refreshToken)
        {
            var url = ApiControllers.ApplicationUsers.AppendRoute(ApiRoutes.AUTH_REFRESH_TOKEN);
            var response = await PostAsync(url, refreshToken);
            return response;
        }
    }

    public class BaseApiService(HttpClient httpClient, SessionStorage sessionStorage)
    {
        public HttpClient HttpClient { get; } = httpClient;
        public SessionStorage SessionStorage { get; } = sessionStorage;

        // create base method to send post requests
        public async Task<HttpResponseMessage> PostAsync(string url, object data)
        {
            // before posting, append the bearer token if available
            var sessionStorage = await SessionStorage.LoadAsync();
            if (sessionStorage != null && !string.IsNullOrEmpty(sessionStorage.AccessToken))
            {
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessionStorage.AccessToken);
            }
            return await HttpClient.PostAsJsonAsync(url, data);
        }

    }


}
