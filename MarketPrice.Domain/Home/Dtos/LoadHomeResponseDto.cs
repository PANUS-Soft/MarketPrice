using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Home.Dtos
{
    public  class LoadHomeResponseDto
    {
        public Guid CommodityTypeId { get; set; }
        public string ?CommodityTypeName { get; set; }
        public Guid CommodityTypeImageId { get; set; }
        public string? ImageUrl { get; set; } 
        public decimal LotSize { get; set; }
        public string? UnitOfMeasure { get; set; }
        public decimal BestBidPrice { get; set; }
        public decimal BestOfferPrice { get; set; }
        public bool IsBidImproved { get; set; }
        public bool IsOfferImproved { get; set; }
    }
}
