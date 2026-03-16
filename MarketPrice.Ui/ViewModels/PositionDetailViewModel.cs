using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevExpress.Maui.Core;
using MarketPrice.Domain.Position.DTOs;
using MarketPrice.Ui.Services.Api;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.Communication;

namespace MarketPrice.Ui.ViewModels
{
    [QueryProperty(nameof(PositionId), "positionId")]
    public partial class PositionDetailViewModel(PositionApiService positionApi) : ObservableObject
    {
        private readonly PositionApiService _positionApi = positionApi;

        [ObservableProperty] private PositionListing position;

        [ObservableProperty] private Guid positionId;

        partial void OnPositionIdChanged(Guid value)
        {
            if (value != Guid.Empty) _ = LoadPositionDetailAsync(value);
        }


        [ObservableProperty] private PositionDetailResponseDto? dto;

        public Guid UserId => Dto?.UserId ?? Guid.Empty;
        public string UserName => Dto?.UserName ?? "---";
        public string AccountType => Dto?.AccountType ?? "---";
        public string PhoneNumber => Dto?.PhoneNumber ?? "---";
        public string CommodityTypeName => Dto?.CommodityTypeName.ToUpper() ?? "---";
        public string CommodityName => Dto?.CommodityName.ToUpper() ?? "---";
        public string CommodityCode => Dto?.CommodityCode ?? "---";
        public string Grade => Dto?.Grade ?? "---";
        public string UnitOfMeasure => Dto?.UnitOfMeasure ?? "---";
        public decimal Quantity => Dto?.Quantity ?? 0;
        public string UnitPrice => Dto?.UnitPrice.ToString("N0", new System.Globalization.CultureInfo("en-CM")) ?? "---";
        public string LotSize => $"{Dto?.LotSize} {UnitOfMeasure}" ?? "---";
        public string ShelfLifeInDays => $"{Dto?.ShelfLifeInDays} days" ?? "---";
        public bool DeliveryAvailable => Dto?.DeliveryAvailable ?? false;
        public string OriginRegion => Dto?.Origin.Region ?? "---";
        public string OriginTown => Dto?.Origin.Town ?? "---";
        public string OriginQuarter => Dto?.Origin.Quarter ?? "---";
        public string OriginStreet => Dto?.Origin.Street ?? "---";
        public string DestinationRegion => Dto?.Destination?.Region ?? "---";
        public string DestinationTown => Dto?.Destination?.Town ?? "---";
        public string DestinationQuarter => Dto?.Destination?.Quarter ?? "---";
        public string DestinationStreet => Dto?.Destination?.Street ?? "---";
        public string LeadTimeInDays => $"{Dto?.LeadTimeInDays} days" ?? "---";
        public string DeliveryFee => Dto?.DeliveryFee?.ToString("N0", new System.Globalization.CultureInfo("en-CM")) ?? "---";

        private async Task LoadPositionDetailAsync(Guid id)
        {
            try
            {
                var positionDetailResponse = await _positionApi.GetPositionDetailAsync(id);

                if (!positionDetailResponse.IsSuccessStatusCode) return;

                Dto = await positionDetailResponse.Content.ReadFromJsonAsync<PositionDetailResponseDto>();

                OnPropertyChanged(string.Empty);
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error", $"There was an error loading position. {e.Message} Please try again later.", "OK");
            }
        }

        [RelayCommand]
        private async Task CallUserAsync()
        {
            if (PhoneDialer.Default.IsSupported)
            {
                try
                {
                    PhoneDialer.Default.Open(PhoneNumber);
                }
                catch (ArgumentException)
                {
                    await Shell.Current.DisplayAlert("Invalid Number", "The phone number is not valid.", "OK");
                }
                catch (Exception)
                {
                    await Shell.Current.DisplayAlert("Error", "Something went wrong while opening the dialer.", "OK");
                }
            }


            try
            {
                await Launcher.Default.OpenAsync($"tel:{PhoneNumber}");
            }
            catch(Exception)
            {
                await Shell.Current.DisplayAlert("Not Supported", "Unable to open the dialer on this device.", "OK");
            }
        }

        [RelayCommand]
        private async Task ChatOnWhatsAppAsync()
        {
            if (string.IsNullOrWhiteSpace(PhoneNumber)) return;

            string cleanedNumber = PhoneNumber.Replace("+", "").Replace(" ", "").Replace("-","");

            await Launcher.Default.OpenAsync($"https://wa.me/{cleanedNumber}");
        }

        [RelayCommand]
        private async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

    }
}
