using Microsoft.Extensions.Primitives;
using System.Net.NetworkInformation;

namespace MarketPrice.Data.Models
{
    public class Commodity
    {
        public Guid CommodityId { get; set; }
        public Guid CommodityTypeId { get; set; }
        public Guid CommodityImageId { get; set; }
        public Guid UnitOfMeasureId { get; set; }
        public required string CommodityName { get; set; }
        public int? ShelfLifeInDays { get; set; }
        public string? Notes { get; set; }
        public short? LotSize { get; set; }
        public decimal LastBestBid {  get; set; }
        public decimal LastBestOffer { get; set; }
        public DateTimeOffset DateUpdated { get; set; }


        //Units of measure navigation property
        public UnitOfMeasure? UnitOfMeasure { get; set; }
        public required CommodityImage CommodityImage { get; set; }
    }
}
