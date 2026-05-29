using System.Net.Http.Json;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketPrice.Domain.Profile.Commands;
using MarketPrice.Domain.Profile.DTOs;
using MarketPrice.Ui.Services.Api;
using MarketPrice.Ui.Services.Session;

namespace MarketPrice.Ui.ViewModels;

public partial class ChangeEmailAddressInputViewModel : ObservableObject, IQueryAttributable
{
    private readonly SessionService _sessionService;
    private readonly ProfileApiService _profileApi;

    [ObservableProperty]
    private string currentEmailAddress;

    [ObservableProperty]
    private string newEmailAddress;

    public ChangeEmailAddressInputViewModel(ProfileApiService profileApiService, SessionService sessionService)
    {
        _sessionService = sessionService;
        _profileApi = profileApiService;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("EmailAddress"))
        {
            CurrentEmailAddress = query["EmailAddress"]?.ToString();
        }
    }

    bool IsValidEmailAddress(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        try
        {
            var address = new System.Net.Mail.MailAddress(email);
            return address.Address == email;
        }
        catch
        {
            return false;
        }
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task ContinueAsync()
    {
        if (string.IsNullOrWhiteSpace(NewEmailAddress))
        {
            await Shell.Current.DisplayAlert(
                "Error ⚠️",
                "Please enter a new email address.",
                "OK");

            return;
        }

        if (!IsValidEmailAddress(NewEmailAddress))
        {
            await Shell.Current.DisplayAlert("Invalid Email ⚠️", "Please enter a valid email address format", "OK");
            return;
        }

        if (CurrentEmailAddress == NewEmailAddress)
        {
            await Shell.Current.DisplayAlert("Error ⚠️",
                "Both emails are the same. Enter a different email address from the current email address.", "OK");
            return;
        }

        // Later: Implement the update email address logic
        try
        {
            var userSession = await _sessionService.GetCurrentSessionAsync();

            var command = new UpdateUserProfileCommand
            {
                UserId = userSession!.UserId,
                EmailAddress = NewEmailAddress
            };

            var response = await _profileApi.UpdateUserProfileAsync(command);

            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadFromJsonAsync<UpdateUserProfileResponseDto>();

                if (dto == null) return;

                if (dto.Success)
                {
                    await Toast.Make("Email updated successfully.", ToastDuration.Short).Show();
                    await Shell.Current.GoToAsync("Settings");
                } else
                {
                    await Shell.Current.DisplayAlert("Error ⚠️", dto.Status, "OK");
                }
            }
            else
            {
                var error = await response.Content.ReadFromJsonAsync<UpdateUserProfileResponseDto>();

                await Shell.Current.DisplayAlert(
                    "Error ⚠️",
                    $"There was an error when trying to update your profile. {error.Status}",
                    "OK");
            }
        }
        catch (Exception e)
        {
            await Shell.Current.DisplayAlert("Error ⚠️", $"An error occured. {e.Message}", "OK");
        }
    }
}