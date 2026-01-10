using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
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
            var ok = await sessionService.ValidateAndRefreshSessionAsync();
            if (!ok)
                throw new Exception("Session expired");

            // 2️ Attach fresh token
            var token = sessionService.CurrentSession?.AccessToken;
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
