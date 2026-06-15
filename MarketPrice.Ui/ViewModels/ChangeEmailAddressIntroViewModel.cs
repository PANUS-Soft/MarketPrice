using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketPrice.Ui.Views;

namespace MarketPrice.Ui.ViewModels;

public partial class ChangeEmailAddressIntroViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private string currentEmailAddress;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("EmailAddress"))
        {
            CurrentEmailAddress = query["EmailAddress"]?.ToString();
        }
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task NavigateToNewEmailAsync()
    {
        await Shell.Current.GoToAsync("ChangeEmailAddressInput", new Dictionary<string, object>
        {
            {"EmailAddress", CurrentEmailAddress}
        });
    }
}