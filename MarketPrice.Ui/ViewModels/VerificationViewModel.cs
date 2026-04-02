using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace MarketPrice.Ui.ViewModels
{
    [QueryProperty(nameof(VerificationType), "type")]
    public partial class VerificationViewModel : ObservableObject
    {
        // --- STATE MANAGEMENT ---

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsPhoneMode))]
        [NotifyPropertyChangedFor(nameof(IsEmailMode))]
        [NotifyPropertyChangedFor(nameof(Step1Subtitle))]
        [NotifyPropertyChangedFor(nameof(Step1Description))]
        [NotifyPropertyChangedFor(nameof(Step2Description))]
        [NotifyPropertyChangedFor(nameof(ChangeContactText))]
        [NotifyPropertyChangedFor(nameof(ToggleModeText))]
        private string verificationType = "Phone"; // Defaults to Phone

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsStep1))]
        [NotifyPropertyChangedFor(nameof(IsStep2))]
        private int currentStep = 1;

        // Dynamic State Checks for the XAML
        public bool IsPhoneMode => VerificationType == "Phone";
        public bool IsEmailMode => VerificationType == "Email";
        public bool IsStep1 => CurrentStep == 1;
        public bool IsStep2 => CurrentStep == 2;

        // Dynamic Text Properties based on Mode
        public string Step1Subtitle => IsPhoneMode ? "Enter Phone Number" : "Enter Email";
        public string Step1Description => IsPhoneMode ? "We'll send a code to your phone number" : "We'll send a code to your email";
        public string Step2Description => IsPhoneMode ? "We have sent a code to your phone number" : "We have sent a code to your email";
        public string ChangeContactText => IsPhoneMode ? "Change Phone Number" : "Change Email";

        // Text for the new toggle button
        public string ToggleModeText => IsPhoneMode ? "Use Email Instead" : "Use Phone Number Instead";

        // --- INPUT PROPERTIES ---

        [ObservableProperty] private string phoneNumber;
        [ObservableProperty] private string emailAddress;

        // 6-Digit OTP Code
        [ObservableProperty] private string digit1;
        [ObservableProperty] private string digit2;
        [ObservableProperty] private string digit3;
        [ObservableProperty] private string digit4;
        [ObservableProperty] private string digit5;
        [ObservableProperty] private string digit6;


        // --- COMMANDS ---

        [RelayCommand]
        private void ToggleMode()
        {
            // Switches the UI instantly between Phone and Email
            VerificationType = IsPhoneMode ? "Email" : "Phone";
        }

        [RelayCommand]
        private void SendCode()
        {
            CurrentStep = 2;
        }

        [RelayCommand]
        private async Task VerifyCodeAsync()
        {
            await Shell.Current.DisplayAlert("Success", "Verification Successful!", "OK");
            await Shell.Current.GoToAsync("//Home");
        }

        [RelayCommand]
        private void ChangeContact()
        {
            CurrentStep = 1;
            Digit1 = Digit2 = Digit3 = Digit4 = Digit5 = Digit6 = string.Empty;
        }

        [RelayCommand]
        private async Task ResendCodeAsync()
        {
            await Shell.Current.DisplayAlert("Code Sent", "A new verification code has been sent.", "OK");
        }

        [RelayCommand]
        private async Task GoBackAsync()
        {
            if (CurrentStep == 2)
            {
                ChangeContact();
            }
            else
            {
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}