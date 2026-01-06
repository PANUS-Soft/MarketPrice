using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace MarketPrice.Ui.ViewModels
{
    public partial class OnboardingViewModel : ObservableObject
    {

        [RelayCommand]
        private async Task NavigateToWelcomeAsync()
        {
            Preferences.Set("HasCompletedOnboarding", true);
            await Shell.Current.GoToAsync("//Welcome");
        }
    }
}
