using System.Collections.ObjectModel;
using System.Net.Http.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketPrice.Domain.Position.Commands;
using MarketPrice.Domain.Position.DTOs;
using MarketPrice.Domain.Reference.DTOs;
using MarketPrice.Ui.Common;
using MarketPrice.Ui.Services.Api;
using MarketPrice.Ui.Services.Session;
using Microsoft.Extensions.Options;

namespace MarketPrice.Ui.ViewModels
{
    public partial class PositionListingViewModel : ObservableObject, IQueryAttributable
    {
        private readonly SessionService _sessionService;
        private readonly PositionApiService _positionApi;
        private readonly ReferenceDataApiService _referenceDataApi;
        private readonly ApiSettings _apiSettings;

        [ObservableProperty] private PositionListingCommand? navArgs;

        public ObservableCollection<RegionDto> Locations { get; } = new();
        public ObservableCollection<PositionListing> Listings { get; } = new();
        public ObservableCollection<PositionListing> Positions { get; } = new();

        [ObservableProperty] private RegionDto? selectedLocation;
        [ObservableProperty] private string searchText = string.Empty;

        [ObservableProperty] private bool isBidHighlighted;
        [ObservableProperty] private bool isOfferHighlighted;

        [ObservableProperty] private bool isBusy;
        [ObservableProperty] private bool isListEmpty;
        [ObservableProperty] private string positionTypeName;
        [ObservableProperty] private string unitPrice;
        [ObservableProperty] private string priceDisplay;

        [ObservableProperty] private string commodityTypeName;
        [ObservableProperty] private string lotSize;
        [ObservableProperty] private ImageSource? commodityImage;
        [ObservableProperty] private string shelfLife = "12 Months";
        [ObservableProperty] private string bidPrice = "-";
        [ObservableProperty] private string bidCount = "0";
        [ObservableProperty] private string offerPrice = "-";
        [ObservableProperty] private string offerCount = "0";

        public PositionListingViewModel(SessionService sessionService, PositionApiService positionApi, ReferenceDataApiService referenceDataApi, IOptions<ApiSettings> apiSettings)
        {
            _sessionService = sessionService;
            _positionApi = positionApi;
            _referenceDataApi = referenceDataApi;
            _apiSettings = apiSettings.Value;
        }

        private bool _isInitializing = false;

        public async Task InitializeAsync(PositionListingCommand args)
        {
            if (_isInitializing) return;
            _isInitializing = true;

            await EnsureSessionActiveAsync();

            await LoadRegionsAsync();
            await LoadPositionListingAsync();

            _isInitializing = false;
        }

        private async Task EnsureSessionActiveAsync()
        {
            var isSessionValid = await _sessionService.ValidateAndRefreshSessionAsync();
            if (isSessionValid) await _sessionService.GetCurrentSessionAsync();
            else await _sessionService.TryRefreshTokenAsync();
        }

        private async Task LoadRegionsAsync()
        {
            var regionResp = await _referenceDataApi.GetRegionsAsync();
            if (regionResp.IsSuccessStatusCode)
            {
                var regions = await regionResp.Content.ReadFromJsonAsync<List<RegionDto>>();
                if (regions != null)
                {
                    Locations.Clear();
                    Locations.Add(new RegionDto { Id = 0, NameInEnglish = "All Locations" });
                    foreach (var r in regions) Locations.Add(r);

                    selectedLocation = Locations.First();
                    OnPropertyChanged(nameof(SelectedLocation));
                }
            }
        }

        private async Task LoadPositionListingAsync()
        {
            if (NavArgs == null) return;
            IsBusy = true;

            var positionListingResponse = await _positionApi.GetPositionListingAsync(NavArgs);

            if (!positionListingResponse.IsSuccessStatusCode)
            {
                IsBusy = false;
                IsListEmpty = true;
                return;
            }

            var dto = await positionListingResponse.Content.ReadFromJsonAsync<PositionListingResponseDto>();
            if (dto == null)
            {
                IsBusy = false;
                IsListEmpty = true;
                return;
            }

            if (string.IsNullOrEmpty(CommodityTypeName)) CommodityTypeName = dto.CommodityTypeName?.ToUpper() ?? string.Empty;
            if (string.IsNullOrEmpty(LotSize)) LotSize = dto.LotSize;

            UnitPrice = dto.UnitPrice.ToString("N0", new System.Globalization.CultureInfo("en-CM"));
            PositionTypeName = $"Position Listings - {dto.PositionTypeName?.ToUpper() ?? string.Empty}";

            Listings.Clear();
            foreach (var item in dto.Listings) Listings.Add(item);

            ApplyFilters();
            IsBusy = false;
        }

        partial void OnSearchTextChanged(string value) => ApplyFilters();
        partial void OnSelectedLocationChanged(RegionDto? value) => ApplyFilters();

