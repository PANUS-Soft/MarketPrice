using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Ui.ViewModels
{
    public partial class ChangePasswordViewModel : ObservableObject
    {
        [ObservableProperty]
        private string currentPassword;

        [ObservableProperty]
        private string newPassword;

        [ObservableProperty]
        private string repeatPassword;


        //Handles the back arrow
        [RelayCommand]
        private async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        //Handles the forget password
        [RelayCommand]
        private async Task ForgotPasswordAsync()
        {
            await Shell.Current.DisplayAlert("Forgot Password", "This would navigate to the Forgot Password flow.", "OK");
        }

        [RelayCommand]
        private async Task ChangePasswordAsync()
        {
            if (string.IsNullOrWhiteSpace(currentPassword) ||
                string.IsNullOrWhiteSpace(newPassword) ||
                string.IsNullOrWhiteSpace(repeatPassword))
            {
                await Shell.Current.DisplayAlert("Error", "Please fill in alll the password fields.", "OK");
                return;
            }

            if (newPassword != repeatPassword)
            {
                await Shell.Current.DisplayAlert("Error", "Your new passwords do not match.", "OK");
                return;
            }

            await Shell.Current.DisplayAlert("Success", "Your password has been successfully!",  "OK");

            //Clear the fields after success
            currentPassword = string.Empty;
            newPassword = string.Empty;
            repeatPassword  = string.Empty;
        }
    }
}
