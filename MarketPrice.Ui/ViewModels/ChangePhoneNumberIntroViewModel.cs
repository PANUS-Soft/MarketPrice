using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketPrice.Ui.Views;

namespace MarketPrice.Ui.ViewModels;

public partial class ChangePhoneNumberIntroViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private string currentPhoneNumber;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("PhoneNumber"))
        {
            CurrentPhoneNumber = query["PhoneNumber"]?.ToString();
        }
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task NavigateToNewPhoneAsync()
    {
        await Shell.Current.GoToAsync(nameof(ChangePhoneNumberInput));
    }
}