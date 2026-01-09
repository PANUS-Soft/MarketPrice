using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MarketPrice.Ui.Services.Session;

namespace MarketPrice.Ui.Services.Api
{
    public class AuthHandler(SessionService sessionService) : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // 1️ Ensure session is valid (refresh if close to expiry)
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
