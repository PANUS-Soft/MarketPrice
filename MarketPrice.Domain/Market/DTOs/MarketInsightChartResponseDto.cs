using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Market.DTOs
{
    public class MarketInsightChartResponseDto
    {
        public Guid CommodityId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Interval { get; set; }

        // Bid Data
        public decimal AvgBid { get; set; }
        public decimal HighBid { get; set; }
        public decimal LowBid { get; set; }

        // Offer Data
        public decimal AvgOffer { get; set; }
        public decimal HighOffer { get; set; }
        public decimal LowOffer { get; set; }

        public int PositionCount { get; set; }
    }
}
