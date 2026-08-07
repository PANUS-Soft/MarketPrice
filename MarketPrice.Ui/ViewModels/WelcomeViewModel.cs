using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketPrice.Ui.Common;

namespace MarketPrice.Ui.ViewModels
{
    public partial class WelcomeViewModel : ObservableObject, IQueryAttributable
    {
        private string? _redirectTo;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            _redirectTo = AuthenticationNavigation.ReadDestination(query);
        }

        [RelayCommand]
        private async Task NavigateToRegisterAsync()
        {
            await Shell.Current.GoToAsync("//Register");
        }

        [RelayCommand]
        private async Task NavigateToLoginAsync()
        {
            if (_redirectTo != null)
                await AuthenticationNavigation.NavigateToLoginAsync(_redirectTo);
            else
                await Shell.Current.GoToAsync("//Login");
        }
    }
}
