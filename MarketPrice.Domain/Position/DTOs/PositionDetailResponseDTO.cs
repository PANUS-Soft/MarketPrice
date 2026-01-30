namespace MarketPrice.Domain.Position.DTOs
{
    public class PositionDetailResponseDto
    {
        // Seller
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty; // Individual
        public string Location { get; set; } = string.Empty;

        // Commodity
        public string CommodityName { get; set; } = string.Empty;
        public string CommodityCode { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;

        // Position
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LotSize { get; set; }
        public string ShelfLife { get; set; } = string.Empty;

        // Delivery
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public int LeadTimeDays { get; set; }
        public decimal DeliveryFee { get; set; }
        public bool DeliveryAvailable { get; set; }

        // Contact
        public string PhoneNumber { get; set; } = string.Empty;



    }
}
