using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketPrice.Ui.Models;
using System.Collections.ObjectModel;

namespace MarketPrice.Ui.ViewModels
{
    public partial class MarketViewModel : ObservableObject
    {
        // Holds the original data so we never "lose" items when filtering
        private readonly List<MarketItem> _allMarketItems;

        public ObservableCollection<string> CommodityTypes { get; set; }
        public ObservableCollection<MarketItem> MarketItems { get; set; }

        [ObservableProperty]
        string selectedCommodityType;

        [ObservableProperty]
        string searchText = string.Empty;

        public MarketViewModel()
        {
            CommodityTypes = new ObservableCollection<string>()
            {
                "ALL", "BEANS", "CORN", "EGUSI", "GINGER", "ONION", "OIL"
            };

            _allMarketItems = new List<MarketItem>()
            {
                new MarketItem {Name = "Fresh Corn", Code = "CRN", ImageSource = "fresh_corn.png", BestBid = 350, BestOffer = 275, IsBidUp = true, IsOfferDown = true},
                new MarketItem {Name = "Dry Corn", Code = "CRN", ImageSource = "dry_corn.png", BestBid = 375, BestOffer = 300, IsBidUp = true, IsOfferDown = true},
                new MarketItem {Name = "Red Beans", Code = "BNS", ImageSource = "red_beans.png", BestBid = 275, BestOffer = 275, IsBidUp = true, IsOfferDown = true},
                new MarketItem {Name = "Black beans", Code = "BNS", ImageSource = "black_beans.png", BestBid = 325, BestOffer = 300, IsBidUp = true, IsOfferDown = true},
                new MarketItem {Name = "White Beans", Code = "BNS", ImageSource = "white_beans.png", BestBid = 350, BestOffer = 400, IsBidUp = true, IsOfferDown = true},
                new MarketItem {Name = "Ginger", Code = "GIN", ImageSource = "ginger.png", BestBid = 250, BestOffer = 200, IsBidUp = true, IsOfferDown = true},
                new MarketItem {Name = "Palm Oil", Code = "OIL", ImageSource = "palm_oil.png", BestBid = 350, BestOffer = 375, IsBidUp = true, IsOfferDown = true},
                new MarketItem {Name = "Cracked Egusi", Code = "EGU", ImageSource = "cracked_egusi.png", BestBid = 400, BestOffer = 350, IsBidUp = true, IsOfferDown = true}
            };

            MarketItems = new ObservableCollection<MarketItem>(_allMarketItems);

            SelectedCommodityType = CommodityTypes[0];
        }

        
        partial void OnSelectedCommodityTypeChanged(string value) => ApplyFilters();

        partial void OnSearchTextChanged(string value) => ApplyFilters();

        private void ApplyFilters()
        {
            var filtered = _allMarketItems.AsEnumerable();

            // 1. Filter by Category Tab
            if (!string.IsNullOrEmpty(SelectedCommodityType) && SelectedCommodityType != "ALL")
            {
                filtered = filtered.Where(item =>
                    item.Name.ToUpper().Contains(SelectedCommodityType.ToUpper()) ||
                    item.Code.ToUpper().Contains(SelectedCommodityType.ToUpper()));
            }

            // 2. Further filter by Search Bar text
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(item =>
                    item.Name.ToUpper().Contains(SearchText.ToUpper()) ||
                    item.Code.ToUpper().Contains(SearchText.ToUpper()));
            }

            UpdateDisplayList(filtered);
        }

        private void UpdateDisplayList(IEnumerable<MarketItem> newItems)
        {
            MarketItems.Clear();
            foreach (var item in newItems)
            {
                MarketItems.Add(item);
            }
        }

        [RelayCommand]
        public async Task NavigateToPlaceBid()
        {
            await Shell.Current.GoToAsync("PlaceBid");
        }

        [RelayCommand]
        public async Task NavigateToPlaceOffer()
        {
            await Shell.Current.GoToAsync("PlaceOffer");
        }
    }
}