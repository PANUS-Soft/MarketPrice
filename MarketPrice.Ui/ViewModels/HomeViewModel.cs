using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using MarketPrice.Domain.Home;
using MarketPrice.Ui.Models;
using MarketPrice.Ui.Services.Session;

namespace MarketPrice.Ui.ViewModels
{
    public partial class HomeViewModel(SessionService sessionService) : ObservableObject
    {
        public ObservableCollection<LoadHomeResponseDto> CommodityTypes { get; } = new();

        public async Task LoadHomeDataAsync()
        {
            var isSessionValid = await sessionService.ValidateAndRefreshSessionAsync();

            if (isSessionValid) await sessionService.GetCurrentSessionAsync();
            else await sessionService.TryRefreshTokenAsync();

            try
            {

            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error", $"There was an error loading data. {e.Message}", "OK");
            }

        }

        //[ObservableProperty]
        //private ObservableCollection<CommodityDisplayItem> _commodities;

        //public HomeViewModel()
        //{
        //    Commodities = new ObservableCollection<CommodityDisplayItem>();
        //    LoadMockData();
        //}

        //private void LoadMockData()
        //{
        //    // 1. BEANS
        //    Commodities.Add(new CommodityDisplayItem
        //    {
        //        Name = "BEANS",
        //        LotSize = "10Kg",
        //        ImageSource = "beans.jpeg",
        //        BackgroundColor = Color.FromArgb("#795548"), 
        //        BestBidPrice = "XAF 450",
        //        IsBidTrendUp = true,
        //        BestOfferPrice = "XAF 460",
        //        IsOfferTrendUp = false
        //    });

        //    // 2. CORN
        //    Commodities.Add(new CommodityDisplayItem
        //    {
        //        Name = "CORN",
        //        LotSize = "50Kg",
        //        ImageSource = "corn.jpeg",
        //        BackgroundColor = Color.FromArgb("#F39C12"), 
        //        BestBidPrice = "XAF 300",
        //        IsBidTrendUp = true,
        //        BestOfferPrice = "XAF 310",
        //        IsOfferTrendUp = true
        //    });

        //    // 3. EGUSI
        //    Commodities.Add(new CommodityDisplayItem
        //    {
        //        Name = "EGUSI",
        //        LotSize = "25Kg",
        //        ImageSource = "egusi.jpeg",
        //        BackgroundColor = Color.FromArgb("#E67E22"), 
        //        BestBidPrice = "XAF 1200",
        //        IsBidTrendUp = false,
        //        BestOfferPrice = "XAF 1250",
        //        IsOfferTrendUp = false
        //    });

        //    // 4. GINGER
        //    Commodities.Add(new CommodityDisplayItem
        //    {
        //        Name = "GINGER",
        //        LotSize = "15Kg",
        //        ImageSource = "ginger.jpeg",
        //        BackgroundColor = Color.FromArgb("#F1C40F"), 
        //        BestBidPrice = "XAF 800",
        //        IsBidTrendUp = true,
        //        BestOfferPrice = "XAF 850",
        //        IsOfferTrendUp = true
        //    });

        //    // 5. ONION
        //    Commodities.Add(new CommodityDisplayItem
        //    {
        //        Name = "ONION",
        //        LotSize = "75Kg",
        //        ImageSource = "onion.jpeg",
        //        BackgroundColor = Color.FromArgb("#9B59B6"),
        //        BestBidPrice = "XAF 500",
        //        IsBidTrendUp = false,
        //        BestOfferPrice = "XAF 550",
        //        IsOfferTrendUp = true
        //    });

        //    // 6. PALM OIL
        //    Commodities.Add(new CommodityDisplayItem
        //    {
        //        Name = "PALM OIL",
        //        LotSize = "30Kg",
        //        ImageSource = "palmoil.jpeg",
        //        BackgroundColor = Color.FromArgb("#C0392B"), 
        //        BestBidPrice = "XAF 1000",
        //        IsBidTrendUp = true,
        //        BestOfferPrice = "XAF 1050",
        //        IsOfferTrendUp = false
        //    });
        //}

        [RelayCommand]
        private async Task NavigateToPlaceBid(CommodityDisplayItem item)
        {
            await Shell.Current.DisplayAlert("Bid Clicked", $"You selected {item.Name}", "OK");
            // await Shell.Current.GoToAsync("PlaceBid");
        }

        [RelayCommand]
        private async Task NavigateToPlaceOffer(CommodityDisplayItem item)
        {
            await Shell.Current.DisplayAlert("Offer Clicked", $"You selected {item.Name}", "OK");
            // await Shell.Current.GoToAsync("PlaceOffer");
        }
    }
}