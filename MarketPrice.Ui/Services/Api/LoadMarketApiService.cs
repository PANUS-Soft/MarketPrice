using MarketPrice.Domain;
using MarketPrice.Ui.Common;
using Microsoft.Extensions.Options;

namespace MarketPrice.Ui.Services.Api
{
    public class LoadMarketApiService(HttpClient httpClient, IOptions<ApiSettings> apiSettingOptions) : BaseApiService(httpClient, apiSettingOptions) 
    {
        public async Task<HttpResponseMessage> LoadMarketAsync()
        {
            var url = ApiControllers.Markets.AppendRoute(ApiRoutes.LOAD_MARKET_DATA);
            var response = await GettingAsync(url);
            return response;
        }
    }
}
