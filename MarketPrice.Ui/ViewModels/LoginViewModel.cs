using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.RegularExpressions;

namespace MarketPrice.Ui.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty] private string email;
        [ObservableProperty] private string emailError;
        [ObservableProperty] private bool isEmailInvalid;
        [ObservableProperty] private bool rememberMe;
        [ObservableProperty] private string password;

        public void LoadingSavedCredentials()
        {
            var savedEmail = Preferences.Default.Get("SavedEmail", string.Empty);

            if (!string.IsNullOrEmpty(savedEmail))
            {
                Email = savedEmail;
                RememberMe = true;
            }
        }
        partial void OnEmailChanged(string value)
        {
            if (RememberMe)
                Preferences.Default.Set("SavedEmail", Email);

            else
            {
                Preferences.Default.Remove("SavedEmail");
            }
            if (string.IsNullOrWhiteSpace(value))
            {
                EmailError = string.Empty;
                IsEmailInvalid = false;
                return;
            }

            var isValid = Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

            IsEmailInvalid = !isValid;
            EmailError = isValid ? string.Empty : "Invalid email format";
        }

        [RelayCommand]
        private async Task Login()
        {

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter credentials", "OK");
                return;
            }

            OnEmailChanged(Email);

            await Shell.Current.DisplayAlert("Success", $"Logged in as: {Email}", "OK");
        }
    }
}
