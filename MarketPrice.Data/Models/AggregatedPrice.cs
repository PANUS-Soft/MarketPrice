using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Data.Models
{
    public class AggregatedPrice
    {
        public long Id { get; set; }
        public Guid CommodityId { get; set; }

        // Use DateTime for easier SQL grouping/filtering
        public DateTime Timestamp { get; set; }

        // "2H", "1D", "1W", "1M"
        public required string Interval { get; set; }
        public decimal AvgBid {  get; set; }
        public decimal HighBid { get; set; }
        public decimal LowBid { get; set; }
        public decimal AvgOffer { get; set; }
        public decimal HighOffer { get; set; }
        public decimal LowOffer { get; set; }
        public int PositionCount { get; set; }
    }
}
