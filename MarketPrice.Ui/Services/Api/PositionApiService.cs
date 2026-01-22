using MarketPrice.Domain;
using MarketPrice.Domain.Position.Commands;
using MarketPrice.Ui.Common;
using Microsoft.Extensions.Options;

namespace MarketPrice.Ui.Services.Api
{
    public class PositionApiService (HttpClient httpClient, IOptions<ApiSettings> apiSettingsOptions) : BaseApiService (httpClient, apiSettingsOptions)
    {
        public async Task<HttpResponseMessage> CreateBidAsync(PositionCommand createPositionCommand)
        {
            var url = ApiControllers.Positions.AppendRoute(ApiRoutes.BID_CREATE);
            var response = await PostAsync(url, createPositionCommand);
            return response;
        }

        public async Task<HttpResponseMessage> CreateOfferAsync(PositionCommand createPositionCommand)
        {
            var url = ApiControllers.Positions.AppendRoute(ApiRoutes.OFFER_CREATE);
            var response = await PostAsync(url, createPositionCommand);
            return response;
        }
    }
}
