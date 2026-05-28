using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketPrice.Domain.Market.DTOs;
using MarketPrice.Domain.Position.Commands;
using MarketPrice.Domain.Reference.DTOs;
using MarketPrice.Ui.Common;
using MarketPrice.Ui.Models;
using MarketPrice.Ui.Services.Api;
using MarketPrice.Ui.Views;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using MarketPrice.Domain.Position.Commands;
using MarketPrice.Domain.Reference.DTOs;
using System.Globalization;

namespace MarketPrice.Ui.ViewModels
{
    [QueryProperty(nameof(SelectedMarketItem), "SelectedMarketItem")]
    public partial class MarketInsightViewModel : ObservableObject
    {
        #region Services

        private readonly ReferenceDataApiService _referenceDataApi;
        private readonly MarketApiService _marketApi;
        private readonly CultureInfo _cameroonCulture = new("en-CM");

        #endregion

        #region Navigation Parameters

        // Commodity passed from Market page
        [ObservableProperty]
        private MarketItem? selectedMarketItem;

        #endregion

        #region Collections

        // Commodity filter list
        public ObservableCollection<MarketItemFilter> Commodities { get; } = new();

        // Stores market insight API response
        [ObservableProperty]
        private MarketInsightResponseDto? dto;

        // Chart history data
        public ObservableCollection<MarketInsightChartResponseDto> PriceHistory { get; } = new();

        // Top bid price levels
        public ObservableCollection<MarketDepthItemDto> TopBids { get; } = new();

        // Top offer price levels
        public ObservableCollection<MarketDepthItemDto> TopOffers { get; } = new();

        #endregion

        #region Header Properties

        [ObservableProperty]
        private string commodityName = "---";

        [ObservableProperty]
        private string lotSizeDisplay = "Lot Size = ---";

        [ObservableProperty]
        private string shelfLifeDisplay = "Shelf Life: ---";

        [ObservableProperty]
        private ImageSource? commodityImage;

        #endregion

        #region Market Summary Properties

        // Best current bid price
        [ObservableProperty]
        private decimal bestBid;

        // Best current offer price
        [ObservableProperty]
        private decimal bestOffer;

        // Number of positions at best bid
        [ObservableProperty]
        private int bestBidPositionCount;

        // Number of positions at best offer
        [ObservableProperty]
        private int bestOfferPositionCount;

        // Highest bid in last 24 hours
        [ObservableProperty]
        private decimal maxBid24H;

        // Lowest bid in last 24 hours
        [ObservableProperty]
        private decimal minBid24H;

        // Highest offer in last 24 hours
        [ObservableProperty]
        private decimal maxOffer24H;

        // Lowest offer in last 24 hours
        [ObservableProperty]
        private decimal minOffer24H;

        #endregion

        #region Market Sentiment

        // Percentage of total bid volume
        [ObservableProperty]
        private double bidPercentage;

        // Percentage of total offer volume
        [ObservableProperty]
        private double offerPercentage;

        #endregion

        public MarketInsightViewModel(
            ReferenceDataApiService referenceDataApiService,
            MarketApiService marketApiService)
        {
            _referenceDataApi = referenceDataApiService;
            _marketApi = marketApiService;

            ChangeRangeCommand = new Command<string>(async (range) =>
            {
                SelectedRange = range;

                await LoadChartDataAsync(CurrentCommodityId);
            });
        }

        #region Initialization

        // Loads initial commodity filter list
        public async Task InitializeAsync()
        {
            await LoadCommoditiesFilterAsync();
        }

        // Automatically triggered when Market page sends commodity data
        partial void OnSelectedMarketItemChanged(MarketItem? value)
        {
            if (value == null)
                return;

            CommodityName = value.Name?.ToUpper() ?? "---";

            LotSizeDisplay = !string.IsNullOrWhiteSpace(value.LotSizeDisplay)
                ? $"Lot Size = {value.LotSizeDisplay}"
                : "Lot Size = ---";

            ShelfLifeDisplay = !string.IsNullOrWhiteSpace(value.ShelfLife)
                ? $"Shelf Life: {value.ShelfLife}"
                : "Shelf Life: ---";

            CommodityImage = value.ImageSource;

            // Temporary until backend sends separate bid/offer values
            BestBid = value.CurrentPrice;
            BestOffer = value.CurrentPrice;

            // Temporary mock counts
            BestBidPositionCount = 8;
            BestOfferPositionCount = 6;

            // Temporary mock bid levels
            TopBids.Clear();

            TopBids.Add(new MarketDepthItemDto
            {
                Quantity = 120,
                Price = value.CurrentPrice,
                PositionCount = 8
            });

            TopBids.Add(new MarketDepthItemDto
            {
                Quantity = 85,
                Price = value.CurrentPrice - 200,
                PositionCount = 4
            });

            // Temporary mock offer levels
            TopOffers.Clear();

            TopOffers.Add(new MarketDepthItemDto
            {
                Quantity = 95,
                Price = value.CurrentPrice + 200,
                PositionCount = 6
            });

            TopOffers.Add(new MarketDepthItemDto
            {
                Quantity = 150,
                Price = value.CurrentPrice + 400,
                PositionCount = 10
            });

            // Temporary chart mock
            PriceHistory.Clear();

            PriceHistory.Add(new MarketInsightChartResponseDto
            {
                Timestamp = DateTime.Now.AddDays(-2),
                AvgBid = value.CurrentPrice - 500,
                AvgOffer = value.CurrentPrice
            });

            PriceHistory.Add(new MarketInsightChartResponseDto
            {
                Timestamp = DateTime.Now.AddDays(-1),
                AvgBid = value.CurrentPrice - 200,
                AvgOffer = value.CurrentPrice + 300
            });

            CalculateSentiment();

            // Real API call
            _ = LoadMarketInsightAsync(value.CommodityId);
        }

