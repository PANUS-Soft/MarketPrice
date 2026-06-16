using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System.Net.NetworkInformation;

namespace MarketPrice.Data.Models
{
    public class Commodity
    {
        public Guid CommodityId { get; set; }
        public Guid CommodityTypeId { get; set; }
        public Guid UnitOfMeasureId { get; set; }
        public required string CommodityName { get; set; }
        public int? ShelfLifeInDays { get; set; }
        public string? Notes { get; set; }
        public short? LotSize { get; set; }

        [Precision(18, 4)]
        public decimal PreviousBestBid { get; set; }

        [Precision(18, 4)]
        public decimal LastBestBid { get; set; }

        [Precision(18, 4)]
        public decimal PreviousBestOffer { get; set; }

        [Precision(18, 4)]
        public decimal LastBestOffer { get; set; }
        public required bool IsBidImproved { get; set; }
        public required bool IsOfferImproved { get; set; }
        public DateTimeOffset DateUpdated { get; set; }


        //Units of measure navigation property
        public UnitOfMeasure? UnitOfMeasure { get; set; }

    }
}