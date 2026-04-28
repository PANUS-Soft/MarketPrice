using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http.Json;
using MarketPrice.Domain.Home.DTOs;
using MarketPrice.Domain.Position.Commands;
using MarketPrice.Ui.Common;
using MarketPrice.Ui.Models;
using MarketPrice.Ui.Services.Api;
using MarketPrice.Ui.Services.Session;
using MarketPrice.Ui.Views;
using Microsoft.Extensions.Options;

namespace MarketPrice.Ui.ViewModels
{
    public partial class HomeViewModel(SessionService sessionService, HomeApiService homeApi, IOptions<ApiSettings> apiSettingOptions) : ObservableObject
    {
        public ObservableCollection<LoadHomeResponseDto> CommodityTypes { get; } = new();
        public ObservableCollection<HomeDisplayInformation> HomeDisplayInfo { get; } = new();

        [ObservableProperty] private ImageSource previewImage;
        [ObservableProperty] private bool isImagePreviewVisible;
        [ObservableProperty] private string selectedCommodityTypeName;

        public async Task InitializeAsync()
        {
            await LoadHomeDataAsync();
        }

        public async Task LoadHomeDataAsync()
        {
            var isSessionValid = await sessionService.ValidateAndRefreshSessionAsync();

            if (isSessionValid) await sessionService.GetCurrentSessionAsync();
            else await sessionService.TryRefreshTokenAsync();

            try
            {
                var homeDataResponse = await homeApi.LoadHomeAsync();

                if (homeDataResponse.IsSuccessStatusCode)
                {
                    CommodityTypes.Clear();
                    HomeDisplayInfo.Clear();

                    var homeData = await homeDataResponse.Content.ReadFromJsonAsync<List<LoadHomeResponseDto>>();
                    if (homeData != null)
                    {
                        var sortedHomeData = homeData
                            .OrderBy(x => x.CommodityTypeName, StringComparer.OrdinalIgnoreCase).ToList();

                        foreach (var item in sortedHomeData)
                        {
                            CommodityTypes.Add(item);
                            var data = new HomeDisplayInformation
                            {
                                CommodityTypeId = item.CommodityTypeId,
                                Name = item.CommodityTypeName!,
                                ImageSource = item.ImageUrl!,
                                BackgroundColor = Color.FromArgb("#795548"),
                                LotSize = $"{item.LotSize} {item.UnitOfMeasure}" ?? "---",
                                BestBidPrice = item.BestBidPrice != 0 ? item.BestBidPrice.ToString("N0", new System.Globalization.CultureInfo("en-CM")) : "No Bids",
                                IsBidTrendUp = item.IsBidImproved && item.BestBidPrice != 0,
                                IsBidTrendDown = !item.IsBidImproved && item.BestBidPrice != 0,
                                BestOfferPrice = item.BestOfferPrice != 0 ? item.BestOfferPrice.ToString("N0", new System.Globalization.CultureInfo("en-CM")) : "No Offers",
                                IsOfferTrendUp = item.IsOfferImproved && item.BestOfferPrice != 0,
                                IsOfferTrendDown = !item.IsOfferImproved && item.BestOfferPrice != 0,
                                IsBidSoonToExpire = item.IsBidSoonToExpire,
                                IsOfferSoonToExpire = item.IsOfferSoonToExpire
                            };

                            data.ImageSource = ImageSource.FromUri(new Uri($"{apiSettingOptions.Value.BaseUrl}{item.ImageUrl}"));

                            HomeDisplayInfo.Add(data);
                        }
                    }
                } else
                {
                    await Shell.Current.DisplayAlert("Error", "Unable to load home data.", "OK");
                }
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error", $"There was an error loading data. {e.Message}", "OK");
            }
        }

        [RelayCommand]
        private void OpenImagePreview(HomeDisplayInformation item)
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

        [RelayCommand]
        private async Task NavigateToBidPositionListing(HomeDisplayInformation item)
        {
            if (item.BestBidPrice == "No Bids") return;

            bool accessAllowed = await sessionService.EnsureUserAccessAsync();

            if (!accessAllowed)
            {
                bool accessAccount = await Shell.Current.DisplayAlert("Access Required", "You need to have an account in order to deeper explore the platform. \n\nYou can either create an account or login into an existing one", "Register or Login", "Cancel");

                if (!accessAccount) return;

                await Shell.Current.GoToAsync("//Welcome");

                return;
            }

            var args = new PositionListingCommand
            {
                CommodityTypeId = item.CommodityTypeId,
                UnitPrice = decimal.Parse(item.BestBidPrice),
                PositionTypeId = 6001
            };

            await Shell.Current.GoToAsync(nameof(PositionListing), new Dictionary<string, object>
            {
                { "Args", args }
            });
        }

        [RelayCommand]
        private async Task NavigateToOfferPositionListing(HomeDisplayInformation item)
        {
            if (item.BestOfferPrice == "No Offers") return;

            bool accessAllowed = await sessionService.EnsureUserAccessAsync();

            if (!accessAllowed)
            {
                bool accessAccount = await Shell.Current.DisplayAlert("Access Required", "You need to have an account in order to deeper explore the platform. \n\nYou can either create an account or login into an existing one", "Register or Login", "Cancel");

                if (!accessAccount) return;

                await Shell.Current.GoToAsync("//Welcome");

                return;
            }

            var args = new PositionListingCommand
            {
                CommodityTypeId = item.CommodityTypeId,
                UnitPrice = decimal.Parse(item.BestOfferPrice),
                PositionTypeId = 6002
            };

            await Shell.Current.GoToAsync(nameof(PositionListing), new Dictionary<string, object>
            {
                {"Args", args}
            });
        }
    }
}