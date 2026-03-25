using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using MarketPrice.Domain.Profile.Commands;
using MarketPrice.Domain.Profile.DTOs;
using MarketPrice.Ui.Services.Api;
using MarketPrice.Ui.Services.Session;

namespace MarketPrice.Ui.ViewModels
{
    [QueryProperty(nameof(UserProfile), "UserProfile")]
    public partial class EditProfileViewModel : ObservableObject
    {
        private readonly ProfileApiService _profileApi;
        private readonly SessionService _sessionApi;

        [ObservableProperty] private string firstName;
        [ObservableProperty] private string familyName;
        [ObservableProperty] private string otherName;
        [ObservableProperty] private string emailAddress;
        [ObservableProperty] private string phoneNumber;

        public EditProfileViewModel(ProfileApiService profileApiService, SessionService sessionService)
        {
            _profileApi = profileApiService;
            _sessionApi = sessionService;
        }

        private UserProfileResponseDto userProfile;

        public UserProfileResponseDto UserProfile
        {
            get => userProfile;
            set
            {
                userProfile = value;
                {
                    FirstName = userProfile.FirstName;
                    FamilyName = userProfile.FamilyName;
                    OtherName = userProfile.OtherName ?? "";
                    EmailAddress = userProfile.EmailAddress;
                    PhoneNumber = userProfile.PhoneNumber.Substring(4);
                }
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

        bool IsValidPhoneNumber(string phone)
        {
            return !string.IsNullOrEmpty(phone) && System.Text.RegularExpressions.Regex.IsMatch(phone, @"^6\d{8}$");
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            // Check if fields were updated before launching a user profile update API request
            var isFirstNameUpdated = userProfile.FirstName != FirstName;
            var isFamilyNameUpdated = userProfile.FamilyName != FamilyName;
            var isOtherNameUpdated = userProfile.OtherName != OtherName;
            var isEmailAddressUpdated = userProfile.EmailAddress != EmailAddress;
            var isPhoneNumberUpdated = userProfile.PhoneNumber.Substring(4) != PhoneNumber;

            if (!isFirstNameUpdated && !isFamilyNameUpdated && !isOtherNameUpdated && !isEmailAddressUpdated &&
                !isPhoneNumberUpdated)
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

            if (!IsValidEmailAddress(EmailAddress))
            {
                await Shell.Current.DisplayAlert("Validation Error ⚠️", "Please enter a valid email address.", "OK");
                return;
            }

            if (!IsValidPhoneNumber(PhoneNumber))
            {
                await Shell.Current.DisplayAlert("Validation Error ⚠️", "Please enter a valid phone number.", "OK");
                return;
            }

            try
            {
                var userSession = await _sessionApi.GetCurrentSessionAsync();

                var command = new UpdateUserProfileCommand
                {
                    UserId = userSession!.UserId,
                    FirstName = FirstName,
                    FamilyName = FamilyName,
                    OtherNames = OtherName,
                    EmailAddress = EmailAddress,
                    PhoneNumber = $"+237{PhoneNumber}"
                };

                var message = $"""""
                               Are you sure that you want to update your profile. Verify the updates.

                               First Name: {command.FirstName}
                               Family Name: {command.FamilyName}
                               OtherName: {command.OtherNames}
                               Email Address: {command.EmailAddress}
                               Phone Number: {command.PhoneNumber}
                               """"";

                var confirmUserProfileUpdate = await Shell.Current.DisplayAlert("Confirm Profile Update", message, "Update Profile", "Cancel");

                if (!confirmUserProfileUpdate) return;

                var response = await _profileApi.UpdateUserProfileAsync(command);

                var dto = await response.Content.ReadFromJsonAsync<UpdateUserProfileResponseDto>();

                if (dto == null) return;

                if (response.IsSuccessStatusCode)
                {
                    if (dto.Status)
                    {
                        await Toast.Make(dto.Message, ToastDuration.Long).Show();
                        await Shell.Current.GoToAsync("..");
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Profile Update Failed", dto.Message, "OK");
                }
            }
            catch(Exception e)
            {
                await Shell.Current.DisplayAlert("Error", $"An error occured. {e.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task BackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task DiscardAsync()
        {
            var confirmFieldsDiscard = await Shell.Current.DisplayAlert("Clear Input Fields",
                "Are you sure that you want to clear the input fields ?", "Yes", "No");

            if (!confirmFieldsDiscard) return;

            FirstName = string.Empty;
            FamilyName = string.Empty;
            OtherName = string.Empty;
            EmailAddress = string.Empty;
            PhoneNumber = string.Empty;
        }
    }
}