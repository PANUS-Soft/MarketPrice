using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketPrice.Ui.Common;
using MarketPrice.Ui.Models;
using MarketPrice.Ui.Views;
using MarketPrice.Domain.Market.DTOs;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using MarketPrice.Domain.Reference;
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
        private readonly LoadMarketApiService _loadMarketInsightApi;
        private readonly ApiSettings _apiSettingOptions;

        public readonly List<MarketItem> _allMarketItems = new();
        public ObservableCollection<string> CommodityTypesList { get; } = new();
        public ObservableCollection<MarketItem> MarketItems { get; } = new();

        [ObservableProperty] private ImageSource previewImage;
        [ObservableProperty] private bool isImagePreviewVisible;
        [ObservableProperty] private string selectedCommodityTypeName;

        [ObservableProperty] private string selectedCommodityType = "ALL";
        [ObservableProperty] private string searchText = string.Empty;
        [ObservableProperty] private bool isListEmpty;

        public MarketViewModel(SessionService sessionService, ReferenceDataApiService referenceDataApi, LoadMarketApiService loadMarketApi, IOptions<ApiSettings> apiSettingOptions)
        {
            _sessionService = sessionService;
            _referenceDataApi = referenceDataApi;
            _loadMarketInsightApi = loadMarketApi;
            _apiSettingOptions = apiSettingOptions.Value;

            _ = InitializeAsync();
        }

        public MarketViewModel()
        {
        }

        private async Task InitializeAsync()
        {
            await EnsureSessionActiveAsync();
            await LoadCommodityTypesAsync();
            await LoadMarketInsightAsync();
        }

        private async Task EnsureSessionActiveAsync()
        {
            var isSessionValid = await _sessionService.ValidateAndRefreshSessionAsync();
            
            if (isSessionValid) await _sessionService.GetCurrentSessionAsync();
            else await _sessionService.TryRefreshTokenAsync();
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

            foreach (var type in commodityTypes) CommodityTypesList.Add(type.Name!.ToUpper());
        }

        public async Task LoadMarketInsightAsync()
        {
            var marketInsightResponse = await _loadMarketInsightApi.LoadMarketAsync();

            if (!marketInsightResponse.IsSuccessStatusCode) return;
            
            var marketInsights = await marketInsightResponse.Content.ReadFromJsonAsync<List<MarketResponseDto>>();
            
            if (marketInsights == null) return;
            
            _allMarketItems.Clear();

            foreach (var insight in marketInsights)
            {
                _allMarketItems.Add(new MarketItem
                {
                    Name = insight.CommodityName!,
                    ImageSource = ImageSource.FromUri(new Uri($"{_apiSettingOptions.BaseUrl}{insight.ImageUrl}")) ?? "smile.png",
                    BestBid = insight.BestBid,
                    LotSize = insight.LotSize,
                    UnitOfMeasure = insight.UnitOfMeasure,
                    LotSizeDisplay = $"{insight.LotSize} {insight.UnitOfMeasure}",
                    BestOffer = insight.BestOffer,
                    IsBidUp = insight.IsBidImproved,
                    IsBidDown = !insight.IsBidImproved,
                    IsOfferUp = insight.IsOfferImproved,
                    IsOfferDown = !insight.IsOfferImproved
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
                filtered = filtered.Where(item => item.Name != null && item.Name.Contains(SelectedCommodityType, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(item => item.Name != null && item.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            MarketItems.Clear();

            foreach (var item in filtered) MarketItems.Add(item);

            IsListEmpty = MarketItems.Count == 0;
        }

        [RelayCommand]
        private async Task NavigateToCommodityInsight(MarketItem? selectedItem)
        {
            if (selectedItem == null) return;

            await Shell.Current.GoToAsync("MarketInsight", new Dictionary<string, object>()
            {
                {"SelectedMarketItem", selectedItem }
            });
        }

        [RelayCommand]
        private void OpenImagePreview(MarketItem item)
        {
            if (item == null) return;

            PreviewImage = item.ImageSource;
            SelectedCommodityTypeName = item.Name.ToUpper();
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