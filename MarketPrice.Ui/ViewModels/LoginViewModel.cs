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
    public partial class LoginViewModel : ObservableObject
    {
        private readonly AuthenticationApiService _authenticationApi;
        private readonly SessionService _sessionService;
        private readonly SessionStorage _sessionStorage;

        public LoginInformation LoginInfo { get; } = new();

        public LoginViewModel(AuthenticationApiService authenticationApi, SessionService sessionService, SessionStorage sessionStorage)
        {
            _authenticationApi = authenticationApi;
            _sessionService = sessionService;
            _sessionStorage = sessionStorage;
        }

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
                var loginCommand = new LoginCommand
                {
                    LoginDate = DateTime.Now,
                    EmailAddress = LoginInfo.EmailAddress,
                    Password = LoginInfo.Password,
                    RememberMe = LoginInfo.RememberMe
                };

                var response = await _authenticationApi.LoginUserAsync(loginCommand);
                var responseMessage = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var dto = JsonSerializer.Deserialize<LoginResponseDto>(responseMessage, new JsonSerializerOptions { PropertyNameCaseInsensitive = true});

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

                        _sessionService.StartSession(session);
                        await _sessionStorage.SaveAsync(session);
                        await Toast.Make($"Welcome back, {dto.FirstName}!!!", ToastDuration.Long).Show();
                        //await Snackbar.Make(
                        //    $"Welcome back, {dto.FirstName}! Good to see you again.", 
                        //    action: null, 
                        //    actionButtonText: "", 
                        //    TimeSpan.FromSeconds(3), 
                        //    new SnackbarOptions
                        //    {
                        //        BackgroundColor = Colors.DarkSlateBlue,
                        //        TextColor = Colors.White,
                        //        CornerRadius = new CornerRadius(10),
                        //        Font = Microsoft.Maui.Font.OfSize("RobotoSerifRegular", 12),
                        //        CharacterSpacing = 0.5
                        //    }).Show();
                        await Shell.Current.GoToAsync("//Home");
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", responseMessage, "OK");
                }
            }
            catch(Exception e)
            {
                await Shell.Current.DisplayAlert("Error", $"{e.Message}", "OK");
            }
        }
    }
}
