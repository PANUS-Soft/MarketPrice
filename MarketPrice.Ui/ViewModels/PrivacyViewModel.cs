using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketPrice.Domain.Profile.DTOs;
using MarketPrice.Ui.Services.Api;
using MarketPrice.Ui.Services.Session;
using System.Collections.ObjectModel;
using System.Net.Http.Json;

namespace MarketPrice.Ui.ViewModels;

public partial class PrivacyViewModel : ObservableObject, IQueryAttributable
{
    private readonly SessionService _sessionService;
    private readonly ProfileApiService _profileApi;

    private UserProfileResponseDto? userProfile;

    [ObservableProperty]
    private string fullName;

    [ObservableProperty]
    private string phoneNumber;

    public ObservableCollection<ProfileMenuItem> PrivacyItems { get; } = new();

    public PrivacyViewModel(
        SessionService sessionService,
        ProfileApiService profileApiService)
    {
        _sessionService = sessionService;
        _profileApi = profileApiService;

        LoadUserProfileAsync();
        LoadPrivacyItems();
    }

    private void LoadPrivacyItems()
    {
        PrivacyItems.Clear();

        PrivacyItems.Add(
            new ProfileMenuItem(
                "Change Password",
                "ChangePassword",
                "lock_icon.png",
                "Secure your account"));
    }

    public async void LoadUserProfileAsync()
    {
        var session = await _sessionService.GetCurrentSessionAsync();

        if (session == null)
            return;

        var response =
            await _profileApi.GetUserProfileAsync(session.UserId);

        if (!response.IsSuccessStatusCode)
            return;

        var dto =
            await response.Content.ReadFromJsonAsync<UserProfileResponseDto>();

        if (dto == null)
            return;

        userProfile = dto;

        FullName = string.IsNullOrWhiteSpace(dto.OtherName)
            ? $"{dto.FirstName.ToUpper()} {dto.FamilyName.ToUpper()}"
            : $"{dto.FirstName.ToUpper()} {dto.FamilyName.ToUpper()} {dto.OtherName.ToUpper()}";

    }

    [RelayCommand]
    private async Task NavigateToItem(ProfileMenuItem? item)
    {
        if (item == null)
            return;

        if (string.IsNullOrWhiteSpace(item.MenuItemView))
            return;

        await Shell.Current.GoToAsync(
            item.MenuItemView,
            new Dictionary<string, object>
            {
                { "UserProfile", userProfile }
            });
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("UserProfile", out var profileObj)
            && profileObj is UserProfileResponseDto profile)
        {
            userProfile = profile;

            FullName = string.IsNullOrWhiteSpace(profile.OtherName)
                ? $"{profile.FirstName.ToUpper()} {profile.FamilyName.ToUpper()}"
                : $"{profile.FirstName.ToUpper()} {profile.FamilyName.ToUpper()} {profile.OtherName.ToUpper()}";
        }
    }
}