using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MarketPrice.Ui.Extensions;
using MarketPrice.Ui.Models;

namespace MarketPrice.Ui.ViewModels
{
    public class RegisterViewModel : BindableObject
    {
        public PersonalInfoModel PersonalInfo { get; } = new();
        public ContactAccountModel ContactInfo { get; } = new();
        public SecurityModel Security { get; } = new();

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

        public event Func<Task<bool>> ValidateCurrentStepRequested;

        public RegisterViewModel()
        {
            CurrentStep = RegistrationStep.PersonalInfo;
            ContinueCommand = new Command(async () => await TryContinueAsync());
            BackCommand = new Command(PreviousStep);
            NavigateToLoginCommand = new Command(NavigateToLogin);
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

        private async void NavigateToLogin()
        {
            await Shell.Current.GoToAsync("//Login");
        }

        private void CreateAccount()
        {
            throw new NotImplementedException();
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
