using MarketPrice.Ui.Models;
using MarketPrice.Ui.Services.Api;
using System.Net.Http.Json;
using System.Text.Json;
using MarketPrice.Domain.Authentication;
using MarketPrice.Domain.Authentication.Commands;
using MarketPrice.Ui.Common;
using MarketPrice.Ui.Extensions;

namespace MarketPrice.Ui.Services.Session
{
    public class SessionService(AuthenticationApiService authenticationApiService)
    {
        private readonly string _sessionKey = "UserSession";
        private const string PendingNavigationKey = "PendingNavigation";

        public bool IsLoggedIn { get; private set; }

        public async Task<bool> EnsureUserAccessAsync()
        {
            bool hasCompletedOnboarding = Preferences.Get("HasCompletedOnboarding", false);

            if (!hasCompletedOnboarding)
            {
                await Shell.Current.GoToAsync("//Onboarding");
                return false;
            }

            return await ValidateAndRefreshSessionAsync();

            //var isSessionValid = await ValidateAndRefreshSessionAsync();

            //if (!isSessionValid)
            //{
            //    await Shell.Current.GoToAsync("//Welcome");
            //    return false;
            //}

            //return true;
        }

        public async Task<UserSession?> GetCurrentSessionAsync()
        {
            try
            {
                var sessionString = await SecureStorage.GetAsync(_sessionKey);
                if (string.IsNullOrEmpty(sessionString))
                    return null;

                return sessionString.FromJson<UserSession?>();
            }
            catch (Exception e)
            {
                SecureStorage.Remove(_sessionKey);
                return null;
            }
        }

        public async Task<bool> StartSessionAsync(UserSession session)
        {
            try
            {
                // Persist session in storage
                await SecureStorage.SetAsync(_sessionKey, session.ToJson());

                IsLoggedIn = true;

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
                    IsLoggedIn = false;
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

            if (session.ExpireAt > DateTime.UtcNow)
            {
                return await StartSessionAsync(session);
            }

            // Access token expired, try to refresh
            return await TryRefreshTokenAsync();
        }

        public async Task InitializeAsync()
        {
            var currentSession = await GetCurrentSessionAsync();
            if (currentSession is null) return;

            await StartSessionAsync(currentSession);

            IsLoggedIn = await ValidateAndRefreshSessionAsync();
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

        public void SavePendingNavigation(PendingNavigation navigation)
        {
            var json = JsonSerializer.Serialize(navigation);

            Preferences.Set(PendingNavigationKey, json);
        }

        public PendingNavigation? GetPendingNavigation()
        {
            var json = Preferences.Get(PendingNavigationKey, string.Empty);

            if (string.IsNullOrWhiteSpace(json)) return null;

            return JsonSerializer.Deserialize<PendingNavigation>(json);
        }

        public void ClearPendingNavigation()
        {
            Preferences.Remove(PendingNavigationKey);
        }

        public async Task<bool> RestorePendingNavigationAsync(string? expectedDestination = null)
        {
            var pendingNavigation = GetPendingNavigation();

            if (pendingNavigation == null) return false;

            var destination = GetPendingDestination(pendingNavigation.Route);
            if (destination == null) return false;

            if (expectedDestination != null &&
                !AuthenticationNavigation.IsSameDestination(destination, expectedDestination))
                return false;

            switch (destination)
            {
                case "//Activity":
                case "//Profile":
                    await Shell.Current.GoToAsync(destination);
                    break;

                case "//Market/MarketInsight" when pendingNavigation.MarketItemFilter != null:
                    await Shell.Current.GoToAsync(destination, new Dictionary<string, object>
                    {
                        { "SelectedMarketItemFilter", pendingNavigation.MarketItemFilter }
                    });
                    break;

                case "//Market/PositionListing":
                case "//Home/PositionListing":
                    if (pendingNavigation.PositionListingCommand == null) return false;

                    var parameters = new Dictionary<string, object>
                    {
                        { "Args", pendingNavigation.PositionListingCommand }
                    };

                    AddParameter(parameters, "PassedCommodityName", pendingNavigation.PassedCommodityName);
                    AddParameter(parameters, "PassedLotSize", pendingNavigation.PassedLotSize);
                    AddParameter(parameters, "PassedBid", pendingNavigation.PassedBid);
                    AddParameter(parameters, "PassedOffer", pendingNavigation.PassedOffer);

                    if (!string.IsNullOrWhiteSpace(pendingNavigation.PassedImageLocation))
                    {
                        var imageSource = Uri.TryCreate(
                            pendingNavigation.PassedImageLocation,
                            UriKind.Absolute,
                            out var imageUri)
                            ? ImageSource.FromUri(imageUri)
                            : ImageSource.FromFile(pendingNavigation.PassedImageLocation);

                        parameters["PassedImage"] = imageSource;
                    }

                    await Shell.Current.GoToAsync(destination, parameters);
                    break;

                default:
                    return false;
            }

            ClearPendingNavigation();
            return true;
        }

        private static string? GetPendingDestination(string route) => route switch
        {
            "MarketInsight" => "//Market/MarketInsight",
            "PositionListing" => "//Market/PositionListing",
            _ => AuthenticationNavigation.ValidateDestination(route)
        };

        private static void AddParameter(
            IDictionary<string, object> parameters,
            string key,
            string? value)
        {
            if (!string.IsNullOrWhiteSpace(value)) parameters[key] = value;
        }
    }
}