        private void ApplyFilters()
        {
            Positions.Clear();
            var filtered = Listings.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(p =>
                    (p.UserName != null && p.UserName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
                    (p.CommodityName != null && p.CommodityName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                );
            }

            if (SelectedLocation != null && SelectedLocation.Id != 0)
            {
                filtered = filtered.Where(p => p.LocationName == SelectedLocation.NameInEnglish);
            }

            foreach (var item in filtered) Positions.Add(item);
            IsListEmpty = Positions.Count == 0;
            PriceDisplay = $"{UnitPrice} FCFA per {LotSize}";
        }

        // =========================================================================
        // BACKEND DEVELOPER NOTES - PRICE CYCLING INTEGRATION
        //
        // Right now, the arrows (< >) use a "Mock Price Ladder" to simulate 
        // cycling through available prices so the frontend UI can be tested.
        // 
        // TO LINK TO THE REAL DATABASE:
        // 1. You need an API endpoint that returns a List of all distinct prices 
        //    (decimals) where positions currently exist for this commodity.
        // 2. Populate `_bidPriceLadder` and `_offerPriceLadder` with those real 
        //    values during `InitializeAsync()` or `LoadPositionListingAsync()`.
        // 3. Ensure the lists are sorted from LOWEST to HIGHEST price.
        // 4. Once you map the real data, DELETE the `GenerateMockPriceLadder()` 
        //    method at the bottom of this file.
        // =========================================================================
        private List<decimal> _bidPriceLadder = new();
        private List<decimal> _offerPriceLadder = new();

        [RelayCommand]
        private async Task SelectPrice(string side)
        {
            if (NavArgs == null) return;

            // 1. Prevent reloading if they click the tab that is already active
            if (side == "Bid" && IsBidHighlighted) return;
            if (side == "Offer" && IsOfferHighlighted) return;

            // 2. Swap the Highlights!
            IsBidHighlighted = side == "Bid";
            IsOfferHighlighted = side == "Offer";

            // 3. Set PositionTypeId to the new side (6001 = Bid, 6002 = Offer)
            NavArgs.PositionTypeId = IsBidHighlighted ? 6001 : 6002;

            // 4. Extract the clean number from the UI string (e.g., "1,500" -> 1500)
            string priceStr = IsBidHighlighted ? BidPrice : OfferPrice;
            var rawPrice = new string(priceStr?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>());

            // 5. If there is a valid price, fetch the positions!
            if (decimal.TryParse(rawPrice, out var price))
            {
                NavArgs.UnitPrice = price;
                await LoadPositionListingAsync();
            }
            else
            {
                // 6. If they click a side with no prices (e.g. "-"), empty the screen
                Positions.Clear();
                Listings.Clear();
                IsListEmpty = true;
            }
        }

        [RelayCommand]
        private async Task PreviousPrice(string side)
        {
            await CyclePriceAsync(side, -1); // Move 1 index backward in the ladder
        }

        [RelayCommand]
        private async Task NextPrice(string side)
        {
            await CyclePriceAsync(side, 1); // Move 1 index forward in the ladder
        }

        private async Task CyclePriceAsync(string side, int direction)
        {
            // First, trigger the highlight and set the current base price
            await SelectPrice(side);
            if (NavArgs == null) return;

            // 1. Grab the correct list for the side they clicked
            var priceLadder = side == "Bid" ? _bidPriceLadder : _offerPriceLadder;
            if (priceLadder.Count == 0) return;

            // 2. Find where our current price is in the list
            int currentIndex = priceLadder.IndexOf(NavArgs.UnitPrice);
            if (currentIndex == -1) currentIndex = 0;

            // 3. Move to the next/previous index based on the arrow clicked
            int newIndex = currentIndex + direction;

            // 4. Prevent going out of bounds (can't go past the first or last price)
            if (newIndex < 0) newIndex = 0;
            if (newIndex >= priceLadder.Count) newIndex = priceLadder.Count - 1;

            // 5. Get the new exact price from the list
            decimal newPrice = priceLadder[newIndex];

            // If we are already at the end of the list, do nothing
            if (newPrice == NavArgs.UnitPrice) return;

            // 6. Update the string in the UI
            string formattedPrice = newPrice.ToString("N0", new System.Globalization.CultureInfo("en-CM"));
            if (side == "Bid") BidPrice = formattedPrice;
            else OfferPrice = formattedPrice;

            // 7. Update the API payload and fetch the new list!
            NavArgs.UnitPrice = newPrice;
            await LoadPositionListingAsync();
        }

        // =========================================================================
        // TODO BACKEND DEV: DELETE THIS ENTIRE METHOD ONCE LINKED TO API
        // This generates temporary prices around the current price so the frontend 
        // developer can test the clicking of the arrows.
        // =========================================================================
        private void GenerateMockPriceLadder(string side, decimal currentPrice)
        {
            var targetList = side == "Bid" ? _bidPriceLadder : _offerPriceLadder;

            // Only generate the mock list if it's currently empty
            if (targetList.Count == 0)
            {
                // Creating 5 available prices, stepped by 500 FCFA
                targetList.Add(currentPrice - 1000);
                targetList.Add(currentPrice - 500);
                targetList.Add(currentPrice);
                targetList.Add(currentPrice + 500);
                targetList.Add(currentPrice + 1000);

                // Real data must also be sorted from lowest to highest!
                targetList.Sort();
            }
        }

        [RelayCommand]
        private async Task GoBackAsync() => await Shell.Current.GoToAsync("..");

        [RelayCommand]
        private async Task GoToPositionDetailAsync(PositionListing item)
        {
            var parameters = new Dictionary<string, object> { { "positionId", item.PositionId } };
            await Shell.Current.GoToAsync("PositionDetail", parameters);
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("PassedImage", out var img) && img is ImageSource imageSource)
                CommodityImage = imageSource;

            if (query.TryGetValue("PassedCommodityName", out var name) && name is string commodityName)
                CommodityTypeName = commodityName.ToUpper();

            if (query.TryGetValue("PassedLotSize", out var lot) && lot is string lotSize)
                LotSize = lotSize;

            if (query.TryGetValue("PassedBid", out var bid) && bid is string bidStr)
                BidPrice = bidStr;

            if (query.TryGetValue("PassedOffer", out var offer) && offer is string offerStr)
                OfferPrice = offerStr;

            if (query.TryGetValue("Args", out var argObj) && argObj is PositionListingCommand args)
            {
                NavArgs = args;

                // Set the Highlight correctly based on what was clicked on the Market page
                IsBidHighlighted = args.PositionTypeId == 6001;
                IsOfferHighlighted = args.PositionTypeId == 6002;

                _ = InitializeAsync(args);
            }
        }
    }
}