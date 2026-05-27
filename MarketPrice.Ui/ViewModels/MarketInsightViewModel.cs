using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketPrice.Domain.Market.DTOs;
using MarketPrice.Domain.Position.Commands;
using MarketPrice.Domain.Reference.DTOs;
using MarketPrice.Ui.Common;
using MarketPrice.Ui.Models;
using MarketPrice.Ui.Services.Api;
using MarketPrice.Ui.Views;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Windows.Input;

namespace MarketPrice.Ui.ViewModels
{
    [QueryProperty(nameof(SelectedMarketItemFilter), "SelectedMarketItemFilter")]
    public partial class MarketInsightViewModel : ObservableObject
    {
        private readonly ReferenceDataApiService _referenceDataApi;
        private readonly MarketApiService _marketApi;

        [ObservableProperty] MarketItemFilter? selectedMarketItemFilter;
        private MarketItemFilter? _incomingMarketItemFilter;

        public ObservableCollection<MarketItemFilter> Commodities { get; } = new();

        [ObservableProperty] private MarketInsightResponseDto? dto;
        public ObservableCollection<MarketInsightChartResponseDto> PriceHistory { get; } = new();

        // Properties defined to make the graph functionality dynamic
        
        private string _selectedRange = "1D";
        public string SelectedRange
        {
            get => _selectedRange;
            set
            {
                if (_selectedRange != value)
                {
                    _selectedRange = value;
                    OnPropertyChanged();

                    UpdateAxisFormat();
                }
            }
        }

