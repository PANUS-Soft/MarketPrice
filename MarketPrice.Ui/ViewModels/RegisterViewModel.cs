    using MarketPrice.Domain.Authentication.Commands;
using MarketPrice.Ui.Extensions;
using MarketPrice.Ui.Models;
using MarketPrice.Ui.Services.Api;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
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
            ShowTermsOfServiceCommand = new Command(async () => await ShowTermsOfService());
            ShowPrivacyPolicyCommand = new Command(async () => await ShowPrivacyPolicy());
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
        private async Task ShowPrivacyPolicy()
        {
            await Shell.Current.DisplayAlert("Terms of Services", "By using MarketPrice, you agree to use the platform responsibly and provide accurate information.\n\n" + "Market prices are shared for guidance and change over time. Missue, fraud, or manipulation of data may lead to account suspension\n\n" + "MarketPrice may update or modify services to improve performance ad security.", "OK");
        }

        private async Task ShowTermsOfService()
        {
            await Shell.Current.DisplayAlert("Privacy Policy", "Marketprice collects only essential information required to operate the app and improve user experience.\n\n" + "Your data is not sold or shared without consent. Reasonable security measures are applied to protect you information.\n\n" + "You may access or delete your data at any time.", "OK");
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
                string errorMessage = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    await Shell.Current.DisplayAlert("Success", "Your account has been created successfully.", "OK");
                    await Shell.Current.GoToAsync("//Login");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", $"There was an error creating your account. {errorMessage}", "OK");
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
