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

public partial class ChangePhoneNumberInputViewModel : ObservableObject, IQueryAttributable
{
    private readonly SessionService _sessionService;    
    private readonly ProfileApiService _profileApi;

    [ObservableProperty]
    private string currentPhoneNumber;

    [ObservableProperty]
    private string newPhoneNumber;

    public ChangePhoneNumberInputViewModel(SessionService sessionService, ProfileApiService profileApiService)
    {
        _sessionService = sessionService;
        _profileApi = profileApiService;
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task ContinueAsync()
    {
        if (string.IsNullOrWhiteSpace(NewPhoneNumber))
        {
            await Shell.Current.DisplayAlert(
                "Error ⚠️",
                "Please enter your new phone number.",
                "OK");

            return;
        }

        if (NewPhoneNumber.Length != 9)
        {
            await Shell.Current.DisplayAlert(
                "Invalid Phone Number ⚠️",
                "Please enter a valid phone number. Phone number must contain exactly 9 digits.",
                "OK");

            return;
        }

        string normalizedCurrent = CurrentPhoneNumber.Replace("+237", "").Replace(" ", "").Trim();

        if (normalizedCurrent == NewPhoneNumber)
        {
            await Shell.Current.DisplayAlert(
                "Error ⚠️",
                "Your new phone number must be different from the current one.",
                "OK");

            return;
        }

        string formattedNewPhoneNumber = $"+237{NewPhoneNumber}";

        long number = long.Parse(formattedNewPhoneNumber);
        string finalPhoneNumber = $"{number:+### ### ## ## ##}";


        // Later: Implement the update phone number logic
        try
        {
            var userSession = await _sessionService.GetCurrentSessionAsync();

            var command = new UpdateUserProfileCommand
            {
                UserId = userSession!.UserId,
                PhoneNumber = formattedNewPhoneNumber
            };

            var response = await _profileApi.UpdateUserProfileAsync(command);

            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadFromJsonAsync<UpdateUserProfileResponseDto>();

                if (dto == null) return;

                if (dto.Success)
                {
                    await Toast.Make("Phone number updated successfully.", ToastDuration.Short).Show();
                    await Shell.Current.GoToAsync("Settings");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error ⚠️", dto.Status, "OK");
                    return;
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

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("PhoneNumber"))
        {
            CurrentPhoneNumber = query["PhoneNumber"]?.ToString();
        }
    }
}