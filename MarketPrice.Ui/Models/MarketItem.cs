using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Ui.Models
{
    public class MarketItem
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? ImageSource { get; set; }
        public decimal BestBid { get; set; }
        public decimal BestOffer { get; set; }
        public bool IsBidUp { get; set; }
        public bool IsOfferDown { get; set; }   
    }
}
