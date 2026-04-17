using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Ui.Models
{
    public class Activity
    {
        public string CommodityName { get; set; }
        public string Quantity { get; set; }
        public string Price { get; set; }
        public string State { get; set; }
        public Color StateColor { get; set; }
        public string ImageUrl { get; set; }
        public DateTime Date { get; set; }
        public string PositionType { get; set; } // <-- Added this (e.g., "Bid" or "Offer")
        public string UnitOfMeasure { get; set; }
        public string LotSize { get; set; }
    }

    public class ActivityGroup : List<Activity>
    {
        public string Name { get; private set; }
        public ActivityGroup(string name, List<Activity> items) : base(items) { Name = name; }
    }
}
