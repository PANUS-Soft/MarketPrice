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
        private readonly SessionService _sessionService;
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

        public MarketViewModel(SessionService sessionService, ReferenceDataApiService referenceDataApi, MarketApiService marketApi, IOptions<ApiSettings> apiSettingOptions)
        {
            _sessionService = sessionService;
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
            var marketInsightResponse = await _marketApi.LoadMarketAsync();

            if (!marketInsightResponse.IsSuccessStatusCode) return;
            
            var marketInsights = await marketInsightResponse.Content.ReadFromJsonAsync<List<MarketResponseDto>>();
            
            if (marketInsights == null) return;
            
            _allMarketItems.Clear();

            foreach (var insight in marketInsights)
            {
                _allMarketItems.Add(new MarketItem
                {
                    Filter = new MarketItemFilter
                    {
                        CommodityTypeId = insight.CommodityTypeId,
                        CommodityId = insight.CommodityId,
                        Name = insight.CommodityName!
                    },
                    ImageSource = ImageSource.FromUri(new Uri($"{_apiSettingOptions.BaseUrl}{insight.ImageUrl}")),
                    BestBid = insight.BestBid != 0 ? insight.BestBid.ToString("N0", new System.Globalization.CultureInfo("en-CM")) : "No Bids",
                    LotSize = insight.LotSize,
                    UnitOfMeasure = insight.UnitOfMeasure,
                    LotSizeDisplay = $"{insight.LotSize} {insight.UnitOfMeasure}",
                    BestOffer = insight.BestOffer != 0 ? insight.BestOffer.ToString("N0", new System.Globalization.CultureInfo("en-CM")) : "No Offers",
                    IsBidUp = insight.IsBidImproved,
                    IsBidDown = !insight.IsBidImproved,
                    IsBidNull = insight.BestBid == 0,
                    IsOfferUp = insight.IsOfferImproved,
                    IsOfferDown = !insight.IsOfferImproved,
                    IsOfferNull = insight.BestOffer == 0,
                    IsBidSoonToExpire = insight.IsBidSoonToExpire,
                    IsOfferSoonToExpire = insight.IsOfferSoonToExpire
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

            bool accessAllowed = await _sessionService.EnsureUserAccessAsync();

            if (!accessAllowed)
            {
                bool accessAccount = await Shell.Current.DisplayAlert("Access Required", "You need to have an account in order to deeper explore the platform.\n\nYou can either create an account or login into an existing one", "Register or Login", "Cancel");

                if (!accessAccount) return;

                await Shell.Current.GoToAsync("//Welcome");
                return;
            }

            await Shell.Current.GoToAsync("MarketInsight", new Dictionary<string, object>()
            {
                {"SelectedMarketItemFilter", selectedItem }
            });
        }

        [RelayCommand]
        private async Task NavigateToBidPositionListing(MarketItem item)
        {
            if (item.BestBid == "No Bids") return;

            bool accessAllowed = await _sessionService.EnsureUserAccessAsync();

            if (!accessAllowed)
            {
                bool accessAccount = await Shell.Current.DisplayAlert("Access Required", "You need to have an account in order to deeper explore the platform. \n\nYou can either create an account or login into an existing one", "Register or Login", "Cancel");

                if (!accessAccount) return;

                await Shell.Current.GoToAsync("//Welcome");

                return;
            }

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

            bool accessAllowed = await _sessionService.EnsureUserAccessAsync();

            if (!accessAllowed)
            {
                bool accessAccount = await Shell.Current.DisplayAlert("Access Required", "You need to have an account in order to deeper explore the platform. \n\nYou can either create an account or login into an existing one", "Register or Login", "Cancel");

                if (!accessAccount) return;

                await Shell.Current.GoToAsync("//Welcome");

                return;
            }

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