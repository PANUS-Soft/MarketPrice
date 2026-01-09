using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketPrice.Domain.Authentication.Commands;
using MarketPrice.Domain.Authentication.DTOs;
using MarketPrice.Ui.Models;
using MarketPrice.Ui.Services.Api;
using MarketPrice.Ui.Services.Session;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace MarketPrice.Ui.ViewModels
{
    public partial class LoginViewModel(
        AuthenticationApiService authenticationApi,
        SessionService sessionService,
        SessionStorage sessionStorage)
        : ObservableObject
    {
        public LoginInformation LoginInfo { get; } = new();

        [RelayCommand]
        private async Task NavigateToRegisterAsync()
        {
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
                    LoginDate = DateTime.Now,
                    EmailAddress = LoginInfo.EmailAddress,
                    Password = LoginInfo.Password,
                    RememberMe = LoginInfo.RememberMe
                };

                var response = await authenticationApi.LoginUserAsync(command);
                var responseMessage = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var dto = JsonSerializer.Deserialize<LoginResponseDto>(responseMessage, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

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

                        sessionService.StartSession(session);
                        await sessionStorage.SaveAsync(session);
                        await Toast.Make($"Welcome back, {dto.FirstName} 👋", ToastDuration.Long).Show();
                        await Shell.Current.GoToAsync("//Home");
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", responseMessage, "OK");
                    return;
                }
            }
            catch(Exception e)
            {
                await Shell.Current.DisplayAlert("Error", $"{e.Message}", "OK");
            }
        }
    }
}
