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
using System.Globalization;

namespace MarketPrice.Ui.ViewModels
{
    /// <summary>
    /// ViewModel for the Market Insight Screen.
    /// Handles market depth, price history charts, and trade placement navigation.
    /// </summary>
    [QueryProperty(nameof(SelectedMarketItemFilter), "SelectedMarketItemFilter")]
    public partial class MarketInsightViewModel : ObservableObject
    {
        #region Fields & Dependencies
        private readonly ReferenceDataApiService _referenceDataApi;
        private readonly MarketApiService _marketApi;
        private readonly CultureInfo _cameroonCulture = new CultureInfo("en-CM");
        #endregion

        #region Parameters & Collections
        [ObservableProperty] MarketItemFilter? selectedMarketItemFilter;
        private MarketItemFilter? _incomingMarketItemFilter;

        // List used for filtering/selecting different commodities
        public ObservableCollection<MarketItemFilter> Commodities { get; } = new();

        // Data Transfer Object holding the core market data from API
        [ObservableProperty] private MarketInsightResponseDto? dto;

        // Collections bound to UI Lists and Charts
        public ObservableCollection<MarketInsightChartResponseDto> PriceHistory { get; } = new();
        public ObservableCollection<MarketDepthItemDto> TopBids { get; } = new();
        public ObservableCollection<MarketDepthItemDto> TopOffers { get; } = new();
        #endregion

        #region UI Properties (Bound to XAML Labels)
        [ObservableProperty] private string _commodityName = "---";
        [ObservableProperty] private string _lotSizeDisplay = "Lot Size = ---";
        [ObservableProperty] private string _shelfLifeDisplay = "Shelf Life: ---";

        [ObservableProperty] private decimal _bestBid;
        [ObservableProperty] private decimal _bestOffer;
        [ObservableProperty] private int _bidCount;
        [ObservableProperty] private int _offerCount;

        [ObservableProperty] private decimal _maxBid24H;
        [ObservableProperty] private decimal _minBid24H;
        [ObservableProperty] private decimal _maxOffer24H;
        [ObservableProperty] private decimal _minOffer24H;

        // Percentages for the "Sentiment Bar" (Green/Blue bar)
        [ObservableProperty] private double _bidPercentage;
        [ObservableProperty] private double _offerPercentage;
        #endregion

        public MarketInsightViewModel(ReferenceDataApiService referenceDataApiService, MarketApiService marketApiService)
        {
            _referenceDataApi = referenceDataApiService;
            _marketApi = marketApiService;

            // Load visual placeholders immediately so the screen isn't empty on launch
            LoadMockData();
        }

        #region Initialization Logic
        /// <summary>
        /// Called when the page is navigated to. 
        /// Fetches commodity lists and triggers initial data load.
        /// </summary>
        public async Task InitializeAsync()
        {
            await LoadCommoditiesFilterAsync();
            ApplySelectedCommodity();
        }

        /// <summary>
        /// Triggered automatically when 'SelectedMarketItemFilter' is passed via navigation query.
        /// </summary>
        partial void OnSelectedMarketItemFilterChanged(MarketItemFilter? value)
        {
            if (value == null) return;
            _incomingMarketItemFilter = value;
            if (Commodities.Count > 0) ApplySelectedCommodity();
        }

        private void ApplySelectedCommodity()
        {
            if (_incomingMarketItemFilter == null) return;

            // Sync the local selection with the incoming filter parameter
            SelectedMarketItemFilter = Commodities.FirstOrDefault(c => c.CommodityId == _incomingMarketItemFilter.CommodityId);

            if (SelectedMarketItemFilter != null)
                _ = LoadMarketInsightAsync(SelectedMarketItemFilter.CommodityId);
        }
        #endregion

        #region Data Loading (API)
        private async Task LoadMarketInsightAsync(Guid commodityId)
        {
            // Run both API calls in parallel for better performance
            await Task.WhenAll(GetCommodityMarketInsightAsync(commodityId), LoadChartDataAsync(commodityId));
        }

        private async Task GetCommodityMarketInsightAsync(Guid id)
        {
            try
            {
                var response = await _marketApi.GetCommodityMarketInsightAsync(id);
                if (response.IsSuccessStatusCode)
                {
                    Dto = await response.Content.ReadFromJsonAsync<MarketInsightResponseDto>();
                    if (Dto != null) UpdateUiFromDto(Dto);
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"API Error: {ex.Message}"); }
        }

