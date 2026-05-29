using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketPrice.Domain.Profile.DTOs;
using MarketPrice.Ui.Models;
using MarketPrice.Ui.Services.Session;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Net.Mail;
using MarketPrice.Ui.Services.Api;

namespace MarketPrice.Ui.ViewModels
{
    public partial class SettingsViewModel : ObservableObject, IQueryAttributable
    {
        private readonly SessionService _sessionService;
        private readonly ProfileApiService _profileApi;

        [ObservableProperty] private string fullName;
        [ObservableProperty] private string phoneNumber;
        private UserProfileResponseDto? userProfile;

        public ObservableCollection<SettingsMenuItem> SettingsItems { get; } = new();

        public SettingsViewModel(SessionService sessionService, ProfileApiService profileApiService)
        {
            _sessionService = sessionService;
            _profileApi = profileApiService;

            LoadHeader();
            LoadUserProfileAsync();
            LoadSettings();
        }

        private async void LoadHeader()
        {
            var session = await _sessionService.GetCurrentSessionAsync();

            if (session == null)
                return;
        }

        public async void LoadUserProfileAsync()
        {
            var session = await _sessionService.GetCurrentSessionAsync();

            if (session == null) return;

            var userId = session.UserId;

            var userProfileResponse = await _profileApi.GetUserProfileAsync(userId);
            if (!userProfileResponse.IsSuccessStatusCode) return;
            var userProfileDto = await userProfileResponse.Content.ReadFromJsonAsync<UserProfileResponseDto>();
            if (userProfileDto == null) return;

            userProfile = userProfileDto;

            FullName = userProfile.OtherName == ""
                ? $"{userProfile.FirstName.ToUpper()} {userProfile.FamilyName.ToUpper()}"
                : $"{userProfile.FirstName.ToUpper()} {userProfile.FamilyName.ToUpper()} {userProfile.OtherName.ToUpper()}";

            long number = long.Parse(userProfile.PhoneNumber);

            PhoneNumber = $"{number:+### ### ## ## ##}";
        }

        private void LoadSettings()
        {
            SettingsItems.Clear();

            SettingsItems.Add(new SettingsMenuItem(
                "Account",
                "Account",
                "account_icon",
                "Name, Number, Email"));
            
            SettingsItems.Add(new SettingsMenuItem(
                "Notifications",
                "Notifications",
                "notification_icon",
                "Manage alerts and updates"));

            SettingsItems.Add(new SettingsMenuItem(
                "Privacy",
                "Privacy",
                "privacy_and_security_icon",
                "Control account privacy"));

            SettingsItems.Add(new SettingsMenuItem(
                "Appearance",
                "Appearance",
                "theme_icon",
                "Dark mode and themes"));

            SettingsItems.Add(new SettingsMenuItem(
                "Language",
                "Language",
                "language_icon",
                "Choose your preferred language"));

            SettingsItems.Add(new SettingsMenuItem(
                "Help & Support",
                "Support",
                "support_icon",
                "Need assistance?"));
        }

        [RelayCommand]
        private async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task OpenMenuAsync()
        {
            await Shell.Current.DisplayActionSheet(
                "Options",
                "Cancel",
                null,
                "Refresh",
                "About");
        }

        [RelayCommand]
        private async Task NavigateToItemAsync(SettingsMenuItem? item)
        {
            if (item == null || string.IsNullOrEmpty(item.Route))
                return;

            await Shell.Current.GoToAsync(item.Route, new Dictionary<string, object>
            {
                {"UserProfile", userProfile}
            });
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            // 1. Safe check: Only run if the parameter exists and matches the right type
            if (query.TryGetValue("UserProfile", out var profileObj) && profileObj is UserProfileResponseDto updatedProfile)
            {
                userProfile = updatedProfile;

                // 2. Safely parse names using string.IsNullOrWhiteSpace
                FullName = string.IsNullOrWhiteSpace(userProfile.OtherName)
                    ? $"{userProfile.FirstName.ToUpper()} {userProfile.FamilyName.ToUpper()}"
                    : $"{userProfile.FirstName.ToUpper()} {userProfile.FamilyName.ToUpper()} {userProfile.OtherName.ToUpper()}";

                // 3. Safely parse phone number to avoid unhandled formatting crashes
                if (long.TryParse(userProfile.PhoneNumber, out long number))
                {
                    PhoneNumber = $"{number:+### ### ## ## ##}";
                }
                else
                {
                    PhoneNumber = userProfile.PhoneNumber; // Fallback if parsing fails
                }
            }
            // 4. If query doesn't contain "UserProfile", it safely exits without crashing.
        }
    }
}