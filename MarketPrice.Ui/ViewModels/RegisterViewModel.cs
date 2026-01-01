    using MarketPrice.Domain.Authentication.Commands;
using MarketPrice.Ui.Extensions;
using MarketPrice.Ui.Models;
using MarketPrice.Ui.Services.Api;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;

namespace MarketPrice.Ui.ViewModels
{
    public class RegisterViewModel : BindableObject
    {
        private readonly RegisterApiService _registerApi;

        public PersonalInformation PersonalInfo { get; } = new();
        public ContactInformation ContactInfo { get; } = new();
        public SecurityDetails SecurityDetail { get; } = new();

        public string CurrentStepDisplay => CurrentStep.GetDisplayName();

        private RegistrationStep _currentStep;
        public RegistrationStep CurrentStep
        {
            get => _currentStep;
            set
            {
                _currentStep = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsPersonalStep));
                OnPropertyChanged(nameof(IsContactStep));
                OnPropertyChanged(nameof(IsSecurityStep));
                OnPropertyChanged(nameof(CurrentStepDisplay));
                OnPropertyChanged(nameof(ButtonText));
            }
        }

        public bool IsPersonalStep => CurrentStep == RegistrationStep.PersonalInfo;
        public bool IsContactStep => CurrentStep == RegistrationStep.ContactAccount;
        public bool IsSecurityStep => CurrentStep == RegistrationStep.Security;

        public ICommand ContinueCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand NavigateToLoginCommand { get; }
        public ICommand ShowTermsOfServiceCommand { get; }
        public ICommand ShowPrivacyPolicyCommand { get; }

        public event Func<Task<bool>> ValidateCurrentStepRequested;

        public string ButtonText => CurrentStep == RegistrationStep.Security ? "Create my account" : "Continue";

        public RegisterViewModel(RegisterApiService? registerApi)
        {
            _registerApi = registerApi;
            CurrentStep = RegistrationStep.PersonalInfo;
            ContinueCommand = new Command(async () => await TryContinueAsync());
            BackCommand = new Command(PreviousStep);
            NavigateToLoginCommand = new Command(NavigateToLogin);
            ShowTermsOfServiceCommand = new Command(ShowTermsOfService);
            ShowPrivacyPolicyCommand = new Command(ShowPrivacyPolicy);
        }

        private async Task TryContinueAsync()
        {
            if (ValidateCurrentStepRequested != null)
            {
                bool isValid = await ValidateCurrentStepRequested.Invoke();
                if (!isValid)
                    return;
            }

            if (CurrentStep == RegistrationStep.Security)
            {
                await CreateAccountAsync();
                return;
            }

            NextStep();
        }

        private void NextStep()
        {
            if (CurrentStep == RegistrationStep.PersonalInfo)
                CurrentStep = RegistrationStep.ContactAccount;
            else if (CurrentStep == RegistrationStep.ContactAccount)
                CurrentStep = RegistrationStep.Security;
        }

        private void PreviousStep()
        {
            if (CurrentStep == RegistrationStep.Security)
                CurrentStep = RegistrationStep.ContactAccount;
            else if (CurrentStep == RegistrationStep.ContactAccount)
                CurrentStep = RegistrationStep.PersonalInfo;
        }

        private void NavigateToLogin()
        {
            Shell.Current.GoToAsync("//Login");
        }
        private void ShowPrivacyPolicy(object obj)
        {
        }

        private void ShowTermsOfService()
        {
        }

        private async Task CreateAccountAsync()
        {
            try
            {
                var accounTypeId = ContactInfo.AccountType == AccountType.Personal ? 1001 : 1002;

                var registerRequest = new RegisterCommand
                {
                    FirstName = PersonalInfo.FirstName,
                    FamilyName = PersonalInfo.FamilyName,
                    OtherNames = PersonalInfo.OtherName,
                    AccountTypeId = accounTypeId,
                    EmailAddress = ContactInfo.EmailAddress,
                    PhoneNumber = $"+237{ContactInfo.PhoneNumber}",
                    Password = SecurityDetail.Password
                };

                var response = await _registerApi.RegisterUserAsync(registerRequest);
                string errorCount = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    await Shell.Current.DisplayAlert("Success", "Your account has been created successfully.", "OK");
                    await Shell.Current.GoToAsync("//Login");
                }

                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    await Shell.Current.DisplayAlert("Error", "There was an error creating your account. An account already exists with these details.", "OK");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    await Shell.Current.DisplayAlert("Error", $"Invalid request data. {errorCount}", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", $"There was an error creating your account. {errorCount}", "OK");
                }
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error", e.Message, "OK");
            }
        }
    }

    public enum RegistrationStep
    {
        [Display(Name = "personal information")]
        PersonalInfo,

        [Display(Name = "contact details and account information")]
        ContactAccount,

        [Display(Name = "security details")]
        Security
    }

}
