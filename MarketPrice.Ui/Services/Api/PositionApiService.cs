using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kotlin;
using MarketPrice.Domain;
using MarketPrice.Domain.Position.Commands;
using MarketPrice.Ui.Common;
using Microsoft.Extensions.Options;

namespace MarketPrice.Ui.Services.Api
{
    public class PositionApiService (HttpClient httpClient, IOptions<ApiSettings> apiSettingsOptions) : BaseApiService (httpClient, apiSettingsOptions)
    {
        public async Task<HttpResponseMessage> CreateBidAsync(CreatePositionCommand createPositionCommand)
        {
            throw new NotImplementedException();
        }

        public async Task<HttpResponseMessage> CreateOfferAsync(CreatePositionCommand createPositionCommand)
        {
            throw new NotImplementedException();
        }
    }
}
