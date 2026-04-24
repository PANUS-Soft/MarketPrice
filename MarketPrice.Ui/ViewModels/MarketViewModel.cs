using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketPrice.Ui.Common;
using MarketPrice.Ui.Models;
using MarketPrice.Ui.Views;
using MarketPrice.Domain.Market.DTOs;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net.Http.Json;
using MarketPrice.Domain.Position.Commands;
using MarketPrice.Domain.Reference.DTOs;
using MarketPrice.Ui.Services.Api;
using MarketPrice.Ui.Services.Session;
using Microsoft.Extensions.Options;

namespace MarketPrice.Ui.ViewModels
{
    public partial class MarketViewModel : ObservableObject
    {
        private readonly ReferenceDataApiService _referenceDataApi;
        private readonly MarketApiService _marketApi;
        private readonly ApiSettings _apiSettingOptions;

        public readonly List<MarketItem> _allMarketItems = new();
        public ObservableCollection<string> CommodityTypesList { get; } = new();
        public ObservableCollection<MarketItem> MarketItems { get; } = new();

        [ObservableProperty] private ImageSource previewImage;
        [ObservableProperty] private bool isImagePreviewVisible;
        [ObservableProperty] private string? selectedCommodityTypeName;

        [ObservableProperty] private string selectedCommodityType = "ALL";
        [ObservableProperty] private string searchText = string.Empty;
        [ObservableProperty] private bool isListEmpty;
        [ObservableProperty] private bool isLoading;

        public MarketViewModel(ReferenceDataApiService referenceDataApi, MarketApiService marketApi, IOptions<ApiSettings> apiSettingOptions)
        {
            _referenceDataApi = referenceDataApi;
            _marketApi = marketApi;
            _apiSettingOptions = apiSettingOptions.Value;

            _ = InitializeAsync();
        }

        public MarketViewModel()
        {
        }