        #endregion

        #region API Calls

        // Loads market insight summary and chart data together
        private async Task LoadMarketInsightAsync(Guid commodityId)
        {
            await Task.WhenAll(
                GetCommodityMarketInsightAsync(commodityId),
                LoadChartDataAsync(commodityId));
            StartAutoRefresh();
        }

        // Gets market depth and price summary
        private async Task GetCommodityMarketInsightAsync(Guid id)
        {
            try
            {
                var response = await _marketApi.GetCommodityMarketInsightAsync(id);

                if (response.IsSuccessStatusCode)
                {
                    Dto = await response.Content.ReadFromJsonAsync<MarketInsightResponseDto>();

                    if (Dto != null)
                    {
                        UpdateUiFromDto(Dto);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"API Error: {ex.Message}");
            }
        }

        // Maps API response into UI properties
        private void UpdateUiFromDto(MarketInsightResponseDto data)
        {
            CommodityName = data.CommodityName?.ToUpper() ?? "---";

            BestBid = data.BestBid;
            BestOffer = data.BestOffer;

            BestBidPositionCount = data.Bids?
                .FirstOrDefault(x => x.Price == data.BestBid)?
                .PositionCount ?? 0;

            BestOfferPositionCount = data.Offers?
                .FirstOrDefault(x => x.Price == data.BestOffer)?
                .PositionCount ?? 0;

            MaxBid24H = data.MaxBid24H;
            MinBid24H = data.MinBid24H;

            MaxOffer24H = data.MaxOffer24H;
            MinOffer24H = data.MinOffer24H;

            TopBids.Clear();

            foreach (var bid in data.Bids)
            {
                TopBids.Add(bid);
            }

            TopOffers.Clear();

            foreach (var offer in data.Offers)
            {
                TopOffers.Add(offer);
            }

            CalculateSentiment();
        }

        // Loads chart history data
        private async Task LoadChartDataAsync(Guid commodityId)
        {
            try
            {
                var response = await _marketApi.GetChartDataAsync(commodityId, "1W");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content
                        .ReadFromJsonAsync<MarketChartDataWrapper>();

                    if (result?.Data != null)
                    {
                        PriceHistory.Clear();

                        foreach (var point in result.Data)
                        {
                            PriceHistory.Add(point);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Chart Error: {ex.Message}");
            }
            finally
                {
                    IsLoading = false;
            }
        }

        // Method used to update the TimeAxis format based on the selected range. This is to ensure that the graph remains readable and appropriately formatted for different time ranges.

        private void UpdateAxisFormat()
        {
            AxisFormat = SelectedRange switch
            {
                "1D" => "HH:mm",
                "1W" => "ddd",
                "1M" => "dd MMM",
                "1Y" => "MMM yyyy",
                _ => "dd/MM"
            };
        }

        // Loads commodity filter dropdown data
        private async Task LoadCommoditiesFilterAsync()
        {
            try
            {
                var response = await _referenceDataApi.GetCommoditiesAsync();

                if (response.IsSuccessStatusCode)
                {
                    var commodities =
                        await response.Content.ReadFromJsonAsync<List<CommodityDto>>();

                    Commodities.Clear();

                    if (commodities != null)
                    {
                        foreach (var commodity in commodities)
                        {
                            Commodities.Add(new MarketItemFilter
                            {
                                CommodityId = commodity.Id,
                                Name = commodity.Name.ToUpper()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Filter Error: {ex.Message}");
            }
        }

        #endregion

        #region Calculations

        // Calculates market sentiment percentages
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
                BidPercentage = 50;
                OfferPercentage = 50;
            }
        }

        #endregion

        #region Navigation Commands

        // Opens position listing for selected price level
        [RelayCommand]
        private async Task NavigateToPositionListingAsync(MarketDepthItemDto item)
        {
            if (item == null)
                return;

            bool isBid = TopBids.Contains(item);

            var args = new PositionListingCommand
            {
                CommodityId = SelectedMarketItem?.CommodityId ?? Guid.Empty,
                PositionTypeId = isBid ? 6001 : 6002,
                UnitPrice = item.Price,
                CommodityName = CommodityName
            };

            await Shell.Current.GoToAsync(
                "PositionListing",
                new Dictionary<string, object>
                {
                    { "Args", args },
                    { "PassedImage", CommodityImage },
                    { "PassedCommodityName", CommodityName },
                    { "PassedLotSize", LotSizeDisplay },
                    { "PassedBid", BestBid.ToString("N0") },
                    { "PassedOffer", BestOffer.ToString("N0") }
                });
        }

        // Returns to previous page
        [RelayCommand]
        private async Task BackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        // Opens place bid page
        [RelayCommand]
        private async Task NavigateToPlaceBidAsync()
        {
            await Shell.Current.GoToAsync(
                nameof(PlacePosition),
                new Dictionary<string, object>
                {
                    [NavigationKeys.PositionType] = PositionType.Bid
                });
        }

        // Opens place offer page
        [RelayCommand]
        private async Task NavigateToPlaceOfferAsync()
        {
            await Shell.Current.GoToAsync(
                nameof(PlacePosition),
                new Dictionary<string, object>
                {
                    [NavigationKeys.PositionType] = PositionType.Offer
                });
        }

        #endregion
    }
}