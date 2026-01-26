using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketPrice.Ui.Common;
using MarketPrice.Ui.Models;
using MarketPrice.Ui.Views;
using MarketPrice.Domain.Market.Dtos;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using MarketPrice.Domain.Reference;
using MarketPrice.Ui.Services.Api;
using MarketPrice.Ui.Services.Session;

namespace MarketPrice.Ui.ViewModels
{
    public partial class MarketViewModel (SessionService sessionService, ReferenceDataApiService referenceDataApi, LoadMarketInsightApiService loadMarketInsightApi) : ObservableObject
    {
        public ObservableCollection<MarketInsightResponseDto> MarketInsights { get; } = new();
        public ObservableCollection<CommodityTypeDto> CommodityTypes { get; } = new();
        public ObservableCollection<string> CommodityTypesList { get; } = new();
        public ObservableCollection<MarketItem> MarketItems { get; } = new();

        [ObservableProperty] private string selectedCommodityType;
        [ObservableProperty] private string searchText = string.Empty;

        public async Task LoadCommodityTypes()
        {
            var isSessionValid = await sessionService.ValidateAndRefreshSessionAsync();

            if (isSessionValid) await sessionService.GetCurrentSessionAsync();
            else await sessionService.TryRefreshTokenAsync();

            try
            {
                var commodityTypesResponse = await referenceDataApi.GetCommodityTypesAsync();

                if (commodityTypesResponse.IsSuccessStatusCode)
                {
                    CommodityTypes.Clear();
                    CommodityTypesList.Clear();
                    var commodityTypes = await commodityTypesResponse.Content.ReadFromJsonAsync<List<CommodityTypeDto>>();

                    if (commodityTypes != null)
                    {
                        foreach (var type in commodityTypes)
                        {
                            CommodityTypes.Add(type);
                            CommodityTypesList.Add(type.Name!.ToUpper());
                        }
                        CommodityTypesList.Insert(0, "ALL");
                        SelectedCommodityType = CommodityTypesList[0];
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log them)
                await Shell.Current.DisplayAlert("Error", $"Error loading commodity types: {ex.Message}", "OK");
            }
        }

        public async Task LoadMarketInsightAsync()
        {
            var isSessionValid = await sessionService.ValidateAndRefreshSessionAsync();

            if (isSessionValid) await sessionService.GetCurrentSessionAsync();
            else await sessionService.TryRefreshTokenAsync();
        [ObservableProperty]
        bool isListEmpty;

        public MarketViewModel()
        {
            CommodityTypes = new ObservableCollection<string>()
            {
                "ALL", "BEANS", "CORN", "EGUSI", "GINGER", "ONION", "OIL"
            };

            try
            {
                var marketInsightDataResponse = await 
            }



        }


        // Holds the original data so we never "lose" items when filtering
        //private readonly List<MarketItem> _allMarketItems;

        //public ObservableCollection<string> CommodityTypes { get; set; }
        //public ObservableCollection<MarketItem> MarketItems { get; set; }

        //[ObservableProperty]
        //string selectedCommodityType;

        //[ObservableProperty]
        //string searchText = string.Empty;

        //public MarketViewModel()
        //{
        //    CommodityTypes = new ObservableCollection<string>()
        //    {
        //        "ALL", "BEANS", "CORN", "EGUSI", "GINGER", "ONION", "OIL"
        //    };

        //    _allMarketItems = new List<MarketItem>()
        //    {
        //        new MarketItem {Name = "Fresh Corn", Code = "CRN", ImageSource = "fresh_corn.png", BestBid = 350, BestOffer = 275, IsBidUp = true, IsOfferDown = true},
        //        new MarketItem {Name = "Dry Corn", Code = "CRN", ImageSource = "dry_corn.png", BestBid = 375, BestOffer = 300, IsBidUp = true, IsOfferDown = true},
        //        new MarketItem {Name = "Red Beans", Code = "BNS", ImageSource = "red_beans.png", BestBid = 275, BestOffer = 275, IsBidUp = true, IsOfferDown = true},
        //        new MarketItem {Name = "Black beans", Code = "BNS", ImageSource = "black_beans.png", BestBid = 325, BestOffer = 300, IsBidUp = true, IsOfferDown = true},
        //        new MarketItem {Name = "White Beans", Code = "BNS", ImageSource = "white_beans.png", BestBid = 350, BestOffer = 400, IsBidUp = true, IsOfferDown = true},
        //        new MarketItem {Name = "Ginger", Code = "GIN", ImageSource = "ginger.png", BestBid = 250, BestOffer = 200, IsBidUp = true, IsOfferDown = true},
        //        new MarketItem {Name = "Palm Oil", Code = "OIL", ImageSource = "palm_oil.png", BestBid = 350, BestOffer = 375, IsBidUp = true, IsOfferDown = true},
        //        new MarketItem {Name = "Cracked Egusi", Code = "EGU", ImageSource = "cracked_egusi.png", BestBid = 400, BestOffer = 350, IsBidUp = true, IsOfferDown = true}
        //    };

        //    MarketItems = new ObservableCollection<MarketItem>(_allMarketItems);

        //    SelectedCommodityType = CommodityTypes[0];
        //}


        //partial void OnSelectedCommodityTypeChanged(string value) => ApplyFilters();

        //partial void OnSearchTextChanged(string value)
        //{
        //    if (!string.IsNullOrWhiteSpace(value))
        //    {
        //        var matchedCommodityType = CommodityTypes.FirstOrDefault(c => c.Equals(value.Trim(), StringComparison.OrdinalIgnoreCase));

        //        if (matchedCommodityType != null && SelectedCommodityType != matchedCommodityType)
        //            SelectedCommodityType = matchedCommodityType;
        //    }

        //    ApplyFilters();
        //}

        //private void ApplyFilters()
        //{
        //    var filtered = _allMarketItems.AsEnumerable();


        //    if (!string.IsNullOrEmpty(SelectedCommodityType) && SelectedCommodityType != "ALL")
        //    {
        //        filtered = filtered.Where(item =>
        //            item.Name.ToUpper().Contains(SelectedCommodityType.ToUpper()) ||
        //            item.Code.ToUpper().Contains(SelectedCommodityType.ToUpper()));
        //    }


        //    if (!string.IsNullOrWhiteSpace(SearchText))
        //    {
        //        filtered = filtered.Where(item =>
        //            item.Name.ToUpper().Contains(SearchText.ToUpper()) ||
        //            item.Code.ToUpper().Contains(SearchText.ToUpper()));
        //    }

        //    UpdateDisplayList(filtered);
        //}

        //private void UpdateDisplayList(IEnumerable<MarketItem> newItems)
        //{
        //    MarketItems.Clear();
        //    foreach (var item in newItems)
        //    {
        //        MarketItems.Add(item);
        //    }
        //}
        private void UpdateDisplayList(IEnumerable<MarketItem> newItems)
        {
            MarketItems.Clear();
            foreach (var item in newItems)
            {
                MarketItems.Add(item);
            }

            IsListEmpty = MarketItems.Count == 0;
        }

        [RelayCommand]
        private async Task NavigateToCommodityInsight(MarketItem selectedItem)
        {
            if (selectedItem == null) return;

            await Shell.Current.GoToAsync("MarketInsight", new Dictionary<string, object>()
            {
                {"SelectedMarketItem", selectedItem }
            });
        }

        //[RelayCommand]
        //private async Task NavigateToPlaceBidAsync()
        //{
        //    await Shell.Current.GoToAsync(nameof(PlacePosition), new Dictionary<string, object>
        //    {
        //        [NavigationKeys.PositionType] = PositionType.Bid
        //    });
        //}

        //[RelayCommand]
        //private async Task NavigateToPlaceOfferAsync()
        //{
        //    await Shell.Current.GoToAsync(nameof(PlacePosition), new Dictionary<string, object>
        //    {
        //        [NavigationKeys.PositionType] = PositionType.Offer
        //    });
        //}
    }
}