using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketPrice.Ui.Common;
using MarketPrice.Ui.Views;

namespace MarketPrice.Ui.ViewModels
{
    public partial class MarketViewModel : ObservableObject
    {

        [RelayCommand]
        private async Task NavigateToPlaceBidAsync()
        {
            await Shell.Current.GoToAsync(nameof(PlacePosition), new Dictionary<string, object>
            {
                [NavigationKeys.PositionType] = PositionType.Bid
            });
        }
        
        [RelayCommand]
        private async Task NavigateToPlaceOfferAsync()
        {
            await Shell.Current.GoToAsync(nameof(PlacePosition), new Dictionary<string, object>
            {
                [NavigationKeys.PositionType] = PositionType.Offer
            });
        }
    }
}
