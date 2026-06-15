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
        private readonly SessionService _sessionService;

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
            _sessionService = sessionService;
        }

        public bool IsUserLoggedIn => _sessionService.IsLoggedIn;
        

        public async Task LoadUserProfileAsync()
        {
            var session = await _sessionService.GetCurrentSessionAsync();

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
            MenuItems.Add(new ProfileMenuItem("Settings","Settings", "settings_icon.png", "General app preferences"));
            MenuItems.Add(new ProfileMenuItem("Verification", "", "verification_icon.png", "Identity status", "NOT VERIFIED"));
        }

        // Menu Collection
        public ObservableCollection<ProfileMenuItem> MenuItems { get; } = new();

        [RelayCommand]
        private async Task NavigateToRegisterAsync()
        {
            await Shell.Current.GoToAsync("//Register");
        }

        [RelayCommand]
        private async Task NavigateToLoginAsync()
        {
            await Shell.Current.GoToAsync("//Login");
        }

        [RelayCommand]
        private async Task NavigateToEditProfileAsync()
        {
            await Shell.Current.GoToAsync("Account", new Dictionary<string, object>
            {
                {"UserProfile", userProfile}
            });
        }

        [RelayCommand]
        private async Task NavigateToItem(ProfileMenuItem? item)
        {
            if (item == null) return;
            if (item.MenuItemView == "") return;
            await Shell.Current.GoToAsync(item.MenuItemView, new Dictionary<string, object>
            {
                {"UserProfile", userProfile}
            });
        }
    }

    public class ProfileMenuItem(string title, string? menuItemView, string iconSource, string subTitle = "", string badgeText = "")
    {
        public string Title { get; set; } = title;
        public string? MenuItemView { get; set; } = menuItemView;
        public string IconSource { get; set; } = iconSource;
        public string SubTitle { get; set; } = subTitle;
        public string BadgeText { get; set; } = badgeText;
        public bool HasBadge => !string.IsNullOrEmpty(BadgeText);
    }
}