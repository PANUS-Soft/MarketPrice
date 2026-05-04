using System.Net.Http.Headers;
using MarketPrice.Domain;
using MarketPrice.Ui.Services.Session;

namespace MarketPrice.Ui.Services.Api
{
    public class AuthHandler(IServiceProvider serviceProvider) : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var publicRoutes = new[]
            {
                ApiControllers.ApplicationUsers.AppendRoute(ApiRoutes.AUTH),
                ApiControllers.Home.AppendRoute(ApiRoutes.LOAD_HOME_DATA),
                ApiControllers.Markets.AppendRoute(ApiRoutes.LOAD_MARKET_DATA),
                ApiControllers.ReferenceData.AppendRoute(ApiRoutes.REF_COMMODITY_TYPE),
                ApiControllers.CommodityImages,
                ApiControllers.CommodityTypeImages
            };

            // if the request is targeting ao public route (endpoint), skip adding the token
            if (request.RequestUri != null && 
                publicRoutes.Any(route => request.RequestUri.AbsolutePath.Contains(route, StringComparison.OrdinalIgnoreCase)))
            {
                return await base.SendAsync(request, cancellationToken);
            }

            // 1️ Ensure session is valid (refresh if close to expiry)
            var sessionService = serviceProvider.GetRequiredService<SessionService>();
            var isValidToken = await sessionService.ValidateAndRefreshSessionAsync();

            if (!isValidToken)
                throw new Exception("Session expired");

            // 2️ Attach fresh token
            var currentSession = await sessionService.GetCurrentSessionAsync();
            if (!string.IsNullOrEmpty(currentSession!.AccessToken))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", currentSession.AccessToken);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