        public async Task InitializeAsync()
        {
            IsLoading = true;
            try
            {
                await LoadCommodityTypesAsync();
                await LoadMarketAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void Search() => ApplyFilters();

        public async Task LoadCommodityTypesAsync()
        {
            var commodityTypesResponse = await _referenceDataApi.GetCommodityTypesAsync();

            if (!commodityTypesResponse.IsSuccessStatusCode) return;

            var commodityTypes = await commodityTypesResponse.Content.ReadFromJsonAsync<List<CommodityTypeDto>>();

            if (commodityTypes == null) return;

            CommodityTypesList.Clear();
            CommodityTypesList.Add("ALL");

            foreach (var type in commodityTypes.OrderBy(t => t.Name, StringComparer.OrdinalIgnoreCase)) CommodityTypesList.Add(type.Name!.ToUpper());

            SelectedCommodityType = "ALL";
        }

        public async Task LoadMarketAsync()
        {
            var marketInsightReponse = await _marketApi.LoadMarketAsync();

            if (!marketInsightReponse.IsSuccessStatusCode) return;

            var marketInsights = await marketInsightReponse.Content.ReadFromJsonAsync<List<MarketResponseDto>>();

            if (marketInsights == null) return;

            _allMarketItems.Clear();

            var culture = new CultureInfo("en-CM");

            foreach (var insight in marketInsights)
            {
                var bid0 = insight.BidDepth.ElementAtOrDefault(0);
                var bid1 = insight.BidDepth.ElementAtOrDefault(1);
                var bid2 = insight.BidDepth.ElementAtOrDefault(2);
                var offer0 = insight.OfferDepth.ElementAtOrDefault(0);
                var offer1 = insight.OfferDepth.ElementAtOrDefault(1);
                var offer2 = insight.OfferDepth.ElementAtOrDefault(2);

                var hasBid = bid0 != null && bid0.Price != 0;
                var hasOffer = offer0 != null && offer0.Price != 0;

                _allMarketItems.Add(new MarketItem()
                {
                    Filter = new MarketItemFilter()
                    {
                        CommodityTypeId = insight.CommodityTypeId,
                        CommodityId = insight.CommodityId,
                        Name = insight.CommodityName!
                    },
                    ImageSource = ImageSource.FromUri(new Uri($"{_apiSettingOptions.BaseUrl}{insight.ImageUrl}")),
                    LotSize = insight.LotSize,
                    UnitOfMeasure = insight.UnitOfMeasure,
                    LotSizeDisplay = $"{insight.LotSize} {insight.UnitOfMeasure}",

                    BestBid = hasBid ? bid0!.Price.ToString("No", culture) : "No Bids",
                    BestBidRaw = bid0?.Price ?? 0,
                    BestBidQuantity = bid0?.TotalActivePosforPrice ?? 0,
                    BestBidLocation = bid0?.Locations.FirstOrDefault() ?? string.Empty,
                    NextBid1 = bid1?.Price ?? 0,
                    NextBid2 = bid2?.Price ?? 0,

                    BestOffer = hasOffer ? offer0!.Price.ToString("No", culture) : "No Offers",
                    BestOfferRaw = offer0?.Price ?? 0,
                    BestOfferQuantity = offer0?.TotalActivePosforPrice ?? 0,
                    BestOfferLocation = offer0?.Locations.FirstOrDefault() ?? string.Empty,
                    NextOffer1 = offer1?.Price ?? 0,
                    NextOffer2 = offer2?.Price ?? 0,

                    IsBidUp = insight.IsBidImproved && hasBid,
                    IsBidDown = insight.IsBidImproved && hasBid,
                    IsBidNull = !hasBid,
                    IsOfferUp = insight.IsOfferImproved && hasOffer,
                    IsOfferDown = insight.IsOfferImproved && hasOffer,
                    IsOfferNull = !hasOffer,

                    IsBidSoonToExpire = insight.IsBestBidSoonToExpire,
                    IsOfferSoonToExpire = insight.IsBestOfferSoonToExpire,
                });
            }

            ApplyFilters();
        }

        partial void OnSelectedCommodityTypeChanged(string value) => ApplyFilters();
        partial void OnSearchTextChanged(string value) => ApplyFilters();

        private void ApplyFilters()
        {
            IEnumerable<MarketItem> filtered = _allMarketItems;

            if (!string.IsNullOrEmpty(SelectedCommodityType) && SelectedCommodityType != "ALL")
            {
                filtered = filtered.Where(item => item.Filter.Name != null && item.Filter.Name.Contains(SelectedCommodityType, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(item => item.Filter.Name != null && item.Filter.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            filtered = filtered.OrderBy(item => item.Filter.Name, StringComparer.OrdinalIgnoreCase);

            MarketItems.Clear();

            foreach (var item in filtered) MarketItems.Add(item);

            IsListEmpty = !IsLoading && MarketItems.Count == 0;
        }

        [RelayCommand]
        private async Task NavigateToMarketInsightAsync(MarketItemFilter? selectedItem)
        {
            if (selectedItem == null) return;

            await Shell.Current.GoToAsync("MarketInsight", new Dictionary<string, object>()
            {
                {"SelectedMarketItemFilter", selectedItem }
            });
        }

        [RelayCommand]
        private async Task NavigateToBidPositionListing(MarketItem item)
        {
            if (item.BestBid == "No Bids") return;

            var args = new PositionListingCommand
            {
                CommodityTypeId = item.Filter.CommodityTypeId,
                CommodityId = item.Filter.CommodityId,
                PositionTypeId = 6001,
                UnitPrice = decimal.Parse(item.BestBid!),
                CommodityName = item.Filter.Name
            };

            await Shell.Current.GoToAsync(nameof(PositionListing), new Dictionary<string, object>
            {
                { "Args", args }
            });
        }
        
        [RelayCommand]
        private async Task NavigateToOfferPositionListing(MarketItem item)
        {
            if (item.BestOffer == "No Offers") return;

            var args = new PositionListingCommand
            {
                CommodityTypeId = item.Filter.CommodityTypeId,
                CommodityId = item.Filter.CommodityId,
                PositionTypeId = 6002,
                UnitPrice = decimal.Parse(item.BestOffer!),
                CommodityName = item.Filter.Name
            };

            await Shell.Current.GoToAsync(nameof(PositionListing), new Dictionary<string, object>
            {
                { "Args", args }
            });
        }

        [RelayCommand]
        private void OpenImagePreview(MarketItem item)
        {
            if (item == null) return;

            PreviewImage = item.ImageSource;
            SelectedCommodityTypeName = item.Filter.Name.ToUpper();
            IsImagePreviewVisible = true;
        }

        [RelayCommand]
        private void CloseImagePreview()
        {
            IsImagePreviewVisible = false;
            PreviewImage = null;
            SelectedCommodityTypeName = string.Empty;
        }

       
    }
}