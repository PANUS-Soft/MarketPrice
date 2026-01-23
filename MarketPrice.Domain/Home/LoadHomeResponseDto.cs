using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Home
{
    public class LoadHomeResponseDto
    {
        public Guid CommodityTypeId { get; set; }
        public string Name { get; set; }
        public short LotSize { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal BestBidPrice { get; set; }
        public decimal BestOfferPrice { get; set; }
        public bool HasBidIncreased { get; set; }
        public bool HasOfferIncreased { get; set; }
    }
}