        private string _axisFormat = "HH:mm";
        public string AxisFormat
        {
            get => _axisFormat;
            set
            {
                _axisFormat = value;
                OnPropertyChanged();
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public Guid CurrentCommodityId { get; set; }

        private bool _autoRefreshStarted;

        public ICommand ChangeRangeCommand { get; }


        public MarketInsightViewModel(ReferenceDataApiService referenceDataApiService, MarketApiService marketApiService)
        {
            _referenceDataApi = referenceDataApiService;
            _marketApi = marketApiService;

            ChangeRangeCommand = new Command<string>(async (range) =>
            {
                SelectedRange = range;

                await LoadChartDataAsync(CurrentCommodityId);
            });
        }

        public async Task InitializeAsync()
        {
            await LoadCommoditiesFilterAsync();

            ApplySelectedCommodity();
        }

        partial void OnSelectedMarketItemFilterChanged(MarketItemFilter? value)
        {
            if (value == null) return;

            _incomingMarketItemFilter = value;

            if (Commodities.Count > 0) ApplySelectedCommodity();
        }

        private void ApplySelectedCommodity()
        {
            if (_incomingMarketItemFilter == null) return;

            SelectedMarketItemFilter =
                Commodities.FirstOrDefault(c => c.CommodityId == _incomingMarketItemFilter.CommodityId);

            if (SelectedMarketItemFilter != null) _ = LoadMarketInsightAsync(SelectedMarketItemFilter.CommodityId);
        }

        //partial void OnSelectedMarketItemFilterChanged(MarketItemFilter? value)
        //{
        //    if (value != null) _ = LoadMarketInsightAsync(value.CommodityId);
        //}

        private async Task LoadMarketInsightAsync(Guid commodityId)
        {
            CurrentCommodityId = commodityId;
            await Task.WhenAll(GetCommodityMarketInsightAsync(commodityId), LoadChartDataAsync(commodityId));
            StartAutoRefresh();
        }

        public string CommodityName => Dto?.CommodityName.ToUpper() ?? "---";
        public string BestBid => Dto?.BestBid.ToString("N0", new System.Globalization.CultureInfo("en-CM")) ?? "---";
        public string BestOffer => Dto?.BestOffer.ToString("N0", new System.Globalization.CultureInfo("en-CM")) ?? "---";
        public string MaxBid24H => Dto?.MaxBid24H.ToString("N0", new System.Globalization.CultureInfo("en-CM")) ?? "---";
        public string MinBid24H => Dto?.MinBid24H.ToString("N0", new System.Globalization.CultureInfo("en-CM")) ?? "---";
        public string MaxOffer24H => Dto?.MaxOffer24H.ToString("N0", new System.Globalization.CultureInfo("en-CM")) ?? "---";
        public string MinOffer24H => Dto?.MinOffer24H.ToString("N0", new System.Globalization.CultureInfo("en-CM")) ?? "---";
        public decimal? BidPercentage => Dto?.BidPercentage;
        public decimal? OfferPercentage => Dto?.OfferPercentage;
        public List<MarketDepthItemDto> Bids => Dto?.Bids ?? new List<MarketDepthItemDto>();
        public List<MarketDepthItemDto> Offers => Dto?.Offers ?? new List<MarketDepthItemDto>();

        public GridLength BidWidth => new GridLength((double)(BidPercentage ?? 0), GridUnitType.Star);
        public GridLength OfferWidth => new GridLength((double)(OfferPercentage ?? 0), GridUnitType.Star);

        private async Task GetCommodityMarketInsightAsync(Guid id)
        {
            try
            {
                var marketInsightResponse = await _marketApi.GetCommodityMarketInsightAsync(id);

                if (!marketInsightResponse.IsSuccessStatusCode) return;

                Dto = await marketInsightResponse.Content.ReadFromJsonAsync<MarketInsightResponseDto>();

                OnPropertyChanged(string.Empty);
            }
            catch(Exception e)
            {
                await Shell.Current.DisplayAlert("Error", $"Something went wrong while loading market insight. {e.Message} Please try again later.", "OK");
            }
        }

        private async Task LoadChartDataAsync(Guid commodityId)
        {
            try
            {
                IsLoading = true;
                var response = await _marketApi.GetChartDataAsync(commodityId, SelectedRange);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<MarketChartDataWrapper>();

                    if (result?.Data == null || !result.Data.Any())
                    {
                        PriceHistory.Clear();
                        return;
                    }
                    PriceHistory.Clear();
                    foreach (var point in result.Data.OrderBy(x => x.Timestamp))
                    {
                        PriceHistory.Add(point);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"API Error: {ex.Message}");
            }
            finally
                {
                    IsLoading = false;
            }
        }

        // Method used to update the TimeAxis format based on the selected range. This is to ensure that the graph remains readable and appropriately formatted for different time ranges.

        private void UpdateAxisFormat()
        {
            AxisFormat = SelectedRange switch
            {
                "1D" => "HH:mm",
                "1W" => "ddd",
                "1M" => "dd MMM",
                "1Y" => "MMM yyyy",
                _ => "dd/MM"
            };
        }


        // Method used to start the auto-refresh timer for updating the chart data every 30 seconds.
        private void StartAutoRefresh()
        {
            if (_autoRefreshStarted)
                return;
            _autoRefreshStarted = true;

            Application.Current.Dispatcher.StartTimer(TimeSpan.FromSeconds(30), () =>
            {
                _ = LoadChartDataAsync(CurrentCommodityId);

                return true;
            });
        }
        private async Task LoadCommoditiesFilterAsync()
        {
            try
            {
                var response = await _referenceDataApi.GetCommoditiesAsync();
                if (response.IsSuccessStatusCode)
                {
                    var commodities = await response.Content.ReadFromJsonAsync<List<CommodityDto>>();
                    Commodities.Clear();
                    foreach (var commodity in commodities!)
                    {
                        Commodities.Add(new MarketItemFilter
                        {
                            CommodityTypeId = commodity.CommodityTypeId,
                            CommodityId = commodity.Id,
                            Name = commodity.Name.ToUpper()
                        });
                    }
                }
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error", $"Something went wrong while loading commodities. {e.Message} Please try again later.", "OK");
            }
        }

        [RelayCommand]
        private async Task BackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task NavigateToPlaceBidAsync()
        {
            await Shell.Current.GoToAsync(nameof(PlacePosition), new Dictionary<string, object>
            {
                [NavigationKeys.PositionType] = PositionType.Bid
            });
        }

        [RelayCommand]
        private async Task NavigateToPlaceOfferAsync()
        {
            await Shell.Current.GoToAsync(nameof(PlacePosition), new Dictionary<string, object>
            {
                [NavigationKeys.PositionType] = PositionType.Offer
            });
        }

        [RelayCommand]
        private async Task NavigateToBidPositionListingAsync(MarketDepthItemDto item)
        {
            var args = new PositionListingCommand
            {
                CommodityTypeId = Dto!.CommodityTypeId,
                CommodityId = Dto?.CommodityId,
                CommodityName = Dto?.CommodityName,
                PositionTypeId = 6001,
                UnitPrice = item.Price

            };

            await Shell.Current.GoToAsync(nameof(PositionListing), new Dictionary<string, object>
            {
                { "Args", args }
            });
        }

        [RelayCommand]
        private async Task NavigateToOfferPositionListingAsync(MarketDepthItemDto item)
        {
            var args = new PositionListingCommand
            {
                CommodityTypeId = Dto!.CommodityTypeId,
                CommodityId = Dto?.CommodityId,
                CommodityName = Dto?.CommodityName,
                PositionTypeId = 6002,
                UnitPrice = item.Price
            };

            await Shell.Current.GoToAsync(nameof(PositionListing), new Dictionary<string, object>
            {
                { "Args", args }
            });
        }

    }

    public partial class MarketDepthItem
    {
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }

    }
}
