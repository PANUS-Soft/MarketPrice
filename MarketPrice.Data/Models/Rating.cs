using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Data.Models
{
    public class Rating
    {
        public Guid RatingId { get; set; }
        public Guid RaterUserId { get; set; }
        public Guid RatedUserId { get; set; }
        public required byte Score { get; set; }
        public string? Comment { get; set; }
        public DateTimeOffset DateRecorded { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset? DateUpdated { get; set; }
    }
}
