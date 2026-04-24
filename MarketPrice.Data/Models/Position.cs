namespace MarketPrice.Data.Models
{
    public class Position
    {
        public Guid PositionId { get; set; }
        public Guid UserId { get; set; }
        public Guid CommodityId { get; set; }
        public required int PositionTypeId { get; set; }
        public required int CurrentStatusId { get; set; }
        public required decimal UnitPrice { get; set; }
        public required decimal Quantity { get; set; }
        public string? Grade { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public required DateTime ExpiryDate { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public DateTime? DateUpdated { get; set; }

        // Include navigation properties
        public User? User { get; set; }
        public Commodity? Commodity { get; set; }
        public LookupData? PositionType { get; set; }
        public LookupData? CurrentStatus { get; set; }

    }

}
