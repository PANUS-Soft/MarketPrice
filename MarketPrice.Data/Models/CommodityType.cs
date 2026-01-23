namespace MarketPrice.Data.Models
{
    public class CommodityType
    {
        public Guid CommodityTypeId { get; set; }
        public int CommodityGroupId { get; set; }
        public Guid CommodityTypeImageId {  get; set; }
        public required int NameId { get; set; }
        public required string Code { get; set; }
        public Guid DefaultUnitOfMeasureId { get; set; }
        public decimal LastBestBid {  get; set; }
        public decimal LastBestOffer { get; set; }
        public DateTimeOffset DateUpdated { get; set; }

        // Adding Commodity name navigation property

        public LookupData? Name { get; set; }
        public required CommodityTypeImage CommodityTypeImage { get; set; }

    }
}
