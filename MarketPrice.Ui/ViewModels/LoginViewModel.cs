using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketPrice.Domain.Authentication.Commands;
using MarketPrice.Ui.Models;
using MarketPrice.Ui.Services.Api;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace MarketPrice.Ui.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly AuthenticationApiService _authenticationApi;

        public LoginInformation LoginInfo { get; } = new();

        public LoginViewModel(AuthenticationApiService authenticationApi)
        {
            _authenticationApi = authenticationApi;
        }


        //[ObservableProperty] private string email;
        //[ObservableProperty] private string emailError;
        //[ObservableProperty] private bool isEmailInvalid;
        //[ObservableProperty] private bool rememberMe;
        //[ObservableProperty] private string password;

        [RelayCommand]
        private async Task NavigateToRegisterAsync()
        {
            await Shell.Current.GoToAsync("//Register");
        }


        //public void LoadingSavedCredentials()
        //{
        //    var savedEmail = Preferences.Default.Get("SavedEmail", string.Empty);

        //    if (!string.IsNullOrEmpty(savedEmail))
        //    {
        //        Email = savedEmail;
        //        RememberMe = true;
        //    }
        //}
        //partial void OnEmailChanged(string value)
        //{
        //    if (RememberMe)
        //        Preferences.Default.Set("SavedEmail", Email);

        //    else
        //    {
        //        Preferences.Default.Remove("SavedEmail");
        //    }
        //    if (string.IsNullOrWhiteSpace(value))
        //    {
        //        EmailError = string.Empty;
        //        IsEmailInvalid = false;
        //        return;
        //    }

        //    var isValid = Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        //    IsEmailInvalid = !isValid;
        //    EmailError = isValid ? string.Empty : "Invalid email format";
        //}

        [RelayCommand]
        private async Task LoginAsync()
        {

            if (string.IsNullOrWhiteSpace(LoginInfo.EmailAddress) || string.IsNullOrWhiteSpace(LoginInfo.Password))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter credentials", "OK");
                return;
            }

            //OnEmailChanged(Email);

            try
            {
                var loginCommand = new LoginCommand
                {
                    LoginDate = DateTime.Now,
                    EmailAddress = LoginInfo.EmailAddress,
                    Password = LoginInfo.Password,
                    RememberMe = LoginInfo.RememberMe
                };

                if (_authenticationApi == null)
                {
                    await Shell.Current.DisplayAlert("Error", "Service not initialized", "OK");
                    return;
                }

                var response = await _authenticationApi.LoginUserAsync(loginCommand);
                var responseMessage = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    await Shell.Current.DisplayAlert("Success", responseMessage, "OK");
                    await Shell.Current.GoToAsync("//MainPage");
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
