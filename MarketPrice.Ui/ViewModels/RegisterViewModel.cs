using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MarketPrice.Domain.Authentication.Commands;
using MarketPrice.Ui.Extensions;
using MarketPrice.Ui.Models;

namespace MarketPrice.Ui.ViewModels
{
    public class RegisterViewModel : BindableObject
    {
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

        public RegisterViewModel()
        {
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
            NextStep();
        }

        private void NextStep()
        {
            if (CurrentStep == RegistrationStep.PersonalInfo)
                CurrentStep = RegistrationStep.ContactAccount;
            else if (CurrentStep == RegistrationStep.ContactAccount)
                CurrentStep = RegistrationStep.Security;
            else
                CreateAccount();
        }

        private void PreviousStep()
        {
            if(CurrentStep == RegistrationStep.Security)
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

        private void CreateAccount()
        {
            var user = new RegisterCommand
            {
              
            };
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
