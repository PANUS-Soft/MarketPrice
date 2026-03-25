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
        private readonly ReferenceDataApiService _referenceDataApi;
        private readonly MarketApiService _marketApi;

        [ObservableProperty] MarketItem? selectedMarketItem;

        public ObservableCollection<MarketItem> Commodities { get; } = new();

        [ObservableProperty] private MarketInsightResponseDto? dto;
        public ObservableCollection<MarketInsightChartResponseDto>? PriceHistory { get; } = new();

        public MarketInsightViewModel(ReferenceDataApiService referenceDataApiService, MarketApiService marketApiService)
        {
            _referenceDataApi = referenceDataApiService;
            _marketApi = marketApiService;
        }

        partial void OnSelectedMarketItemChanged(MarketItem? value)
        {
            if (value != null) _ = GetCommodityMarketInsightAsync(value.CommodityId);
            if (value != null) _ = LoadApiDataAsync(value.CommodityId);
            if (value != null) _ = LoadApiChartDataAsync(value.CommodityId);
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

        //partial void OnSelectedMarketItemChanged(MarketItem? value)
        //{
        //    if (value != null)
        //        _ = LoadApiDataAsync(value.CommodityId);
        //}

        private async Task LoadApiChartDataAsync(Guid commodityId)
        {
            try
            {
                var response = await _marketApi.GetChartDataAsync(commodityId);
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
                        OnPropertyChanged(nameof(PriceHistory));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"API Error: {ex.Message}");
            }
        }

        private async Task LoadApiDataAsync(Guid commodityId)
        {
            try
            {
                var response = await _marketApi.GetMarketInsightAsync(commodityId);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<MarketInsightResponseDto>();
                    if (result != null)
                    {
                        selectedMarketItem.HighBid = result.MaxBid24H;
                        selectedMarketItem.HighOffer = result.MaxOffer24H;
                        selectedMarketItem.LowBid = result.MinBid24H;
                        selectedMarketItem.LowOffer = result.MinOffer24H;
                    }
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"API Error: {ex.Message}");
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
    }

    public partial class MarketDepthItem
    {
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }

    }
}