        /// <summary>
        /// Maps the raw DTO data to individual properties that the UI can easily display.
        /// </summary>
        private void UpdateUiFromDto(MarketInsightResponseDto data)
        {
            CommodityName = data.CommodityName?.ToUpper() ?? "---";
            BestBid = data.BestBid;
            BestOffer = data.BestOffer;
            MaxBid24H = data.MaxBid24H;
            MinBid24H = data.MinBid24H;
            MaxOffer24H = data.MaxOffer24H;
            MinOffer24H = data.MinOffer24H;

            TopBids.Clear();
            foreach (var b in data.Bids) TopBids.Add(b);

            TopOffers.Clear();
            foreach (var o in data.Offers) TopOffers.Add(o);

            CalculateSentiment();
        }

        /// <summary>
        /// Calculates the ratio of Bidders vs Offerors to drive the UI Sentiment Bar.
        /// </summary>
        private void CalculateSentiment()
        {
            double totalBids = (double)TopBids.Sum(x => x.Quantity);
            double totalOffers = (double)TopOffers.Sum(x => x.Quantity);
            double total = totalBids + totalOffers;

            if (total > 0)
            {
                BidPercentage = (totalBids / total) * 100;
                OfferPercentage = (totalOffers / total) * 100;
            }
            else
            {
                BidPercentage = 50; OfferPercentage = 50; // Default split if no volume
            }
        }

        private async Task LoadChartDataAsync(Guid commodityId)
        {
            try
            {
                var response = await _marketApi.GetChartDataAsync(commodityId, "1W");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<MarketChartDataWrapper>();
                    if (result?.Data != null)
                    {
                        PriceHistory.Clear();
                        foreach (var point in result.Data) PriceHistory.Add(point);
                    }
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Chart Error: {ex.Message}"); }
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
                    if (commodities != null)
                    {
                        foreach (var c in commodities)
                            Commodities.Add(new MarketItemFilter { CommodityId = c.Id, Name = c.Name.ToUpper() });
                    }
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Filter Error: {ex.Message}"); }
        }
        #endregion

        #region Mockup Data (Visual Testing)
        /// <summary>
        /// Populates the UI with hardcoded data for Douala market testing.
        /// Useful when the API is down or in development.
        /// </summary>
        private void LoadMockData()
        {
            CommodityName = "CORN";
            LotSizeDisplay = "Lot Size = 100kg";
            ShelfLifeDisplay = "Shelf Life: 12 Months";
            BestBid = 24500; BestOffer = 25200;
            MaxBid24H = 26000; MinBid24H = 23500;

            TopBids.Clear();
            TopBids.Add(new MarketDepthItemDto { Quantity = 120, Price = 24500 });
            TopBids.Add(new MarketDepthItemDto { Quantity = 85, Price = 24300 });

            TopOffers.Clear();
            TopOffers.Add(new MarketDepthItemDto { Quantity = 95, Price = 25200 });
            TopOffers.Add(new MarketDepthItemDto { Quantity = 150, Price = 25400 });

            PriceHistory.Clear();
            PriceHistory.Add(new MarketInsightChartResponseDto { Timestamp = DateTime.Now.AddDays(-2), AvgBid = 23500, AvgOffer = 24800 });
            PriceHistory.Add(new MarketInsightChartResponseDto { Timestamp = DateTime.Now.AddDays(-1), AvgBid = 24000, AvgOffer = 25000 });

            CalculateSentiment();
        }
        #endregion

        #region Commands (Navigation)
        [RelayCommand]
        private async Task BackAsync() => await Shell.Current.GoToAsync("..");

        [RelayCommand]
        private async Task NavigateToPlaceBidAsync() =>
            await Shell.Current.GoToAsync(nameof(PlacePosition), new Dictionary<string, object> { [NavigationKeys.PositionType] = PositionType.Bid });

        [RelayCommand]
        private async Task NavigateToPlaceOfferAsync() =>
            await Shell.Current.GoToAsync(nameof(PlacePosition), new Dictionary<string, object> { [NavigationKeys.PositionType] = PositionType.Offer });
        #endregion
    }
}