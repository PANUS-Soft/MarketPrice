using MarketPrice.Domain.Authentication.DTOs;
using MarketPrice.Ui.Models;
using MarketPrice.Ui.Services.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MarketPrice.Domain.Authentication.Commands;

namespace MarketPrice.Ui.Services.Session
{
    public class SessionService(SessionStorage sessionStorage, AuthenticationApiService authenticationApiService)
    {
        public UserSession? CurrentSession { get; private set; }

        public bool IsAuthenticated => CurrentSession != null && CurrentSession.ExpireAt > DateTime.Now;

        public bool IsExpired() => CurrentSession == null || CurrentSession.ExpireAt <= DateTime.Now;

        public void StartSession(UserSession session) => CurrentSession = session;

        public void EndSession()
        {
            CurrentSession = null;
            sessionStorage.Clear();
        }

        /// <summary>
        /// Attempts to refresh the access token using the refresh token.
        /// </summary>
        /// <returns>True if refresh was successful, false other otherwise ...</returns>
        public async Task<bool> TryRefreshTokenAsync()
        {
            try
            {
                var session = await sessionStorage.LoadAsync();

                if (session == null) return false;

                if (string.IsNullOrEmpty(session.RefreshToken)) return false;

                var command = new RefreshTokenCommand
                {
                    AccessToken = session.AccessToken,
                    RefreshToken = session.RefreshToken
                };

                var response = await authenticationApiService.RefreshTokenAsync(command);

                if (!response.IsSuccessStatusCode)
                {
                    EndSession();
                    return false;
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var dto = JsonSerializer.Deserialize<RefreshTokenResponseDto>(responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (dto == null || !dto.Success)
                {
                    EndSession();
                    return false;
                }

                var updateSession = new UserSession
                {
                    AccessToken = dto.AccessToken,
                    RefreshToken = dto.RefreshToken,
                    ExpireAt = dto.ExpiryDate,
                    FirstName = session.FirstName,
                    EmailAddress = session.EmailAddress
                };

                StartSession(updateSession);
                await sessionStorage.SaveAsync(updateSession);

                return true;
            }
            catch (Exception e)
            {
                EndSession();
                return false;
            }
        }

        /// <summary>
        /// Validates current session and refreshes tokens if needed.
        /// </summary>
        /// <returns>True if user has valid session (or token was refreshed), false otherwise.</returns>
        public async Task<bool> ValidateAndRefreshSessionAsync()
        {
            var session = await sessionStorage.LoadAsync();

            if (session == null)
                return false;

            if (session.ExpireAt > DateTime.Now)
            {
                StartSession(session);
                return true;
            }

            // Access token expired, try to refresh
            return await TryRefreshTokenAsync();
        }

        public async Task InitializeAsync()
        {
            CurrentSession = await sessionStorage.LoadAsync();
        }

        //public async Task<bool> RefreshSessionAsync()
        //{
        //    if (CurrentSession == null) return false;

        //    var response = await authenticationApiService.RefreshTokenAsync(new RefreshTokenCommand { AccessToken = CurrentSession.AccessToken, RefreshToken = CurrentSession.RefreshToken });
        //    if (!response.IsSuccessStatusCode) return false;
            
        //    var responseMessage = await response.Content.ReadAsStringAsync();
        //    var dto = JsonSerializer.Deserialize<RefreshTokenResponseDto>(responseMessage, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
        //    if (dto == null || string.IsNullOrEmpty(dto.AccessToken)) return false;

        //    UpdateAccessToken(dto.AccessToken, dto.RefreshToken, dto.ExpiryDate);
        //    await sessionStorage.SaveAsync(CurrentSession);
            
        //    return true;
        //}

        //public async Task<bool> EnsureValidSessionAsync()
        //{
        //    if (CurrentSession == null) return false;

        //    var timeLeft = CurrentSession.ExpireAt - DateTime.Now;

        //    if (timeLeft.TotalSeconds > 90) return true;

        //    return await RefreshSessionAsync();
        //}
    }
}
