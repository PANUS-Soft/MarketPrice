namespace MarketPrice.Domain.Position.DTOs
{
    public class PositionDetailResponseDto
    {
        // User Information
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty; // Individual
        public string PhoneNumber { get; set; } = string.Empty;

        // Commodity
        public string CommodityName { get; set; } = string.Empty;
        public string CommodityTypeName { get; set; } = string.Empty;
        public string CommodityCode { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;

        // Position
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LotSize { get; set; }
        public string ShelfLifeInDays { get; set; } = string.Empty;

        public bool DeliveryAvailable { get; set; }

        // Logistics
        public LocationResponse Origin { get; set; } = new();
        public LocationResponse Destination { get; set; } = new();
        public int LeadTimeDays { get; set; }
        public decimal DeliveryFee { get; set; }
    }

    public class LocationResponse
    {
        public string Region { get; set; } = string.Empty;
        public string Town { get; set; } = string.Empty;
        public string Quarter { get; set; } = string.Empty;
        public string? Street { get; set; }
    }
}
