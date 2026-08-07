using System.Net.Http.Json;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketPrice.Domain.Authentication.Commands;
using MarketPrice.Domain.Authentication.DTOs;
using MarketPrice.Ui.Common;
using MarketPrice.Ui.Models;
using MarketPrice.Ui.Services.Api;
using MarketPrice.Ui.Services.Session;

namespace MarketPrice.Ui.ViewModels
{
    public partial class LoginViewModel(
        AuthenticationApiService authenticationApi,
        SessionService sessionService)
        : ObservableObject, IQueryAttributable
    {
        private string? _redirectTo;

        public LoginInformation LoginInfo { get; } = new();

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            _redirectTo = AuthenticationNavigation.ReadDestination(query);
        }

        [RelayCommand]
        private async Task NavigateToRegisterAsync()
        {
            if (_redirectTo != null && sessionService.GetPendingNavigation() == null)
            {
                sessionService.SavePendingNavigation(new PendingNavigation
                {
                    Route = _redirectTo
                });
            }

            await Shell.Current.GoToAsync("//Register");
        }


        [RelayCommand]
        private async Task LoginAsync()
        {

            if (string.IsNullOrWhiteSpace(LoginInfo.EmailAddress) || string.IsNullOrWhiteSpace(LoginInfo.Password))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter credentials", "OK");
                return;
            }

            try
            {
                var command = new LoginCommand
                {
                    LoginDate = DateTime.UtcNow,
                    EmailAddress = LoginInfo.EmailAddress,
                    Password = LoginInfo.Password,
                    RememberMe = LoginInfo.RememberMe
                };

                var response = await authenticationApi.LoginUserAsync(command);
                if (response.IsSuccessStatusCode)
                {

                    var dto  = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
                    if (dto != null)
                    {
                        var session = new UserSession
                        {
                            AccessToken = dto.AccessToken,
                            RefreshToken = dto.RefreshToken,
                            ExpireAt = dto.ExpiryDate,
                            FirstName = dto.FirstName,
                            EmailAddress = dto.EmailAddress
                        };
                        
                        await sessionService.StartSessionAsync(dto);
                        await Toast.Make($"Welcome back, {dto.FirstName} 👋", ToastDuration.Long).Show();

                        var redirectRoute = GetRedirectRoute();
                        _redirectTo = null;

                        if (redirectRoute != null)
                        {
                            var restored = await sessionService
                                .RestorePendingNavigationAsync(redirectRoute);

                            if (!restored)
                            {
                                sessionService.ClearPendingNavigation();

                                if (AuthenticationNavigation.RequiresPendingState(redirectRoute))
                                    await Shell.Current.GoToAsync("//Home");
                                else
                                    await Shell.Current.GoToAsync(redirectRoute);
                            }
                        }
                        else if (sessionService.GetPendingNavigation() != null)
                        {
                            await sessionService.RestorePendingNavigationAsync();
                        }
                        else
                        {
                            await Shell.Current.GoToAsync("//Home");
                        }
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Something went wrong, try again or contact support.", "OK");
                    return;
                }
            }
            catch(Exception e)
            {
                await Shell.Current.DisplayAlert("Error", $"{e.Message}", "OK");
            }
        }

        private string? GetRedirectRoute()
        {
            return AuthenticationNavigation.ValidateDestination(_redirectTo);
        }
    }
}
