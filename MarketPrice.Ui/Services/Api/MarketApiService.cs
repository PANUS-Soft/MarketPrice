using MarketPrice.Domain;
using MarketPrice.Ui.Common;
using Microsoft.Extensions.Options;

namespace MarketPrice.Ui.Services.Api
{
    public class MarketApiService(HttpClient httpClient, IOptions<ApiSettings> apiSettingOptions) : BaseApiService(httpClient, apiSettingOptions)
    {
        public async Task<HttpResponseMessage> LoadMarketAsync()
        {
            var url = ApiControllers.Markets.AppendRoute(ApiRoutes.LOAD_MARKET_DATA);
            var response = await GettingAsync(url);
            return response;
        }

        public async Task<HttpResponseMessage> GetCommodityMarketInsightAsync(Guid commodityId)
        {
            var url = ApiControllers.Markets.AppendRoute(ApiRoutes.GET_MARKET_INSIGHT, commodityId.ToString());
            var response = await GettingAsync(url);
            return response;
        }

        public async Task<HttpResponseMessage> GetMarketOverviewAsync(int positionTypeId)
        {
            var url = ApiControllers.Markets.AppendRoute(ApiRoutes.GET_MARKET_OVERVIEW, positionTypeId.ToString());
            var response = await GettingAsync(url);
            return response;
        }

        public async Task<HttpResponseMessage> GetChartDataAsync(Guid commodityId, string range)
        {
            var route = ApiControllers.Markets.AppendRoute(ApiRoutes.GET_CHART_DATA);
            var url = route.Replace("{commodityId}", commodityId.ToString());
            var finalUrl = $"{url}?range={range}";

            return await GettingAsync(finalUrl);
        }
    }
}