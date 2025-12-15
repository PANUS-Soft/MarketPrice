using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Data.Models
{
    public class Position
    {
        public Guid PositionId { get; set; }
        public Guid UserId { get; set; }
        public Guid CommodityId { get; set; }
        public required int PositionTypeId { get; set; }
        public required int CurrentStatusId { get; set; }
        public required decimal UnitPrice { get; set; }
        public required decimal Quantity { get; set; }
        public string? Grade { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset StartDate { get; set; } = DateTimeOffset.UtcNow;
        public required DateTimeOffset ExpiryDate { get; set; }
        public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? DateUpdated { get; set; }

    }







}
