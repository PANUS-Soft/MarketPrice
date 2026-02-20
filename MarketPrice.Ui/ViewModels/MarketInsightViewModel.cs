using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketPrice.Ui.Common;
using MarketPrice.Ui.Models;
using MarketPrice.Ui.Views;
using MarketPrice.Ui.Services.Api;
using MarketPrice.Domain.Market.DTOs;
using System.Collections.ObjectModel;
using System.Net.Http.Json;

namespace MarketPrice.Ui.ViewModels
{
    [QueryProperty(nameof(SelectedMarketItem), "SelectedMarketItem")]
    public partial class MarketInsightViewModel : ObservableObject
    {
        private readonly LoadMarketApiService _marketApiService;

        [ObservableProperty]
        MarketItem? selectedMarketItem;

        public ObservableCollection<MarketInsightChartResponseDto>? PriceHistory { get; } = new();

        public ObservableCollection<DepthItem>? MarketDepthBids { get; set; }
        public ObservableCollection<DepthItem>? MarketDepthOffers { get; set; }

        public MarketInsightViewModel(LoadMarketApiService marketApiService)
        {
            _marketApiService = marketApiService;

            MarketDepthBids = new ObservableCollection<DepthItem>(Enumerable.Repeat(new DepthItem { Value = 200 }, 10));
            MarketDepthOffers = new ObservableCollection<DepthItem>(Enumerable.Repeat(new DepthItem { Value = 300 }, 10));
        }

        partial void OnSelectedMarketItemChanged(MarketItem? value)
        {
            if (value != null)
                _ = LoadApiDataAsync(value.CommodityId);
        }

        private async Task LoadApiDataAsync(Guid commodityId)
        {
            try
            {
                var response = await _marketApiService.GetChartDataAsync(commodityId);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<MarketChartDataWrapper>();

                    if (result?.Data != null)
                    {
                        PriceHistory?.Clear();
                        foreach (var point in result.Data)
                        {
                            PriceHistory?.Add(point);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"API Error: {ex.Message}");
            }
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
