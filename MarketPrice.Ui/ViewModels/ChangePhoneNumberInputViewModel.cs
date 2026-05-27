using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MarketPrice.Ui.ViewModels;

public partial class ChangePhoneNumberInputViewModel : ObservableObject
{
    [ObservableProperty]
    private string newPhoneNumber;

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task ContinueAsync()
    {
        if (string.IsNullOrWhiteSpace(NewPhoneNumber))
        {
            await Shell.Current.DisplayAlert(
                "Error",
                "Please enter a phone number.",
                "OK");

            return;
        }

        // Navigate to OTP page later
        await Shell.Current.DisplayAlert(
            "Success",
            $"Verification code will be sent to +237 {NewPhoneNumber}",
            "OK");
    }
}