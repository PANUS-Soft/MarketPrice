namespace MarketPrice.Domain.Reference
{
    public class CommodityDto
    {
        public Guid Id { get; set; }
        public Guid CommodityTypeId { get; set; }
        public int? ShelfLifeInDays { get; set; }
        public short? LotSize { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
