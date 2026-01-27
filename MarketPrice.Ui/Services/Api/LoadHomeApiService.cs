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
    public class LoadHomeApiService(HttpClient httpClient, IOptions<ApiSettings> apiSettingsOptions) : BaseApiService (httpClient, apiSettingsOptions)
    {
        public async Task<HttpResponseMessage> LoadHomeAsync()
        {
            var url = ApiControllers.Home.AppendRoute(ApiRoutes.HOME_DATA);
            var response = await GettingAsync(url);
            return response;
        }

        //public async Task<ImageSource> LoadImageAsync(Guid id)
        //{
        //    var url = ApiControllers.Images.AppendRoute(ApiRoutes.COMMODITY_TYPE_IMAGE);
        //    var stream = await httpClient.GetStreamAsync(imageUrl);
        //    return ImageSource.FromStream(() => stream);
        //}
    }
}
