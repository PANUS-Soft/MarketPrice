using MarketPrice.Domain;
using MarketPrice.Domain.Profile.Commands;
using Microsoft.Extensions.Options;
using MarketPrice.Ui.Common;
using MarketPrice.Ui.Services.Session;

namespace MarketPrice.Ui.Services.Api
{
    public class ProfileApiService(HttpClient httpClient, SessionService sessionService, IOptions<ApiSettings> apiSettingOptions) : BaseApiService(httpClient, apiSettingOptions)
    {
        public async Task<HttpResponseMessage> GetUserProfileAsync(Guid userId)
        {
            var url = ApiControllers.ApplicationUsers.AppendRoute(ApiRoutes.GET_USER_PROFILE, userId.ToString());
            var response = await GettingAsync(url);
            return response;
        }

        public async Task<HttpResponseMessage> UpdateUserProfileAsync(UpdateUserProfileCommand updateUserProfileCommand)
        {
            var url = ApiControllers.ApplicationUsers.AppendRoute(ApiRoutes.UPDATE_USER_PROFILE);
            var response = await PatchAsync(url, updateUserProfileCommand);
            return response;
        }

        public async Task<HttpResponseMessage> ChangePasswordAsync(ChangePasswordCommand changePasswordCommand)
        {
            var url = ApiControllers.ApplicationUsers.AppendRoute(ApiRoutes.CHANGE_PASSWORD);
            var response = await PostAsync(url, changePasswordCommand);
            return response;
        }
    }
}
