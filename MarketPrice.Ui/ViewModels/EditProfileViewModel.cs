using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System.Collections.ObjectModel;

namespace MarketPrice.Ui.ViewModels
{
    [QueryProperty(nameof(Initials), "Initials")]
    [QueryProperty(nameof(FullName), "FullName")]
    [QueryProperty(nameof(Email), "Email")]
    [QueryProperty(nameof(PhoneNumber), "PhoneNumber")]
    [QueryProperty(nameof(AccountType), "AccountType")]
    public partial class EditProfileViewModel : ObservableObject
    {
        // ... existing properties ...
        [ObservableProperty] private string firstName;
        [ObservableProperty] private string familyName;
        [ObservableProperty] private string otherName;
        [ObservableProperty] private string email;
        [ObservableProperty] private string phoneNumber;
        [ObservableProperty] private string accountType;

        // FIX CS8618: Initialize with default values
        [ObservableProperty] private string initials = string.Empty;
        [ObservableProperty] private string fullName = string.Empty;

        // FIX: Add list for ComboBox
        public ObservableCollection<string> AccountTypes { get; } = new() { "Personal", "Business" };

        public EditProfileViewModel()
        {
            // Mock data
            FirstName = "CHU";
            FamilyName = "BEH";
            OtherName = "NELSON";
            Email = "chubeh@gmail.com";
            PhoneNumber = "+237 671000000";
            AccountType = "Personal";
        }

        // ... Save and Discard commands remain the same ...
        [RelayCommand]
        private async Task Save()
        {
            await Task.Delay(500);
            await Toast.Make("Profile updated successfully", ToastDuration.Long).Show();
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task Discard()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}