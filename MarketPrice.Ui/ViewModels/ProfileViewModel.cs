using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace MarketPrice.Ui.ViewModels
{
    public partial class ProfileViewModel : ObservableObject
    {
        // User Details
        [ObservableProperty] private string initials;
        [ObservableProperty] private string fullName;
        [ObservableProperty] private string email;
        [ObservableProperty] private string phoneNumber;
        [ObservableProperty] private string accountType;

        // Menu Collection
        public ObservableCollection<ProfileMenuItem> MenuItems { get; } = new();

        public ProfileViewModel()
        {
            LoadMockData();
        }

        private void LoadMockData()
        {
            // 1. Setup User Data (Matches your screenshot)
            Initials = "BN";
            FullName = "BEH NELSON";
            Email = "chubeh@gmail.com";
            PhoneNumber = "237 671000000";
            AccountType = "Personal";

            // 2. Setup Menu Items
            MenuItems.Clear();
            MenuItems.Add(new ProfileMenuItem("Settings", "settings_icon.png"));
            MenuItems.Add(new ProfileMenuItem("My Position", "position_icon.png"));
            MenuItems.Add(new ProfileMenuItem("Change Password", "lock_icon.png"));
            MenuItems.Add(new ProfileMenuItem("Verification", "verification_icon.png"));
        }

        [RelayCommand]
        private async Task EditProfile()
        {
            await Shell.Current.DisplayAlert("Edit", "Navigate to Edit Profile", "OK");
        }

        [RelayCommand]
        private async Task NavigateToItem(ProfileMenuItem item)
        {
            if (item == null) return;
            await Shell.Current.DisplayAlert("Navigate", $"Go to {item.Title}", "OK");
        }

        [RelayCommand]
        private async Task Logout()
        {
            bool confirm = await Shell.Current.DisplayAlert("Logout", "Are you sure you want to log out?", "Yes", "No");
            if (confirm)
            {
                await Shell.Current.GoToAsync("//Welcome");
            }
        }
    }

    public class ProfileMenuItem
    {
        public string Title { get; set; }
        public string IconSource { get; set; }

        public ProfileMenuItem(string title, string iconSource)
        {
            Title = title;
            IconSource = iconSource;
        }
    }
}