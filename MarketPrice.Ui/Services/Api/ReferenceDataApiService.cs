    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketPrice.Domain;
using MarketPrice.Ui.Common;
using Microsoft.Extensions.Options;

namespace MarketPrice.Ui.Services.Api
{
    public class ReferenceDataApiService (HttpClient httpClient, IOptions<ApiSettings> apiSettingOptions) : BaseApiService (httpClient, apiSettingOptions)
    {
        public async Task<HttpResponseMessage> GetRegionsAsync()
        {
            var url = ApiControllers.ReferenceData.AppendRoute(ApiRoutes.REF_REGION);
            var response = await GettingAsync(url);
            return response;
        }

        public async Task<HttpResponseMessage> GetCommodityTypesAsync()
        {
            var url = ApiControllers.ReferenceData.AppendRoute(ApiRoutes.REF_COMMODITY_TYPE);
            var response = await GettingAsync(url);
            return response;
        }

        public async Task<HttpResponseMessage> GetCommoditiesByCommodityTypeIdAsync(Guid id)
        {
            var url = ApiControllers.ReferenceData.AppendRoute(ApiRoutes.REF_COMMODITY + $"/{id}");
            var response = await GettingAsync(url);
            return response;
        }
    }
}
