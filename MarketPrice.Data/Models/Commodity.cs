namespace MarketPrice.Data.Models
{
    public class Commodity
    {
        public Guid CommodityId { get; set; }
        public Guid CommodityTypeId { get; set; }
        public Guid UnitOfMeasureId { get; set; }
        public required string CommodityName { get; set; }
        public byte? ShelfLifeInDays { get; set; }
        public string? Notes { get; set; }
        public short? LotSize { get; set; }

    }
}
