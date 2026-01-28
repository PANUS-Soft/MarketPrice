using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketPrice.Ui.Common;
using MarketPrice.Ui.Models;
using MarketPrice.Ui.Views;
using System.Collections.ObjectModel;

namespace MarketPrice.Ui.ViewModels
{
    [QueryProperty(nameof(SelectedMarketItem), "SelectedMarketItem")]
    public partial class MarketInsightViewModel : ObservableObject
    {
        [ObservableProperty]
        MarketItem? selectedMarketItem;

        public ObservableCollection<PricePoint>? PriceHistory { get; set; }

        public ObservableCollection<DepthItem>? MarketDepthBids { get; set; }
        public ObservableCollection<DepthItem>? MarketDepthOffers { get; set; }

        public MarketInsightViewModel()
        {
            PriceHistory = new ObservableCollection<PricePoint>()
           {
               new(DateTime.Now.AddDays(-6), 3800), new(DateTime.Now.AddDays(-5), 1200), new(DateTime.Now.AddDays(-4), 1500),
                new(DateTime.Now.AddDays(-3), 800),  new(DateTime.Now.AddDays(-2), 2000),  new(DateTime.Now.AddDays(-1), 3500),
                 new(DateTime.Now, 2500)
           };
            MarketDepthBids = new ObservableCollection<DepthItem>(Enumerable.Repeat(new DepthItem { Value = 200 }, 10));
            MarketDepthOffers = new ObservableCollection<DepthItem>(Enumerable.Repeat(new DepthItem { Value = 300 }, 10));
        }

        [RelayCommand]
        private void Back()
        {
            Shell.Current.GoToAsync("..");
        }

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

    public class PricePoint
    {
        public DateTime Date { get; set; }
        public double Price { get; set; }

        public PricePoint(DateTime date, double price)
        {
            Date = date;
            Price = price;
        }
    }

    public partial class DepthItem
    {
        public double Value { get; set; }

       
    }
}
