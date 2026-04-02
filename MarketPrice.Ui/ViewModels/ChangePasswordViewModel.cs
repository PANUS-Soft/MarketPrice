using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace MarketPrice.Ui.ViewModels
{
    // 'partial' is required for the CommunityToolkit to do its magic behind the scenes
    public partial class ChangePasswordViewModel : ObservableObject
    {
        // ==========================================
        // 1. PROPERTIES (The Data)
        // ==========================================

        [ObservableProperty]
        private string currentPassword;

        [ObservableProperty]
        private string newPassword;

        [ObservableProperty]
        private string repeatPassword;

        // ==========================================
        // 2. COMMANDS (The Actions)
        // ==========================================

        // This handles the back arrow at the top left
        [RelayCommand]
        private async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync(".."); // ".." tells MAUI to go back to the previous page
        }

        // This handles the "Forgot Password" text click
        [RelayCommand]
        private async Task ForgotPasswordAsync()
        {
            // Since we are mocking, we just show an alert for now
            await Shell.Current.DisplayAlert("Forgot Password", "This would navigate to the Forgot Password flow.", "OK");
        }

        // This handles the main "Change Password" blue button
        [RelayCommand]
        private async Task ChangePasswordAsync()
        {
            // Simple mock logic to prove the frontend works
            if (string.IsNullOrWhiteSpace(CurrentPassword) ||
                string.IsNullOrWhiteSpace(NewPassword) ||
                string.IsNullOrWhiteSpace(RepeatPassword))
            {
                await Shell.Current.DisplayAlert("Error", "Please fill in all password fields.", "OK");
                return;
            }

            if (NewPassword != RepeatPassword)
            {
                await Shell.Current.DisplayAlert("Error", "Your new passwords do not match.", "OK");
                return;
            }

            // If we get here, it means the validation passed!
            await Shell.Current.DisplayAlert("Success", "Your password has been changed successfully! (Mock)", "OK");

            // Clear the fields after success
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            RepeatPassword = string.Empty;
        }
    }
}