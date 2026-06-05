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
        // --- Dependencies ---
        private readonly ReferenceDataApiService _referenceDataApi;
        private readonly MarketApiService _marketApi;

        // --- Fields & States ---
        [ObservableProperty]
        private MarketItemFilter? selectedMarketItemFilter;
        private MarketItemFilter? _incomingMarketItemFilter;
        private bool _autoRefreshStarted;

        // --- Core UI Collections ---
        public ObservableCollection<MarketItemFilter> Commodities { get; } = new();
        public ObservableCollection<MarketInsightChartResponseDto> PriceHistory { get; } = new();

        // --- Core Market Data Payload ---
        [ObservableProperty]
        private MarketInsightResponseDto? dto;

        // --- Dynamic Graph Configurations ---
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

        // --- Interactivity Commands ---
        public ICommand ChangeRangeCommand { get; }

        // --- Constructor ---
        public MarketInsightViewModel(ReferenceDataApiService referenceDataApiService, MarketApiService marketApiService)
        {
            _referenceDataApi = referenceDataApiService ?? throw new ArgumentNullException(nameof(referenceDataApiService));
            _marketApi = marketApiService ?? throw new ArgumentNullException(nameof(marketApiService));

            ChangeRangeCommand = new Command<string>(async (range) =>
            {
                SelectedRange = range;
                await LoadChartDataAsync(CurrentCommodityId);
            });
        }

        // --- Lifecycle Initialization ---
        public async Task InitializeAsync()
        {
            await LoadCommoditiesFilterAsync();
            ApplySelectedCommodity();
        }

        // --- Business Logic & Data Ingestion Pipelines ---
        partial void OnSelectedMarketItemFilterChanged(MarketItemFilter? value)
        {
            if (value == null) return;

            _incomingMarketItemFilter = value;
            if (Commodities.Count > 0) ApplySelectedCommodity();
        }

        private void ApplySelectedCommodity()
        {
            if (_incomingMarketItemFilter == null) return;

            SelectedMarketItemFilter = Commodities.FirstOrDefault(c => c.CommodityId == _incomingMarketItemFilter.CommodityId);

            if (SelectedMarketItemFilter != null)
            {
                _ = LoadMarketInsightAsync(SelectedMarketItemFilter.CommodityId);
            }
        }

        private async Task LoadMarketInsightAsync(Guid commodityId)
        {
            CurrentCommodityId = commodityId;

            // Optimization: Execute both independent domain network operations in parallel
            await Task.WhenAll(GetCommodityMarketInsightAsync(commodityId), LoadChartDataAsync(commodityId));

            StartAutoRefresh();
        }

        // --- Computed Properties for Explicit View Bindings ---
        public string CommodityName => Dto?.CommodityName.ToUpper() ?? "---";
        public string BestBid => Dto?.BestBid.ToString("N0", new System.Globalization.CultureInfo("en-CM")) ?? "---";
        public string BestOffer => Dto?.BestOffer.ToString("N0", new System.Globalization.CultureInfo("en-CM")) ?? "---";
        public string MaxBid24H => Dto?.MaxBid24H.ToString("N0", new System.Globalization.CultureInfo("en-CM")) ?? "---";
        public string MinBid24H => Dto?.MinBid24H.ToString("N0", new System.Globalization.CultureInfo("en-CM")) ?? "---";
        public string MaxOffer24H => Dto?.MaxOffer24H.ToString("N0", new System.Globalization.CultureInfo("en-CM")) ?? "---";
        public string MinOffer24H => Dto?.MinOffer24H.ToString("N0", new System.Globalization.CultureInfo("en-CM")) ?? "---";
        public decimal? BidPercentage => Dto?.BidPercentage;
        public decimal? OfferPercentage => Dto?.OfferPercentage;

        // Perfectly synchronized names to match your modified XAML changes (Bids & Offers)
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

                if (Dto?.Bids != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Bids Count: {Dto.Bids.Count}");

                    foreach (var bid in Dto.Bids)
                    {
                        System.Diagnostics.Debug.WriteLine(
                            $"Bid Price={bid.Price} PositionCount={bid.PositionCount}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Bids is NULL");
                }

                if (Dto?.Offers != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Offers Count: {Dto.Offers.Count}");

                    foreach (var offer in Dto.Offers)
                    {
                        System.Diagnostics.Debug.WriteLine(
                            $"Offer Price={offer.Price} PositionCount={offer.PositionCount}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Offers is NULL");
                }



                NotifyMarketInsightDataChanged();
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error", $"Something went wrong while loading market insight. {e.Message} Please try again later.", "OK");
            }
        }

        private void NotifyMarketInsightDataChanged()
        {
            OnPropertyChanged(nameof(CommodityName));
            OnPropertyChanged(nameof(BestBid));
            OnPropertyChanged(nameof(BestOffer));
            OnPropertyChanged(nameof(MaxBid24H));
            OnPropertyChanged(nameof(MinBid24H));
            OnPropertyChanged(nameof(MaxOffer24H));
            OnPropertyChanged(nameof(MinOffer24H));
            OnPropertyChanged(nameof(BidPercentage));
            OnPropertyChanged(nameof(OfferPercentage));
            OnPropertyChanged(nameof(BidWidth));
            OnPropertyChanged(nameof(OfferWidth));

            // Triggers the depth tables collection elements explicitly
            OnPropertyChanged(nameof(Bids));
            OnPropertyChanged(nameof(Offers));
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

                    PriceHistory.Clear();
                    if (result?.Data != null && result.Data.Any())
                    {
                        foreach (var point in result.Data.OrderBy(x => x.Timestamp))
                        {
                            PriceHistory.Add(point);
                        }
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

        private void StartAutoRefresh()
        {
            if (_autoRefreshStarted) return;
            _autoRefreshStarted = true;

            Application.Current?.Dispatcher.StartTimer(TimeSpan.FromSeconds(30), () =>
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

                    if (commodities != null)
                    {
                        foreach (var commodity in commodities)
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
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error", $"Something went wrong while loading commodities. {e.Message} Please try again later.", "OK");
            }
        }

        // --- Navigation Actions ---
        [RelayCommand]
        private async Task BackAsync() => await Shell.Current.GoToAsync("..");

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
            if (item == null || Dto == null) return;

            var args = new PositionListingCommand
            {
                CommodityTypeId = Dto.CommodityTypeId,
                CommodityId = Dto.CommodityId,
                CommodityName = Dto.CommodityName,
                PositionTypeId = 6001, // Domain specific Bid identification
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
            if (item == null || Dto == null) return;

            var args = new PositionListingCommand
            {
                CommodityTypeId = Dto.CommodityTypeId,
                CommodityId = Dto.CommodityId,
                CommodityName = Dto.CommodityName,
                PositionTypeId = 6002, // Domain specific Offer identification
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