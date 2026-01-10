using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace MarketPrice.Ui.ViewModels
{
    public partial class WelcomeViewModel : ObservableObject
    {

        [RelayCommand]
        private async Task NavigateToRegisterAsync()
        {
            await Shell.Current.GoToAsync("//Register");
        }

        [RelayCommand]
        private async Task NavigateToLoginAsync()
        {
            await Shell.Current.GoToAsync("//Login");
        }
    }
}
