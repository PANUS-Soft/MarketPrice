namespace MarketPrice.Data.Models
{
    public class CommodityType
    {
        public Guid CommodityTypeId { get; set; }
        public int CommodityGroupId { get; set; }
        public required int NameId { get; set; }
        public required string Code { get; set; }
        public Guid DefaultUnitOfMeasureId { get; set; }

    }
}
