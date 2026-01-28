using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using MarketPrice.Ui.Models;

namespace MarketPrice.Ui.ViewModels;

public partial class PositionListingViewModel : ObservableObject
{
    private List<PositionListingItem> _allPositions;

    [ObservableProperty]
    private string _pageTitle = "CORN";

    [ObservableProperty]
    private string _unitPrice = "XAF 490";

    [ObservableProperty]
    private string _commodityImage = "dry_corn.png";

    public ObservableCollection<FilterChip> Filters { get; set; }
    public ObservableCollection<PositionListingItem> Positions { get; set; }

    public PositionListingViewModel()
    {
        Filters = new ObservableCollection<FilterChip>();
        Positions = new ObservableCollection<PositionListingItem>();
        _allPositions = new List<PositionListingItem>();

        LoadData();
    }

    private void LoadData()
    {
        // 1. UPDATED FILTERS: All, Dry, Fresh
        Filters.Add(new FilterChip { Title = "All", IsSelected = true });
        Filters.Add(new FilterChip { Title = "Dry", IsSelected = false });
        Filters.Add(new FilterChip { Title = "Fresh", IsSelected = false });

        // 2. UPDATED DATA: Using "Dry Corn" and "Fresh Corn"
        var rawData = new List<PositionListingItem>
        {
            new() { Initials = "BN", UserName = "BEH NELSON", Quantity = "250 Kg", CommodityType = "Dry Corn" },
            new() { Initials = "AN", UserName = "ATANKEU NATANAHEL", Quantity = "250 Kg", CommodityType = "Fresh Corn" },
            new() { Initials = "CT", UserName = "CHESSEU TERTULLIEN", Quantity = "500 Kg", CommodityType = "Fresh Corn" },
            new() { Initials = "TE", UserName = "TCHOUANI EMMA", Quantity = "300 Kg", CommodityType = "Dry Corn" },
            new() { Initials = "LF", UserName = "LR Farms", Quantity = "1500 Kg", CommodityType = "Dry Corn" },
            new() { Initials = "RC", UserName = "RED FARM", Quantity = "500 Kg", CommodityType = "Fresh Corn" }
        };

        _allPositions.AddRange(rawData);
        foreach (var item in _allPositions) Positions.Add(item);
    }

    [RelayCommand]
    private void SelectFilter(FilterChip selectedChip)
    {
        if (selectedChip == null) return;

        // Visual update (Radio button style)
        foreach (var chip in Filters)
        {
            chip.IsSelected = (chip.Title == selectedChip.Title);
        }

        Positions.Clear();

        if (selectedChip.Title == "All")
        {
            foreach (var item in _allPositions) Positions.Add(item);
        }
        else
        {
            // Filter logic: Check if "Dry Corn" contains "Dry", etc.
            var filtered = _allPositions
                .Where(p => p.CommodityType.Contains(selectedChip.Title, StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var item in filtered) Positions.Add(item);
        }
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task GoToDetails(PositionListingItem item)
    {
        await Shell.Current.GoToAsync("CommodityDetail");
    }
}