using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel; // For Launcher
using Microsoft.Maui.ApplicationModel.Communication; // For PhoneDialer

namespace MarketPrice.Ui.ViewModels
{
    public partial class PositionDetailViewModel : ObservableObject
    {
        // 1. Seller Info
        public string SellerName { get; set; } = "BEH CHU NELSON";
        public string SellerType { get; set; } = "Individual";
        public string SellerLocation { get; set; } = "Buea";
        public string PhoneNumber { get; set; } = "+237 670000000";

        // 2. The Initials Logic
        public string Initials
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SellerName)) return "?";
                var parts = SellerName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                // If 2 names, take first letter of each. If 1 name, take just 1 letter.
                if (parts.Length >= 2)
                    return $"{parts[0][0]}{parts[1][0]}".ToUpper();

                return $"{parts[0][0]}".ToUpper();
            }
        }

        // 3. Commodity Info
        public string CommodityName { get; set; } = "Dry Corn";
        public string Grade { get; set; } = "A";
        public string Quantity { get; set; } = "500";
        public string Price { get; set; } = "1,000";

        // 4. Details
        public string Type { get; set; } = "Dry";
        public string LotSize { get; set; } = "25kg";
        public string Code { get; set; } = "CRN";
        public string ShelfLife { get; set; } = "90days";

        // 5. Logistics
        public string Origin { get; set; } = "Buea";
        public string Destination { get; set; } = "Buea, Limbe, Douala, Yaounde";
        public string LeadTime { get; set; } = "2days";
        public string Fees { get; set; } = "2000 FCFA";
        public string PossibleDelivery { get; set; } = "Yes";

        public PositionDetailViewModel()
        {
            // In the real app, this data will come from the database
        }

        // --- COMMANDS FOR TESTING ---

        [RelayCommand]
        private void CallSeller()
        {
            if (PhoneDialer.Default.IsSupported)
            {
                // Opens the native phone dialer with the number pre-filled
                PhoneDialer.Default.Open(PhoneNumber);
            }
        }

        [RelayCommand]
        private async Task ChatWhatsApp()
        {
            if (string.IsNullOrWhiteSpace(PhoneNumber)) return;

            // WA.ME format requires the number clean (no +, no spaces)
            string cleanNumber = PhoneNumber.Replace("+", "").Replace(" ", "").Replace("-", "");

            // Opens WhatsApp directly
            await Launcher.Default.OpenAsync($"https://wa.me/{cleanNumber}");
        }
    }
}