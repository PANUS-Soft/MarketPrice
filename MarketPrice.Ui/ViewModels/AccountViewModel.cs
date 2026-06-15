using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketPrice.Domain.Authentication.Commands;
using MarketPrice.Domain.Authentication.DTOs;
using MarketPrice.Domain.Profile.Commands;
using MarketPrice.Domain.Profile.DTOs;
using MarketPrice.Ui.Services.Api;
using MarketPrice.Ui.Services.Session;
using System.Net.Http.Json;
using System.Net.Mail;

namespace MarketPrice.Ui.ViewModels;

public partial class AccountViewModel : ObservableObject, IQueryAttributable
{
    private readonly AuthenticationApiService _authenticationApi;
    private readonly SessionService _sessionService;
    private readonly ProfileApiService _profileApi;

    private UserProfileResponseDto? userProfile;

    [ObservableProperty]
    private string firstName;

    [ObservableProperty]
    private string familyName;

    [ObservableProperty] 
    private string otherName;

    [ObservableProperty]
    private string fullName;

    [ObservableProperty]
    private string phoneNumber;

    [ObservableProperty]
    private string emailAddress;

    [ObservableProperty]
    private string bio;

    [ObservableProperty] 
    private string accountType;



    public AccountViewModel(SessionService sessionService, AuthenticationApiService authenticationApiService, ProfileApiService profileApiService)
    {
        _sessionService = sessionService;
        _authenticationApi = authenticationApiService;
        _profileApi = profileApiService;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("UserProfile"))
        {
            userProfile = query["UserProfile"] as UserProfileResponseDto;

            if (userProfile != null)
            {
                FirstName = userProfile.FirstName;
                FamilyName = userProfile.FamilyName;
                OtherName = userProfile.OtherName;

                Bio = userProfile.Bio;

                long number = long.Parse(userProfile.PhoneNumber);
                PhoneNumber = $"{number:+### ### ## ## ##}";
                EmailAddress = userProfile.EmailAddress;
                AccountType = userProfile.AccountType;
            }

        }
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task GoToChangePhoneNumberAsync()
    {
        await Shell.Current.GoToAsync("ChangePhoneNumberIntro", new Dictionary<string, object>
        {
            {"PhoneNumber", PhoneNumber}
        });
    }

    [RelayCommand]
    private async Task GoToChangeEmailAddressAsync()
    {
        await Shell.Current.GoToAsync("ChangeEmailAddressIntro", new Dictionary<string, object>
        {
            {"EmailAddress", EmailAddress}
        });
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
                        await Toast.Make("You have been logged out successfully.", ToastDuration.Short).Show();
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

    [RelayCommand]
    private async Task SaveAsync()
    {
        // Check if fields were updated before launching a user profile update API request
        var isFirstNameUpdated = userProfile.FirstName != FirstName;
        var isFamilyNameUpdated = userProfile.FamilyName != FamilyName;
        var isOtherNameUpdated = userProfile.OtherName != OtherName;
        var isBioUpdated = userProfile.Bio != Bio;

        if (!isFirstNameUpdated && !isFamilyNameUpdated && !isOtherNameUpdated && !isBioUpdated)
        {
            await Shell.Current.DisplayAlert("Ooops ⚠️",
                "No updates perform on the user fields. Try reviewing your input before attempting to update your profile.",
                "OK");
            return;
        }

        if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(FamilyName) || string.IsNullOrEmpty(EmailAddress) || string.IsNullOrEmpty(PhoneNumber))
        {
            await Shell.Current.DisplayAlert("Validation Error ⚠️",
                "Please fill in the required fields before updating your profile.", "OK");
            return;
        }

        try
        {
            var userSession = await _sessionService.GetCurrentSessionAsync();

            var command = new UpdateUserProfileCommand
            {
                UserId = userSession!.UserId,
                FirstName = FirstName,
                FamilyName = FamilyName,
                OtherNames = OtherName,
                Bio = Bio
            };

            var response = await _profileApi.UpdateUserProfileAsync(command);

            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadFromJsonAsync<UpdateUserProfileResponseDto>();

                if (dto == null) return;

                if (dto.Success)
                {
                    await Shell.Current.GoToAsync("..");
                }
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();

                await Shell.Current.DisplayAlert(
                    "Error ⚠️",
                    $"There was an error when trying to update your profile. {error}",
                    "OK");
            }
        }
        catch (Exception e)
        {
            await Shell.Current.DisplayAlert("Error ⚠️", $"An error occured. {e.Message}", "OK");
        }
    }
}