using MarketPrice.Domain;
using MarketPrice.Domain.Position.Commands;
using MarketPrice.Ui.Common;
using Microsoft.Extensions.Options;

namespace MarketPrice.Ui.Services.Api
{
    public class PositionApiService (HttpClient httpClient, IOptions<ApiSettings> apiSettingOptions) : BaseApiService (httpClient, apiSettingOptions)
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

        public async Task<HttpResponseMessage> GetPositionListingAsync(PositionListingCommand positionListingCommand)
        {
            var url = ApiControllers.Positions.AppendRoute(ApiRoutes.POSITION_BY_PRICE);
            var response = await PostAsync(url, positionListingCommand);
            return response;
        }

        public async Task<HttpResponseMessage> GetPositionDetailAsync(Guid positionId)
        {
            var url = ApiControllers.Positions.AppendRoute(ApiRoutes.POSITION_DETAIL, positionId.ToString());
            var response = await GettingAsync(url);
            return response;
        }

    }
}
