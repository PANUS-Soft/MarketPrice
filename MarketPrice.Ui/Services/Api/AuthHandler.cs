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

            // if the request is going to the "Authentication" controller, skip adding the token
            var authUri = ApiControllers.ApplicationUsers.AppendRoute(ApiRoutes.AUTH);
            if (request.RequestUri != null &&
                request.RequestUri.AbsolutePath.Contains(authUri, StringComparison.OrdinalIgnoreCase))
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
