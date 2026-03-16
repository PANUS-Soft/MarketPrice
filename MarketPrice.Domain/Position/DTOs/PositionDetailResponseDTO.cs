namespace MarketPrice.Domain.Position.DTOs
{
    public class PositionDetailResponseDto
    {
        // User Information
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        // Commodity Information
        public string CommodityName { get; set; } = string.Empty;
        public string CommodityTypeName { get; set; } = string.Empty;
        public string CommodityCode { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string UnitOfMeasure { get; set; } = string.Empty;

        // Position Information
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public short? LotSize { get; set; }
        public int? ShelfLifeInDays { get; set; }
        public bool DeliveryAvailable { get; set; }

        // Logistics Information
        public LocationResponse Origin { get; set; } = new();
        public LocationResponse? Destination { get; set; } = new();
        public string? LeadTimeInDays { get; set; }
        public decimal? DeliveryFee { get; set; }
    }

    public class LocationResponse
    {
        public string Region { get; set; } = string.Empty;
        public string Town { get; set; } = string.Empty;
        public string Quarter { get; set; } = string.Empty;
        public string? Street { get; set; }
    }
}
