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

        // Price ladders from backend
        private List<PricePointDto> _bidPriceLadder = new();
        private List<PricePointDto> _offerPriceLadder = new();

        // Current indexes
        private int _bidIndex;
        private int _offerIndex;

        [ObservableProperty]
        private PositionListingCommand? navArgs;

        public ObservableCollection<RegionDto> Locations { get; } = new();

        public ObservableCollection<PositionListing> Listings { get; } = new();

        public ObservableCollection<PositionListing> Positions { get; } = new();

        [ObservableProperty]
        private RegionDto? selectedLocation;

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private bool isBidHighlighted;

        [ObservableProperty]
        private bool isOfferHighlighted;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool isListEmpty;

        [ObservableProperty]
        private string positionTypeName = string.Empty;

        [ObservableProperty]
        private string unitPrice = string.Empty;

        [ObservableProperty]
        private string priceDisplay = string.Empty;

        [ObservableProperty]
        private string commodityTypeName = string.Empty;

        [ObservableProperty]
        private string lotSize = string.Empty;

        [ObservableProperty]
        private ImageSource? commodityImage;

        [ObservableProperty]
        private string shelfLife = "---";

        [ObservableProperty]
        private string bidPrice = "-";

        [ObservableProperty]
        private string bidCount = "0 Buyer(s)";

        [ObservableProperty]
        private string offerPrice = "-";

        [ObservableProperty]
        private string offerCount = "0 Seller(s)";

        public PositionListingViewModel(
            SessionService sessionService,
            PositionApiService positionApi,
            ReferenceDataApiService referenceDataApi,
            IOptions<ApiSettings> apiSettings)
        {
            _sessionService = sessionService;
            _positionApi = positionApi;
            _referenceDataApi = referenceDataApi;
            _apiSettings = apiSettings.Value;
        }

        private bool _isInitializing = false;

        public async Task InitializeAsync(PositionListingCommand args)
        {
            if (_isInitializing)
                return;

            _isInitializing = true;

            await EnsureSessionActiveAsync();

            await LoadRegionsAsync();

            await LoadPositionListingAsync();

            _isInitializing = false;
        }

        private async Task EnsureSessionActiveAsync()
        {
            var isSessionValid =
                await _sessionService.ValidateAndRefreshSessionAsync();

            if (isSessionValid)
                await _sessionService.GetCurrentSessionAsync();
            else
                await _sessionService.TryRefreshTokenAsync();
        }

        private async Task LoadRegionsAsync()
        {
            var regionResp =
                await _referenceDataApi.GetRegionsAsync();

            if (regionResp.IsSuccessStatusCode)
            {
                var regions =
                    await regionResp.Content
                        .ReadFromJsonAsync<List<RegionDto>>();

                if (regions != null)
                {
                    Locations.Clear();

                    Locations.Add(new RegionDto
                    {
                        Id = 0,
                        NameInEnglish = "All Locations"
                    });

                    foreach (var r in regions)
                    {
                        Locations.Add(r);
                    }

                    SelectedLocation = Locations.First();
                }
            }
        }

        private async Task LoadPositionListingAsync()
        {
            if (NavArgs == null)
                return;

            try
            {
                IsBusy = true;

                var response =
                    await _positionApi.GetPositionListingAsync(NavArgs);

                if (!response.IsSuccessStatusCode)
                {
                    Positions.Clear();
                    Listings.Clear();
                    IsListEmpty = true;
                    return;
                }

                var dto =
                    await response.Content
                        .ReadFromJsonAsync<PositionListingResponseDto>();

                if (dto == null)
                {
                    Positions.Clear();
                    Listings.Clear();
                    IsListEmpty = true;
                    return;
                }

                CommodityTypeName =
                    dto.CommodityTypeName?.ToUpper() ?? string.Empty;

                LotSize = dto.LotSize ?? "---";

                ShelfLife = dto.ShelfLife ?? "---";

                UnitPrice =
                    dto.UnitPrice.ToString(
                        "N0",
                        new System.Globalization.CultureInfo("en-CM"));

                PriceDisplay = $"{UnitPrice} FCFA per {LotSize}";

                PositionTypeName =
                    $"Position Listings - {dto.PositionTypeName?.ToUpper()}";

                Listings.Clear();

                foreach (var item in dto.Listings)
                {
                    Listings.Add(item);
                }

                // Save ladders
                _bidPriceLadder = dto.BidPrices ?? new();
                _offerPriceLadder = dto.OfferPrices ?? new();

                // Initialize indexes
                _bidIndex = _bidPriceLadder.FindIndex(
                    x => x.Price == dto.UnitPrice);

                _offerIndex = _offerPriceLadder.FindIndex(
                    x => x.Price == dto.UnitPrice);

                if (_bidIndex < 0)
                    _bidIndex = 0;

                if (_offerIndex < 0)
                    _offerIndex = 0;

                UpdatePriceUi();

                ApplyFilters();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void UpdatePriceUi()
        {
            var culture =
                new System.Globalization.CultureInfo("en-CM");

            if (_bidPriceLadder.Any())
            {
                var currentBid = _bidPriceLadder[_bidIndex];

                BidPrice =
                    currentBid.Price.ToString("N0", culture);

                BidCount =
                    $"{currentBid.Count} Buyer(s)";
            }
            else
            {
                BidPrice = "-";
                BidCount = "0 Buyer(s)";
            }

            if (_offerPriceLadder.Any())
            {
                var currentOffer = _offerPriceLadder[_offerIndex];

                OfferPrice =
                    currentOffer.Price.ToString("N0", culture);

                OfferCount =
                    $"{currentOffer.Count} Seller(s)";
            }
            else
            {
                OfferPrice = "-";
                OfferCount = "0 (Seller)";
            }
        }

        partial void OnSearchTextChanged(string value)
            => ApplyFilters();

        partial void OnSelectedLocationChanged(RegionDto? value)
            => ApplyFilters();

        private void ApplyFilters()
        {
            Positions.Clear();

            var filtered = Listings.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(p =>
                    (p.UserName != null &&
                     p.UserName.Contains(
                         SearchText,
                         StringComparison.OrdinalIgnoreCase))
                    ||
                    (p.CommodityName != null &&
                     p.CommodityName.Contains(
                         SearchText,
                         StringComparison.OrdinalIgnoreCase)));
            }

            if (SelectedLocation != null &&
                SelectedLocation.Id != 0)
            {
                filtered = filtered.Where(p =>
                    p.LocationName ==
                    SelectedLocation.NameInEnglish);
            }

            foreach (var item in filtered)
            {
                Positions.Add(item);
            }

            IsListEmpty = Positions.Count == 0;
        }

        [RelayCommand]
        private async Task SelectPrice(string side)
        {
            if (NavArgs == null)
                return;

            IsBidHighlighted = side == "Bid";
            IsOfferHighlighted = side == "Offer";

            decimal selectedPrice;

            if (side == "Bid")
            {
                if (!_bidPriceLadder.Any())
                    return;

                selectedPrice =
                    _bidPriceLadder[_bidIndex].Price;

                NavArgs.PositionTypeId = 6001;
            }
            else
            {
                if (!_offerPriceLadder.Any())
                    return;

                selectedPrice =
                    _offerPriceLadder[_offerIndex].Price;

                NavArgs.PositionTypeId = 6002;
            }

            NavArgs.UnitPrice = selectedPrice;

            await LoadPositionListingAsync();
        }

        [RelayCommand]
        private async Task PreviousPrice(string side)
        {
            await ChangePriceAsync(side, -1);
        }

        [RelayCommand]
        private async Task NextPrice(string side)
        {
            await ChangePriceAsync(side, 1);
        }

        private async Task ChangePriceAsync(
            string side,
            int direction)
        {
            if (NavArgs == null)
                return;

            if (side == "Bid")
            {
                if (!_bidPriceLadder.Any())
                    return;

                IsBidHighlighted = true;
                IsOfferHighlighted = false;

                _bidIndex += direction;

                if (_bidIndex < 0)
                    _bidIndex = 0;

                if (_bidIndex >= _bidPriceLadder.Count)
                    _bidIndex = _bidPriceLadder.Count - 1;

                NavArgs.PositionTypeId = 6001;
                NavArgs.UnitPrice =
                    _bidPriceLadder[_bidIndex].Price;
            }
            else
            {
                if (!_offerPriceLadder.Any())
                    return;

                IsBidHighlighted = false;
                IsOfferHighlighted = true;

                _offerIndex += direction;

                if (_offerIndex < 0)
                    _offerIndex = 0;

                if (_offerIndex >= _offerPriceLadder.Count)
                    _offerIndex = _offerPriceLadder.Count - 1;

                NavArgs.PositionTypeId = 6002;
                NavArgs.UnitPrice =
                    _offerPriceLadder[_offerIndex].Price;
            }

            UpdatePriceUi();

            await LoadPositionListingAsync();
        }

        [RelayCommand]
        private async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task GoToPositionDetailAsync(PositionListing item)
        {
            var parameters =
                new Dictionary<string, object>
                {
                    { "positionId", item.PositionId }
                };

            await Shell.Current.GoToAsync(
                "PositionDetail",
                parameters);
        }

        public void ApplyQueryAttributes(
            IDictionary<string, object> query)
        {
            if (query.TryGetValue(
                    "PassedImage",
                    out var img) &&
                img is ImageSource imageSource)
            {
                CommodityImage = imageSource;
            }

            if (query.TryGetValue(
                    "PassedCommodityName",
                    out var name) &&
                name is string commodityName)
            {
                CommodityTypeName =
                    commodityName.ToUpper();
            }

            if (query.TryGetValue(
                    "PassedLotSize",
                    out var lot) &&
                lot is string lotSize)
            {
                LotSize = lotSize;
            }

            if (query.TryGetValue(
                    "PassedBid",
                    out var bid) &&
                bid is string bidStr)
            {
                BidPrice = bidStr;
            }

            if (query.TryGetValue(
                    "PassedOffer",
                    out var offer) &&
                offer is string offerStr)
            {
                OfferPrice = offerStr;
            }

            if (query.TryGetValue(
                    "Args",
                    out var argObj) &&
                argObj is PositionListingCommand args)
            {
                NavArgs = args;

                IsBidHighlighted =
                    args.PositionTypeId == 6001;

                IsOfferHighlighted =
                    args.PositionTypeId == 6002;

                _ = InitializeAsync(args);
            }
        }
    }
}