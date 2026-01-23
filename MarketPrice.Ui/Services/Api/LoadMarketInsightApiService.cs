using MarketPrice.Domain;
using MarketPrice.Ui.Common;
using Microsoft.Extensions.Options;

namespace MarketPrice.Ui.Services.Api
{
    public class LoadMarketInsightApiService(HttpClient httpClient, IOptions<ApiSettings> apiSettingOptions) : BaseApiService(httpClient, apiSettingOptions) 
    {
        public async Task<HttpResponseMessage> GetMarketInsightsAsync()
        {
            var url = ApiControllers.Markets.AppendRoute(ApiRoutes.MARKET_INSIGHTS);
            var response = await GettingAsync(url);
            return response;
        }
    }
}
