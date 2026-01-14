using MarketPrice.Ui.Models;
using MarketPrice.Ui.Services.Api;
using System.Net.Http.Json;
using MarketPrice.Domain.Authentication;
using MarketPrice.Domain.Authentication.Commands;
using MarketPrice.Ui.Extensions;

namespace MarketPrice.Ui.Services.Session
{
    public class SessionService(AuthenticationApiService authenticationApiService)
    {
        private readonly string _sessionKey = "UserSession";

        public async Task<UserSession?> GetCurrentSessionAsync()
        {
            var sessionString = await SecureStorage.GetAsync(_sessionKey);
            if (string.IsNullOrEmpty(sessionString))
                return null;

            return sessionString.FromJson<UserSession?>();
        }

        public async Task<bool> StartSessionAsync(UserSession session)
        {
            try
            {
                // Persist session in storage
                await SecureStorage.SetAsync(_sessionKey, session.ToJson());

                return true;
            }
            catch (Exception e)
            {
                // TODO: Use a logger to log to a logging service
                return false;
            }
        }

        public async Task<bool> EndSessionAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    SecureStorage.Remove(_sessionKey);
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            });

        }

        /// <summary>
        /// Attempts to refresh the access token using the refresh token.
        /// </summary>
        /// <returns>True if refresh was successful, false other otherwise ...</returns>
        public async Task<bool> TryRefreshTokenAsync()
        {
            try
            {
                var session = await GetCurrentSessionAsync();

                if (session == null) return false;

                var refreshToken = session.RefreshToken;
                if (string.IsNullOrEmpty(refreshToken)) return false;

                var command = new RefreshTokenCommand
                {
                    UserId = session.UserId,
                    RefreshToken = session.RefreshToken
                };

                var response = await authenticationApiService.RefreshTokenAsync(command);

                if (!response.IsSuccessStatusCode)
                {
                    await EndSessionAsync();
                    return false;
                }
                
                var dto = await response.Content.ReadFromJsonAsync<AuthenticationResponseDto>();
                if (dto == null || !dto.Success)
                {
                    await EndSessionAsync();
                    return false;
                }

                return await StartSessionAsync(dto);
            }
            catch (Exception e)
            {
                await EndSessionAsync();
                return false;
            }
        }

        /// <summary>
        /// Validates current session and refreshes tokens if needed.
        /// </summary>
        /// <returns>True if user has valid session (or token was refreshed), false otherwise.</returns>
        public async Task<bool> ValidateAndRefreshSessionAsync()
        {
            var session = await GetCurrentSessionAsync();

            if (session == null)
                return false;

            if (session.ExpireAt > DateTime.Now)
            {
                return await StartSessionAsync(session);
            }

            // Access token expired, try to refresh
            return await TryRefreshTokenAsync();
        }

        public async Task InitializeAsync()
        {
            var currentSession = await GetCurrentSessionAsync();
            if(currentSession is null) return;

            await StartSessionAsync(currentSession);
        }

        public async Task<bool> StartSessionAsync(AuthenticationResponseDto authResponseDto)
        {
            // Create session
           var session =  new UserSession
            {
                UserId = authResponseDto.UserId,
                AccessToken = authResponseDto.AccessToken,
                EmailAddress = authResponseDto.EmailAddress,
                ExpireAt = authResponseDto.ExpiryDate,
                FirstName = authResponseDto.FirstName,
                RefreshToken = authResponseDto.RefreshToken
            };

            return await StartSessionAsync(session);
        }
    }
}
