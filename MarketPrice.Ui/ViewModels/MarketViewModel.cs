using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MarketPrice.Ui.ViewModels
{
    public partial class MarketViewModel : ObservableObject
    {

        [RelayCommand]
        public async Task NavigateToPlaceBid()
        {
            await Shell.Current.GoToAsync("PlaceBid");
        }
        
        [RelayCommand]
        public async Task NavigateToPlaceOffer()
        {
            await Shell.Current.GoToAsync("PlaceOffer");
        }
    }
}
