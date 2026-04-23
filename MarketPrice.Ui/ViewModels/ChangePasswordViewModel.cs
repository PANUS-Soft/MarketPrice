using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketPrice.Domain.Profile.Commands;
using MarketPrice.Domain.Profile.DTOs;
using MarketPrice.Ui.Services.Api;
using MarketPrice.Ui.Services.Session;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Alerts;

namespace MarketPrice.Ui.ViewModels
{
    public partial class ChangePasswordViewModel : ObservableObject
    {
        private readonly SessionService _sessionService;
        private readonly ProfileApiService _profileApi;

        [ObservableProperty] private string currentPassword;
        [ObservableProperty] private string newPassword;
        [ObservableProperty] private string confirmNewPassword;

        [ObservableProperty] private bool hasAttemptedSubmit;

        [ObservableProperty] private string? currentPasswordError;
        [ObservableProperty] private string? newPasswordError;
        [ObservableProperty] private string? confirmNewPasswordError;

        public ChangePasswordViewModel(SessionService sessionService, ProfileApiService profileApiService)
        {
            _sessionService = sessionService;
            _profileApi = profileApiService;
        }

        // Property change handlers
        partial void OnCurrentPasswordChanged(string value)
        {
            if (!HasAttemptedSubmit) return;

            ValidateCurrentPassword();
        }

        partial void OnNewPasswordChanged(string value)
        {
            if (!HasAttemptedSubmit) return;

            ValidateNewPassword();
            ValidateConfirmNewPassword();
        }

        partial void OnConfirmNewPasswordChanged(string value)
        {
            if (!HasAttemptedSubmit) return;

            ValidateConfirmNewPassword();
        }

        // Dedicated validation methods for inputs of the change password form
        private void ValidateCurrentPassword()
        {
            if (string.IsNullOrWhiteSpace(CurrentPassword))
            {
                CurrentPasswordError = "Please enter your current password.";
                return;
            }

            if (CurrentPassword.Length < 8)
            {
                CurrentPasswordError = "Current password must be at least 8 characters";
                return;
            }

            CurrentPasswordError = null;
        }

        private void ValidateNewPassword()
        {
            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                NewPasswordError = "Please enter a new password.";
                return;
            }

            if (NewPassword.Length < 8)
            {
                NewPasswordError = "Password must be at least 8 characters.";
                return;
            }

            if (NewPassword == CurrentPassword)
            {
                NewPasswordError = "New Password must be different from current password.";
                return;
            }

            NewPasswordError = null;
        }

        private void ValidateConfirmNewPassword()
        {
            //if (string.IsNullOrWhiteSpace(NewPassword))
            //{
            //    ConfirmNewPasswordError = null;
            //    return;
            //}

            if (string.IsNullOrWhiteSpace(ConfirmNewPassword))
            {
                ConfirmNewPasswordError = "Please confirm your new password.";
                return;
            }

            if (NewPassword != ConfirmNewPassword)
            {
                ConfirmNewPasswordError = "Passwords do not match.";
                return;
            }

            ConfirmNewPasswordError = null;
        }

        // Global validation check
        private bool ValidateChangePasswordForm()
        {
            ValidateCurrentPassword();
            ValidateNewPassword();
            ValidateConfirmNewPassword();

            return CurrentPasswordError == null && NewPasswordError == null && ConfirmNewPasswordError == null;
        }

        [RelayCommand]
        private async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task ForgotPasswordAsync()
        {
            await Shell.Current.DisplayAlert("Forgot Password", "This would navigate to the Forgot Password flow.", "OK");
        }

        [RelayCommand]
        private async Task ChangePasswordAsync()
        {
            HasAttemptedSubmit = true;

            if (!ValidateChangePasswordForm()) return;

            try
            {
                var userSession = await _sessionService.GetCurrentSessionAsync();

                var command = new ChangePasswordCommand
                {
                    UserId = userSession!.UserId,
                    CurrentPassword = CurrentPassword,
                    NewPassword = NewPassword
                };

                var confirmPasswordChange = await Shell.Current.DisplayAlert("Confirm Password Change", "Are you sure you want to change your password ? You will use this new password to access your account.", "Change Password", "Cancel");

                if (!confirmPasswordChange) return;

                var response = await _profileApi.ChangePasswordAsync(command);

                if (response.IsSuccessStatusCode)
                {
                    var dto = await response.Content.ReadFromJsonAsync<ChangePasswordResponseDto>();

                    if (dto == null) return;

                    if (dto is { Success: true, Message: not null })
                    {
                        await Toast.Make(dto.Message, ToastDuration.Long).Show();

                        CurrentPassword = string.Empty;
                        NewPassword = string.Empty;
                        ConfirmNewPassword = string.Empty;
                        CurrentPasswordError = null;
                        NewPasswordError = null;
                        ConfirmNewPasswordError = null;
                        HasAttemptedSubmit = false;
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", $"An error occurred. {dto.Message}", "OK");
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", $"Server returned status: {response.StatusCode}", "OK");
                }
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error", $"An error occured when trying to change password. {e.Message}", "OK");
            }
        }
    }
}