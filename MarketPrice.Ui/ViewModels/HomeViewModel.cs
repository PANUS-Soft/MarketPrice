using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketPrice.Domain.Home.DTOs;
using MarketPrice.Domain.Position.Commands;
using MarketPrice.Ui.Common;
using MarketPrice.Ui.Models;
using MarketPrice.Ui.Services.Api;
using MarketPrice.Ui.Services.Session;
using MarketPrice.Ui.Views;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;

namespace MarketPrice.Ui.ViewModels
{
    public partial class HomeViewModel(
        SessionService sessionService,
        HomeApiService homeApi,
        IOptions<ApiSettings> apiSettingOptions) : ObservableObject
    {
        // ?? Collections ????????????????????????????????????????????
        public ObservableCollection<CommodityGroupDisplayModel> Commodities { get; } = new();
        public ObservableCollection<CommodityDisplayModel> SearchResults { get; } = new();

        // ?? Observable properties ???????????????????????????????????
        [ObservableProperty] private ImageSource? _previewImage;
        [ObservableProperty] private bool _isImagePreviewVisible;
        [ObservableProperty] private string? _selectedCommodityTypeName;
        [ObservableProperty] private bool _isSearchActive;
        [ObservableProperty] private bool _isSearchEmpty;
        [ObservableProperty] private bool _isSearchHintVisible = false;
        [ObservableProperty] private bool _hasSearchResults;
        [ObservableProperty] private string _searchText = string.Empty;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        // Lifecycle 
        public async Task InitializeAsync() => await LoadHomeDataAsync();

        // Data loading
        public async Task LoadHomeDataAsync()
        {
            System.Diagnostics.Debug.WriteLine("[HOME] LoadHomeDataAsync started");

            //var isSessionValid = await sessionService.ValidateAndRefreshSessionAsync();
            //if (isSessionValid) await sessionService.GetCurrentSessionAsync();
            //else await sessionService.TryRefreshTokenAsync();

            try
            {
                var homeDataResponse = await homeApi.LoadHomeAsync();
                System.Diagnostics.Debug.WriteLine($"[HOME] API status: {homeDataResponse.StatusCode}");

                if (!homeDataResponse.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"[HOME] ? API FAILED: {homeDataResponse.StatusCode}");
                    await Shell.Current.DisplayAlert("Error", "Unable to load home data.", "OK");
                    return;
                }

                var homeData = await homeDataResponse.Content
                    .ReadFromJsonAsync<List<LoadHomeResponseDto>>(JsonOptions);

                System.Diagnostics.Debug.WriteLine($"[HOME] homeData null: {homeData == null}");
                System.Diagnostics.Debug.WriteLine($"[HOME] homeData count: {homeData?.Count ?? 0}");

                if (homeData == null || homeData.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("[HOME] ?? No data returned from API");
                    return;
                }

                Commodities.Clear();

                foreach (var group in homeData.OrderBy(x => x.CommodityTypeName, StringComparer.OrdinalIgnoreCase))
                {
                    System.Diagnostics.Debug.WriteLine($"[HOME] Group: '{group.CommodityTypeName}' | Commodities: {group.Commodities?.Count ?? 0}");

                    var groupModel = new CommodityGroupDisplayModel
                    {
                        CommodityTypeId = group.CommodityTypeId,
                        GroupName = group.CommodityTypeName?.ToUpperInvariant() ?? string.Empty,
                    };

                    foreach (var dto in group.Commodities ?? new())
                    {
                        System.Diagnostics.Debug.WriteLine($"[HOME]   ? '{dto.CommodityName}' Bids:{dto.BidDepth?.Count} Offers:{dto.OfferDepth?.Count}");

                        var bid0 = dto.BidDepth?.ElementAtOrDefault(0);
                        var bid1 = dto.BidDepth?.ElementAtOrDefault(1);
                        var bid2 = dto.BidDepth?.ElementAtOrDefault(2);
                        var offer0 = dto.OfferDepth?.ElementAtOrDefault(0);
                        var offer1 = dto.OfferDepth?.ElementAtOrDefault(1);
                        var offer2 = dto.OfferDepth?.ElementAtOrDefault(2);

                        groupModel.Commodities.Add(new CommodityDisplayModel
                        {
                            CommodityId = dto.CommodityId,
                            Name = dto.CommodityName ?? string.Empty,
                            ImageUrl = ImageSource.FromUri(new Uri(
                                                    $"{apiSettingOptions.Value.BaseUrl}{dto.ImageUrl}")),
                            LotSizeDisplay = dto.LotSize.HasValue
                                                    ? $"{dto.LotSize} {dto.UnitOfMeasure}"
                                                    : "---",
                            IsBidImproved = dto.IsBidImproved,
                            IsOfferImproved = dto.IsOfferImproved,
                            IsBidSoonToExpire = dto.IsBidSoonToExpire,
                            IsOfferSoonToExpire = dto.IsOfferSoonToExpire,

                            // Best Bid
                            HasBid = bid0 != null && bid0.Price > 0,
                            BestBidPrice = bid0?.Price ?? 0,
                            BestBidDisplay = bid0 != null && bid0.Price > 0
                                                    ? bid0.Price.ToString("N0", new CultureInfo("en-CM"))
                                                    : "No Bid",
                            BestBidQuantity = bid0?.TotalActivePosforPrice ?? 0,
                            BestBidLocation = bid0?.Locations.FirstOrDefault() ?? string.Empty,
                            NextBid1 = bid1?.Price ?? 0,
                            NextBid1Display = bid1 != null && bid1.Price > 0 ? bid1.Price.ToString("N0", new CultureInfo("en-CM")) : "-",
                            NextBid1Location = bid1?.Locations.FirstOrDefault() ?? string.Empty,
                            NextBid2 = bid2?.Price ?? 0,
                            NextBid2Display = bid2 != null && bid2.Price > 0 ? bid2.Price.ToString("N0", new CultureInfo("en-CM")) : "-",
                            NextBid2Location = bid2?.Locations.FirstOrDefault() ?? string.Empty,

                            // Best Offer
                            HasOffer = offer0 != null && offer0.Price > 0,
                            BestOfferPrice = offer0?.Price ?? 0,
                            BestOfferDisplay = offer0 != null && offer0.Price > 0
                                                    ? offer0.Price.ToString("N0", new CultureInfo("en-CM"))
                                                    : "No Offer",
                            BestOfferQuantity = offer0?.TotalActivePosforPrice ?? 0,
                            BestOfferLocation = offer0?.Locations.FirstOrDefault() ?? string.Empty,
                            NextOffer1 = offer1?.Price ?? 0,
                            NextOffer1Display = offer1 != null && offer1.Price > 0 ? offer1.Price.ToString("N0", new CultureInfo("en-CM")) : "-",
                            NextOffer1Location = offer1?.Locations.FirstOrDefault() ?? string.Empty,
                            NextOffer2 = offer2?.Price ?? 0,
                            NextOffer2Display = offer2 != null && offer2.Price > 0 ? offer2.Price.ToString("N0", new CultureInfo("en-CM")) : "-",
                            NextOffer2Location = offer2?.Locations.FirstOrDefault() ?? string.Empty,

                            // Buyer/Seller counts
                            BuyerCountDisplay = bid0 != null && bid0.TotalActivePosforPrice > 0 ? $"x{bid0.TotalActivePosforPrice:N0} Buyer(s)" : "x0 Buyer(s)",
                            SellerCountDisplay = offer0 != null && offer0.TotalActivePosforPrice > 0 ? $"x{offer0.TotalActivePosforPrice:N0} Seller(s)" : "x0 Seller(s)",});
                    }

                    Commodities.Add(groupModel);
                    System.Diagnostics.Debug.WriteLine($"[HOME] ? Group '{groupModel.GroupName}' added with {groupModel.Commodities.Count} commodities");
                }

                System.Diagnostics.Debug.WriteLine($"[HOME] ? Commodities final count: {Commodities.Count}");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"[HOME] ? EXCEPTION: {e.GetType().Name}: {e.Message}");
                System.Diagnostics.Debug.WriteLine($"[HOME] STACK: {e.StackTrace}");
                await Shell.Current.DisplayAlert("Error", $"There was an error loading data. {e.Message}", "OK");
            }
        }

        // Search
        partial void OnSearchTextChanged(string value) => ApplySearch(value);

        private void ApplySearch(string query)
        {
            SearchResults.Clear();
           
            if (string.IsNullOrWhiteSpace(query))
            {
                IsSearchHintVisible = true;
                IsSearchEmpty = false;
                HasSearchResults = false;
                return;
            }

            IsSearchHintVisible = false;

            var lower = query.ToLowerInvariant();

            var matches = Commodities
                .SelectMany(g => g.Commodities
                       .Where(c =>
                                c.Name.Contains(lower, StringComparison.OrdinalIgnoreCase) ||
                                g.GroupName.Contains(lower, StringComparison.OrdinalIgnoreCase))
                       .Select(c => c))
                .OrderBy(c => c.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (var c in matches)
                SearchResults.Add(c);

            HasSearchResults = SearchResults.Count > 0;
            IsSearchEmpty = !String.IsNullOrWhiteSpace(query) && SearchResults.Count == 0;

        }

        [RelayCommand]
        private async Task ViewAll(Guid commodityTypeId)
        {
            var group = Commodities.FirstOrDefault(g => g.CommodityTypeId == commodityTypeId);
            if (group == null) return;

            await Shell.Current.GoToAsync(nameof(CommodityListing),
                new Dictionary<string, object>()
                {
                    {"GroupName", group.GroupName },
                    {"Commodities", group.Commodities.ToList()}
                });
        }
 
        [RelayCommand]
        private void ActivateSearch()
        {
            SearchText = string.Empty;
            IsSearchEmpty = false;
            IsSearchHintVisible = true; 
            HasSearchResults = false;
            SearchResults.Clear();
            IsSearchActive = true;
        }

        [RelayCommand]
        private void CancelSearch()
        {
            SearchText = string.Empty;
            IsSearchHintVisible = false;
            IsSearchEmpty = false;
            HasSearchResults = false;
            SearchResults.Clear();
            IsSearchActive = false;
        }

        [RelayCommand]
        private void ClearSearch()
        {
            SearchText = string.Empty;
            IsSearchHintVisible = true;
            IsSearchEmpty = false;
            HasSearchResults = false;
            SearchResults.Clear();
        }

        // Image preview
        [RelayCommand]
        private void OpenImagePreview(CommodityDisplayModel item)
        {
            if (item == null) return;
            PreviewImage = item.ImageUrl;
            SelectedCommodityTypeName = item.Name?.ToUpper();
            IsImagePreviewVisible = true;
        }

        [RelayCommand]
        private void CloseImagePreview()
        {
            IsImagePreviewVisible = false;
            PreviewImage = null;
            SelectedCommodityTypeName = string.Empty;
        }

        // ?? Navigation ??????????????????????????????????????????????
        [RelayCommand]
        private async Task NavigateToBidPositionListing(CommodityDisplayModel item)
        {
            if (item.BestBidPrice == 0) return;

            var args = new PositionListingCommand
            {
                CommodityTypeId = item.CommodityId,
                UnitPrice = item.BestBidPrice,
                PositionTypeId = 6001
            };

            await Shell.Current.GoToAsync(nameof(PositionListing), new Dictionary<string, object>
            {
                { "Args", args }
            });
        }

        [RelayCommand]
        private async Task NavigateToOfferPositionListing(CommodityDisplayModel item)
        {
            if (item.BestOfferPrice == 0) return;

            var args = new PositionListingCommand
            {
                CommodityTypeId = item.CommodityId,
                UnitPrice = item.BestOfferPrice,
                PositionTypeId = 6002
            };

            await Shell.Current.GoToAsync(nameof(PositionListing), new Dictionary<string, object>
            {
                { "Args", args }
            });
        }
    }
}