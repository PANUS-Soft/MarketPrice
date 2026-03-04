using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketPrice.Domain.Authentication.Commands;
using MarketPrice.Domain.Authentication.DTOs;
using MarketPrice.Ui.Services.Api;
using MarketPrice.Ui.Services.Session;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using CommunityToolkit.Maui.Alerts;

namespace MarketPrice.Ui.ViewModels
{
    public partial class ProfileViewModel : ObservableObject
    {
        public readonly AuthenticationApiService _authenticationApi;
        public readonly SessionService _sessionService;

        public ProfileViewModel(AuthenticationApiService authenticationApiService, SessionService sessionService)
        {
            _authenticationApi = authenticationApiService;
            _sessionService = sessionService;
        }

        public async Task InitializeAsync()
        {
            LoadMockData();
        }

        // User Details
        [ObservableProperty] private string initials;
        [ObservableProperty] private string fullName;
        [ObservableProperty] private string email;
        [ObservableProperty] private string phoneNumber;
        [ObservableProperty] private string accountType;

        // Menu Collection
        public ObservableCollection<ProfileMenuItem> MenuItems { get; } = new();

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
        private async Task NavigateToEditProfileAsync()
        {
            await Shell.Current.GoToAsync("EditProfile");
        }

        [RelayCommand]
        private async Task NavigateToItem(ProfileMenuItem item)
        {
            if (item == null) return;
            await Shell.Current.DisplayAlert("Navigate", $"Go to {item.Title}", "OK");
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
            bool confirmLogout = await Shell.Current.DisplayAlert("Logout", "Are you sure you want to log out?", "Yes", "No");

            if (!confirmLogout) return;

            try
            {
                var userSession = await _sessionService.GetCurrentSessionAsync();

                var command = new LogoutCommand { EmailAddress = userSession!.EmailAddress };

                var response = await _authenticationApi.LogoutUserAsync(command);

                if (response.IsSuccessStatusCode)
                {
                    var dto = await response.Content.ReadFromJsonAsync<LogoutResponseDto>();

                    if (dto is { LogoutStatus: true })
                    {
                        var isSessionEnded = await _sessionService.EndSessionAsync();
                        if (isSessionEnded)
                        {
                            await Toast.Make("You have been logged out successfully.", ToastDuration.Long).Show();
                            await Shell.Current.GoToAsync("//Welcome");
                        }
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "Failed to log out. Please try again.", "OK");
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to log out. Please try again.", "OK");
                }
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error", $"There was an error when trying to log you out ... {e.Message}", "OK");
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