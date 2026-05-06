using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketPrice.Ui.Common;
using MarketPrice.Ui.Models;
using MarketPrice.Ui.Views;
using MarketPrice.Ui.Services.Api;
using MarketPrice.Domain.Market.DTOs;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using MarketPrice.Domain.Position.Commands;
using MarketPrice.Domain.Reference.DTOs;

namespace MarketPrice.Ui.ViewModels
{
    [QueryProperty(nameof(SelectedMarketItemFilter), "SelectedMarketItemFilter")]
    public partial class MarketInsightViewModel : ObservableObject
    {
        private readonly ReferenceDataApiService _referenceDataApi;
        private readonly MarketApiService _marketApi;

        [ObservableProperty] MarketItemFilter? selectedMarketItemFilter;
        private MarketItemFilter? _incomingMarketItemFilter;

        public ObservableCollection<MarketItemFilter> Commodities { get; } = new();

        [ObservableProperty] private MarketInsightResponseDto? dto;
        public ObservableCollection<MarketInsightChartResponseDto>? PriceHistory { get; } = new();

        public MarketInsightViewModel(ReferenceDataApiService referenceDataApiService, MarketApiService marketApiService)
        {
            _referenceDataApi = referenceDataApiService;
            _marketApi = marketApiService;
        }

        public async Task InitializeAsync()
        {
            await LoadCommoditiesFilterAsync();

            ApplySelectedCommodity();
        }

        partial void OnSelectedMarketItemFilterChanged(MarketItemFilter? value)
        {
            if (value == null) return;

            _incomingMarketItemFilter = value;

            if (Commodities.Count > 0) ApplySelectedCommodity();
        }

        private void ApplySelectedCommodity()
        {
            if (_incomingMarketItemFilter == null) return;

            SelectedMarketItemFilter =
                Commodities.FirstOrDefault(c => c.CommodityId == _incomingMarketItemFilter.CommodityId);

            if (SelectedMarketItemFilter != null) _ = LoadMarketInsightAsync(SelectedMarketItemFilter.CommodityId);
        }

        //partial void OnSelectedMarketItemFilterChanged(MarketItemFilter? value)
        //{
        //    if (value != null) _ = LoadMarketInsightAsync(value.CommodityId);
        //}

        private async Task LoadMarketInsightAsync(Guid commodityId)
        {
            await Task.WhenAll(GetCommodityMarketInsightAsync(commodityId), LoadChartDataAsync(commodityId));
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

        private async Task LoadChartDataAsync(Guid commodityId)
        {
            try
            {
                //var response = await _marketApi.GetChartDataAsync(commodityId);
                var response = await _marketApi.GetChartDataAsync(commodityId, "1W");
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

        private async Task LoadCommoditiesFilterAsync()
        {
            try
            {
                var response = await _referenceDataApi.GetCommoditiesAsync();
                if (response.IsSuccessStatusCode)
                {
                    var commodities = await response.Content.ReadFromJsonAsync<List<CommodityDto>>();
                    Commodities.Clear();
                    foreach (var commodity in commodities!)
                    {
                        Commodities.Add(new MarketItemFilter
                        {
                            CommodityTypeId = commodity.CommodityTypeId,
                            CommodityId = commodity.Id,
                            Name = commodity.Name.ToUpper()
                        });
                    }
                }
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error", $"Something went wrong while loading commodities. {e.Message} Please try again later.", "OK");
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

        [RelayCommand]
        private async Task NavigateToBidPositionListingAsync(MarketDepthItemDto item)
        {
            var args = new PositionListingCommand
            {
                CommodityTypeId = Dto!.CommodityTypeId,
                CommodityId = Dto?.CommodityId,
                CommodityName = Dto?.CommodityName,
                PositionTypeId = 6001,
                UnitPrice = item.Price

            };

            await Shell.Current.GoToAsync(nameof(PositionListing), new Dictionary<string, object>
            {
                { "Args", args }
            });
        }

        [RelayCommand]
        private async Task NavigateToOfferPositionListingAsync(MarketDepthItemDto item)
        {
            var args = new PositionListingCommand
            {
                CommodityTypeId = Dto!.CommodityTypeId,
                CommodityId = Dto?.CommodityId,
                CommodityName = Dto?.CommodityName,
                PositionTypeId = 6002,
                UnitPrice = item.Price
            };

            await Shell.Current.GoToAsync(nameof(PositionListing), new Dictionary<string, object>
            {
                { "Args", args }
            });
        }

    }

    public partial class MarketDepthItem
    {
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }

    }
}
