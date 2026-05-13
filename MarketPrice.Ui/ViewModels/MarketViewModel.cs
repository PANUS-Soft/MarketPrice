using System.Collections.ObjectModel;
using System.Net.Http.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using MarketPrice.Domain.Reference.DTOs;
using MarketPrice.Domain.Market.DTOs;
using MarketPrice.Ui.Services.Api;
using MarketPrice.Ui.Common;
using Microsoft.Extensions.Options;
using MarketPrice.Domain.Position.Commands;

namespace MarketPrice.Ui.ViewModels
{
    public partial class MarketViewModel : ObservableObject
    {
        private readonly ReferenceDataApiService _referenceDataApi;
        private readonly MarketApiService _marketApi;
        private readonly ApiSettings _apiSettingOptions;

        // Master list for filtering without hitting the API again
        private readonly List<MarketItem> _allMarketItems = new();

        public ObservableCollection<string> CommodityTypesList { get; } = new();
        public ObservableCollection<MarketItem> MarketItems { get; } = new();

        [ObservableProperty] private string searchText = string.Empty;
        [ObservableProperty] private string selectedCommodityType = "ALL";
        [ObservableProperty] private bool isListEmpty;
        [ObservableProperty] private bool isLoading;

        // Bid/Offer Toggle States (Default to Bid)
        [ObservableProperty] private bool isBidHighlighted = true;
        [ObservableProperty] private bool isOfferHighlighted = false;

        // Image Preview States
        [ObservableProperty] private ImageSource previewImage;
        [ObservableProperty] private bool isImagePreviewVisible;
        [ObservableProperty] private string selectedCommodityTypeName = string.Empty;
        [ObservableProperty] private string selectedCommodityName;


        public MarketViewModel(ReferenceDataApiService referenceDataApi, MarketApiService marketApi, IOptions<ApiSettings> apiSettingOptions)
        {
            _referenceDataApi = referenceDataApi;
            _marketApi = marketApi;
            _apiSettingOptions = apiSettingOptions.Value;

            _ = InitializeAsync();
        }

        public MarketViewModel() { }

