using MarketPrice.Domain;
using Microsoft.Extensions.Options;
using MarketPrice.Ui.Common;
using MarketPrice.Ui.Services.Session;

namespace MarketPrice.Ui.Services.Api
{
    public class ProfileApiService(HttpClient httpClient, SessionService sessionService, IOptions<ApiSettings> apiSettingOptions) : BaseApiService(httpClient, apiSettingOptions)
    {
        public async Task<HttpResponseMessage> GetUserProfileAsync(Guid userId)
        {
            var currentSession = await sessionService.GetCurrentSessionAsync();
            var url = ApiControllers.ApplicationUsers.AppendRoute(ApiRoutes.GET_USER_PROFILE, userId.ToString());
            var response = await GettingAsync(url);
            return response;
        }
            
    }
}
