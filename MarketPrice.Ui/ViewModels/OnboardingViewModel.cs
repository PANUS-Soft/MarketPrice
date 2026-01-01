using System.Windows.Input;

namespace MarketPrice.Ui.ViewModels
{
    public class OnboardingViewModel : BindableObject
    {
        public ICommand NavigateToWelcomeCommand { get; }

        public OnboardingViewModel()
        {
            NavigateToWelcomeCommand = new Command(NavigateToWelcome);
        }

        private async void NavigateToWelcome()
        {
            await Shell.Current.GoToAsync("//Welcome");
        }
    }
}
