using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Market.Dtos
{
    public class MarketDepthResponseDto
    {

        public Guid CommodityId { get; set; }
        public string CommodityName { get; set; }
        public string CommodityCode { get; set; }
        public string ImageUrl { get; set; }
        public decimal BestBid { get; set; }
        public decimal BestOffer { get; set; }

    }
    












}
