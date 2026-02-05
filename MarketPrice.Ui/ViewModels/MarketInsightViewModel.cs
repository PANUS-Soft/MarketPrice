using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketPrice.Ui.Common;
using MarketPrice.Ui.Models;
using MarketPrice.Ui.Views;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using MarketPrice.Ui.Services.Api;
using MarketPrice.Domain.Market.DTOs;

namespace MarketPrice.Ui.ViewModels
{
    [QueryProperty(nameof(SelectedMarketItem), "SelectedMarketItem")]
    public partial class MarketInsightViewModel (ReferenceDataApiService referenceDataApi, MarketApiService marketApi) : ObservableObject
    {
        private readonly ReferenceDataApiService _referenceDataApi = referenceDataApi;
        private readonly MarketApiService _marketApi = marketApi;

        [ObservableProperty] MarketItem selectedMarketItem;
        [ObservableProperty] private MarketInsightResponseDto? dto;
        //[ObservableProperty] private List<MarketDepthItem> bidMarketDepth;
        //[ObservableProperty] private List<MarketDepthItem> offerMarketDepth;

        partial void OnSelectedMarketItemChanged(MarketItem? value)
        {
            if (value != null) _ = GetCommodityMarketInsightAsync(value.CommodityId);
        }

        public string CommodityName => Dto?.CommodityName.ToUpper() ?? "---";
        public string BestBid => Dto?.BestBid.ToString("N0", new System.Globalization.CultureInfo("en-CM")) ?? "---";
        public string BestOffer => Dto?.BestOffer.ToString("N0", new System.Globalization.CultureInfo("en-CM")) ?? "---";
        public string MaxBid24H => Dto?.MaxBid24H.ToString("N0", new System.Globalization.CultureInfo("en-CM")) ?? "---";
        public string MinBid24H => Dto?.MinBid24H.ToString("N0", new System.Globalization.CultureInfo("en-CM")) ?? "---";
        public string MaxOffer24H => Dto?.MaxOffer24H.ToString("N0", new System.Globalization.CultureInfo("en-CM")) ?? "---";
        public string MinOffer24H => Dto?.MinOffer24H.ToString("N0", new System.Globalization.CultureInfo("en-CM")) ?? "---";
        public List<MarketDepthItemDto> Bids => Dto?.Bids ?? new List<MarketDepthItemDto>();
        public List<MarketDepthItemDto> Offers => Dto?.Offers ?? new List<MarketDepthItemDto>();

        private async Task GetCommodityMarketInsightAsync(Guid id)
        {
            try
            {
                var marketInsightResponse = await _marketApi.GetCommodityMarketInsightAsync(id);

                if (!marketInsightResponse.IsSuccessStatusCode) return;

                Dto = await marketInsightResponse.Content.ReadFromJsonAsync<MarketInsightResponseDto>();

                OnPropertyChanged(string.Empty);
            }
            catch(Exception e)
            {
                await Shell.Current.DisplayAlert("Error", $"Something went wrong while loading market insight. {e.Message} Please try again later.", "OK");
            }
        }


        [RelayCommand]
        private async Task BackAsync()
        {
            await Shell.Current.GoToAsync("..");
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

        //public ObservableCollection<PricePoint>? PriceHistory { get; set; }

        //public ObservableCollection<DepthItem>? MarketDepthBids { get; set; }
        //public ObservableCollection<DepthItem>? MarketDepthOffers { get; set; }
        //public MarketInsightViewModel()
        //{
        //    PriceHistory = new ObservableCollection<PricePoint>()
        //    {
        //        new(DateTime.Now.AddDays(-6), 3800), new(DateTime.Now.AddDays(-5), 1200), new(DateTime.Now.AddDays(-4), 1500),
        //        new(DateTime.Now.AddDays(-3), 800),  new(DateTime.Now.AddDays(-2), 2000),  new(DateTime.Now.AddDays(-1), 3500),
        //        new(DateTime.Now, 2500)
        //    };
        //    MarketDepthBids = new ObservableCollection<DepthItem>(Enumerable.Repeat(new DepthItem { Value = 200 }, 10));
        //    MarketDepthOffers = new ObservableCollection<DepthItem>(Enumerable.Repeat(new DepthItem { Value = 300 }, 10));
        //}
    }

    //public class PricePoint
    //{
    //    public DateTime Date { get; set; }
    //    public double Price { get; set; }

    //    public PricePoint(DateTime date, double price)
    //    {
    //        Date = date;
    //        Price = price;
    //    }
    //}


    public partial class MarketDepthItem
    {
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }

    }
}
