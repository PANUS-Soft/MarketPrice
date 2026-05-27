using System.Net.Http.Json;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketPrice.Domain.Authentication.Commands;
using MarketPrice.Domain.Authentication.DTOs;
using MarketPrice.Ui.Services.Api;
using MarketPrice.Ui.Services.Session;
using System.Net.Mail;
using MarketPrice.Domain.Profile.DTOs;

namespace MarketPrice.Ui.ViewModels;

public partial class AccountViewModel : ObservableObject, IQueryAttributable
{
    private readonly AuthenticationApiService _authenticationApi;
    private readonly SessionService _sessionService;

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



    public AccountViewModel(SessionService sessionService, AuthenticationApiService authenticationApiService)
    {
        _sessionService = sessionService;
        _authenticationApi = authenticationApiService;
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

        if (query.ContainsKey("PhoneNumber"))
            PhoneNumber = query["PhoneNumber"]?.ToString();

        if (query.ContainsKey("EmailAddress"))
            EmailAddress = query["EmailAddress"]?.ToString();
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task GoToChangePhoneNumberAsync()
    {
        await Shell.Current.GoToAsync("ChangePhoneNumberIntro");
    }

    [RelayCommand]
    private async Task GoToChangeEmailAddressAsync()
    {
        await Shell.Current.GoToAsync("ChangeEmailAddress");
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
        await Shell.Current.DisplayAlert(
            "Saved",
            "Account updated successfully.",
            "OK");
    }
}