        public async Task InitializeAsync()
        {
            IsLoading = true;
            try
            {
                await LoadCommodityTypesAsync();
                await LoadMarketDataAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task LoadCommodityTypesAsync()
        {
            var response = await _referenceDataApi.GetCommodityTypesAsync();
            if (!response.IsSuccessStatusCode) return;

            var types = await response.Content.ReadFromJsonAsync<List<CommodityTypeDto>>();
            if (types == null) return;

            CommodityTypesList.Clear();
            CommodityTypesList.Add("ALL");

            foreach (var t in types.OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase))
            {
                CommodityTypesList.Add(t.Name!.ToUpper());
            }

            SelectedCommodityType = "ALL";
        }

        [RelayCommand]
        private async Task SetSideAsync(string side)
        {
            // 1. Prevent button spamming! If it's already fetching data, ignore the click.
            if (IsLoading) return;

            if (side == "Bid" && IsBidHighlighted) return;
            if (side == "Offer" && IsOfferHighlighted) return;

            IsBidHighlighted = side == "Bid";
            IsOfferHighlighted = side == "Offer";

            await LoadMarketDataAsync();
        }

        public async Task LoadMarketDataAsync()
        {
            IsLoading = true;
            try
            {
                int positionTypeId = IsBidHighlighted ? 6001 : 6002;

                var response = await _marketApi.GetMarketOverviewAsync(positionTypeId);

                if (!response.IsSuccessStatusCode)
                {
                    // Force UI updates onto the Main Thread
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        IsListEmpty = true;
                        MarketItems.Clear();
                    });
                    return;
                }

                var marketdata = await response.Content.ReadFromJsonAsync<List<MarketCommodityDto>>();

                // Safely build the new list on the background thread
                var tempItems = new List<MarketItem>();

                if (marketdata != null)
                {
                    var sortMarketData = marketdata.OrderBy(x => x.CommodityName, StringComparer.OrdinalIgnoreCase).ToList();

                    foreach (var item in sortMarketData)
                    {
                        var imageUrl = !string.IsNullOrWhiteSpace(item.ImageUrl)
                            ? $"{_apiSettingOptions.BaseUrl.TrimEnd('/')}/{item.ImageUrl.TrimStart('/')}"
                            : null;

                        tempItems.Add(new MarketItem
                        {
                            CommodityId = item.CommodityId,
                            Name = item.CommodityName,
                            LotSizeDisplay = item.LotSizeDisplay,
                            CurrentPrice = item.CurrentPrice,

                            FormattedDifference = item.CurrentPrice > 0
                                ? item.FormattedDifference
                                : "-",

                            IsPositiveTrend = item.IsPositiveTrend,

                            DisplayPrice = item.CurrentPrice > 0
                                ? item.CurrentPrice.ToString("N0")
                                : (IsBidHighlighted ? "No Bids" : "No Offers"),

                            ImageSource = imageUrl != null
                                ? ImageSource.FromUri(new Uri(imageUrl))
                                : ImageSource.FromFile("corn_placeholder.png")
                        });
                    }
                }

                // 2. CRITICAL FIX: Push the final data to the UI on the Main Thread
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    _allMarketItems.Clear();
                    _allMarketItems.AddRange(tempItems);
                    ApplyFilters();
                });
            }
            catch (Exception ex)
            {
                // Silently handle any network drops or backend 500 errors so the app doesn't crash
                Console.WriteLine($"Error fetching market data: {ex.Message}");
            }
            finally
            {
                // Ensure the loading spinner/state always turns off, even if it fails
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    IsLoading = false;
                });
            }
        }

        [RelayCommand]
        private void Search() => ApplyFilters();

        partial void OnSearchTextChanged(string value) => ApplyFilters();
        partial void OnSelectedCommodityTypeChanged(string value) => ApplyFilters();

        private void ApplyFilters()
        {
            IEnumerable<MarketItem> filtered = _allMarketItems;

            if (!string.IsNullOrEmpty(SelectedCommodityType) && SelectedCommodityType != "ALL")
            {
                filtered = filtered.Where(i => i.Name.Contains(SelectedCommodityType, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(i => i.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            MarketItems.Clear();
            foreach (var item in filtered.OrderBy(i => i.Name))
            {
                MarketItems.Add(item);
            }

            IsListEmpty = MarketItems.Count == 0;
        }

        // --- Navigates to Market Detail Page when the main card is tapped ---
        [RelayCommand]
        private async Task NavigateToMarketDetailAsync(MarketItem item)
        {
            if (item == null) return;

            // Make sure "MarketDetail" (or whatever you named the detail page) matches the route registered in your AppShell.xaml
            await Shell.Current.GoToAsync("MarketInsight", new Dictionary<string, object>
            {
                { "CommodityId", item.CommodityId }
            });
        }

        // --- Navigates to PositionListing Page when the Price is tapped ---
        [RelayCommand]
        private async Task NavigateToPositionListingAsync(MarketItem item)
        {
            if (item == null) return;

            // Navigate to PositionListing, passing the correct Price based on the active tab
            var args = new PositionListingCommand
            {
                CommodityId = item.CommodityId,
                PositionTypeId = IsBidHighlighted ? 6001 : 6002,
                UnitPrice = item.CurrentPrice,
                CommodityName = item.Name,
            };

            await Shell.Current.GoToAsync("PositionListing", new Dictionary<string, object>
            {
                { "Args", args },
                { "PassedImage", item.ImageSource },
                { "PassedCommodityName", item.Name },
                { "PassedLotSize", item.LotSizeDisplay },
                { "PassedBid", IsBidHighlighted ? item.CurrentPrice.ToString("N0") : "-" },
                { "PassedOffer", IsOfferHighlighted ? item.CurrentPrice.ToString("N0") : "-" }
            });
        }

        [RelayCommand]
        private void OpenImagePreview(MarketItem item)
        {
            if (item == null) return;
            PreviewImage = item.ImageSource;
            selectedCommodityTypeName = item.Name.ToUpper();
            IsImagePreviewVisible = true;
        }

        [RelayCommand]
        private void CloseImagePreview()
        {
            IsImagePreviewVisible = false;
            PreviewImage = null;
            selectedCommodityTypeName = string.Empty;
        }
    }

    // A lightweight wrapper just for this UI screen
    public class MarketItem
    {
        public Guid CommodityId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LotSizeDisplay { get; set; } = string.Empty;
        public decimal CurrentPrice { get; set; }
        public string FormattedDifference { get; set; } = string.Empty;
        public bool IsPositiveTrend { get; set; }
        public ImageSource? ImageSource { get; set; }
        public string DisplayPrice { get; set; } = string.Empty;
    }
}