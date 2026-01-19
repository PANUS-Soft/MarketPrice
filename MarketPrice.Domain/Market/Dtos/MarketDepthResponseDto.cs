using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Market.Dtos
{
    public class MarketDepthResponseDto
    {
        public List<MarketDepthPriceLevel> Bids { get; set; } = new();
        public List<MarketDepthPriceLevel> Offers { get; set; } = new();

        // Logic: Best bid is the highest price in the bids list
        //        Best offer is the lowest price in the offers list

        public decimal? BestBid => Bids.Any() ? Bids.Max(bid => bid.Price) : 0;
        public decimal? BestOffer => Offers.Any() ? Offers.Min(offer => offer.Price) : 0;

        // Logic: LowBid is the lowest price in the bids list
        //        LowOffer is the highest price in the offers list
        public decimal? LowBid => Bids.Any() ? Bids.Min(bid => bid.Price) : 0;
        public decimal? HighOffer => Offers.Any() ? Offers.Max(offer => offer.Price) : 0;
    }

    public class MarketDepthPriceLevel
    {
        public decimal Price { get; set; }
        public decimal TotalQuantity { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
