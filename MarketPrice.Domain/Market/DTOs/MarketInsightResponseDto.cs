using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Market.DTOs
{
    public class MarketInsightResponseDto
    {
        public Guid CommodityId { get; set; }
        public string CommodityName { get; set; }
        public decimal BestBid { get; set; }
        public decimal BestOffer { get; set; }
        public decimal MaxBid24h { get; set; }
        public decimal MaxOffer24h { get; set; }
        public decimal MinBid24h { get; set; }
        public decimal MinOffer24h { get; set; }
        public List<MarketDepthItemDto> Bids {get; set;}
        public List<MarketDepthItemDto> Offers { get; set;}
    }

    public class MarketDepthItemDto
    {
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
    }
}
