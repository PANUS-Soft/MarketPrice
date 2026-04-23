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
using MarketPrice.Domain.Profile.DTOs;

namespace MarketPrice.Ui.ViewModels
{
    public partial class ProfileViewModel : ObservableObject
    {
        private readonly AuthenticationApiService _authenticationApi;
        private readonly ProfileApiService _profileApi;
        private readonly SessionService _sessionApi;

        private UserProfileResponseDto userProfile;

        [ObservableProperty] private string firstName;
        [ObservableProperty] private string familyName;
        [ObservableProperty] private string otherName;
        [ObservableProperty] private string fullName;
        [ObservableProperty] private string emailAddress;
        [ObservableProperty] private string phoneNumber;
        [ObservableProperty] private string accountType;

        public ProfileViewModel(AuthenticationApiService authenticationApiService, ProfileApiService profileApiService, SessionService sessionService)
        {
            _authenticationApi = authenticationApiService;
            _profileApi = profileApiService;
            _sessionApi = sessionService;
        }

        public async Task LoadUserProfileAsync()
        {
            var session = await _sessionApi.GetCurrentSessionAsync();

            if (session == null) return;

            var userId = session.UserId;

            var userProfileResponse = await _profileApi.GetUserProfileAsync(userId);
            if (!userProfileResponse.IsSuccessStatusCode) return;
            var userProfileDto = await userProfileResponse.Content.ReadFromJsonAsync<UserProfileResponseDto>();
            if (userProfileDto == null) return;

            long number = long.Parse(userProfileDto.PhoneNumber);

            FirstName = userProfileDto.FirstName.ToUpper();
            FamilyName = userProfileDto.FamilyName.ToUpper();
            OtherName = userProfileDto.OtherName.ToUpper() ?? "";
            FullName = userProfileDto.OtherName == "" ? $"{FirstName} {FamilyName}" : $"{FirstName} {FamilyName} {OtherName}";
            EmailAddress = userProfileDto.EmailAddress;
            PhoneNumber = $"{number:+### ### ## ## ##}";
            AccountType = userProfileDto.AccountType;

            userProfile = userProfileDto;
        }

        public async Task InitializeAsync()
        {
            await LoadUserProfileAsync();
            LoadProfileComponentAsync();
        }

        private void LoadProfileComponentAsync()
        {
            MenuItems.Clear();
            MenuItems.Add(new ProfileMenuItem("Settings","", "settings_icon.png"));
            MenuItems.Add(new ProfileMenuItem("My Position", "","position_icon.png"));
            MenuItems.Add(new ProfileMenuItem("Change Password", "ChangePassword", "lock_icon.png"));
            MenuItems.Add(new ProfileMenuItem("Verification", "", "verification_icon.png"));
        }

        // Menu Collection
        public ObservableCollection<ProfileMenuItem> MenuItems { get; } = new();

        [RelayCommand]
        private async Task NavigateToEditProfileAsync()
        {
            await Shell.Current.GoToAsync("EditProfile", new Dictionary<string, object>
            {
                {"UserProfile", userProfile}
            });
        }

        [RelayCommand]
        private async Task NavigateToItem(ProfileMenuItem? item)
        {
            if (item == null) return;
            if (item.MenuItemView == "") return;
            await Shell.Current.GoToAsync(item.MenuItemView);
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
            bool confirmLogout = await Shell.Current.DisplayAlert("Logout", "Are you sure you want to log out?", "Yes", "No");

            if (!confirmLogout) return;

            try
            {
                var userSession = await _sessionApi.GetCurrentSessionAsync();

                var command = new LogoutCommand { EmailAddress = userSession!.EmailAddress };

                var response = await _authenticationApi.LogoutUserAsync(command);

                if (response.IsSuccessStatusCode)
                {
                    var dto = await response.Content.ReadFromJsonAsync<LogoutResponseDto>();

                    if (dto is { LogoutStatus: true })
                    {
                        var isSessionEnded = await _sessionApi.EndSessionAsync();
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

    public class ProfileMenuItem(string title, string? menuItemView, string iconSource)
    {
        public string Title { get; set; } = title;
        public string? MenuItemView { get; set; } = menuItemView;
        public string IconSource { get; set; } = iconSource;
    }
}