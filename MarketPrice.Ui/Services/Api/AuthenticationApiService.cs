using System.Net.Http.Headers;
using MarketPrice.Domain.Authentication.Commands;
using System.Net.Http.Json;
using MarketPrice.Domain;
using MarketPrice.Ui.Common;
using MarketPrice.Ui.Services.Session;
using Microsoft.Extensions.Options;

namespace MarketPrice.Ui.Services.Api
{
    public class AuthenticationApiService(HttpClient httpClient, IOptions<ApiSettings> apiSettingOptions)
        : BaseApiService(httpClient, apiSettingOptions)
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

        public async Task<HttpResponseMessage> RefreshTokenAsync(RefreshTokenCommand refreshTokenCommand)
        {
            var url = ApiControllers.ApplicationUsers.AppendRoute(ApiRoutes.AUTH_REFRESH_TOKEN);
            var response = await PostAsync(url, refreshTokenCommand);
            return response;
        }

        public async Task<HttpResponseMessage> PingAsync()
        {
            var url = ApiControllers.ApplicationUsers.AppendRoute("auth/ping");
            var response = await GettingAsync(url);
            return response;
        }
    }

    public class BaseApiService
    {
        public BaseApiService(HttpClient httpClient, IOptions<ApiSettings> apiSettingsOptions)
        {
            var apiSettings = apiSettingsOptions.Value;
            // Set base address
            httpClient.BaseAddress = new Uri(apiSettings.BaseUrl);
            httpClient.DefaultRequestHeaders.Add("client-name",apiSettings.ClientNameHeader);
            HttpClient = httpClient;
        }

        public HttpClient HttpClient { get; }

        // Create base method to send post requests
        public Task<HttpResponseMessage> PostAsync(string url, object data)
        {
            return HttpClient.PostAsJsonAsync(url, data);
        }

        // Create base method to send get requests
        public Task<HttpResponseMessage> GettingAsync(string url) => HttpClient.GetAsync(url);


    }
}
