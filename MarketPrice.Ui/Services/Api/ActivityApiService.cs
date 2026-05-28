using MarketPrice.Domain.Position.Commands;
using MarketPrice.Ui.Common;
using MarketPrice.Ui.Services.Session;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketPrice.Domain;

namespace MarketPrice.Ui.Services.Api
{
    public class ActivityApiService (HttpClient httpClient, IOptions<ApiSettings> apiSettingsOptions) : BaseApiService (httpClient, apiSettingsOptions)
    {
        public async Task<HttpResponseMessage> GetUserActivityAsync(Guid userId)
        {
            var url = ApiControllers.ApplicationUsers.AppendRoute(ApiRoutes.GET_USER_ACTIVITY, userId.ToString());
            var response = await GettingAsync(url);
            return response;
        }
    }
}
