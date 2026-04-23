using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MarketPrice.Domain.Market.DTOs;
using MarketPrice.Services.Interfaces;
using MarketPrice.Data;

namespace MarketPrice.Services.Implementations
{
    public class PriceService(MarketPriceDbContext context) : IPriceService
    {
        public async Task<List<MarketInsightChartResponseDto>> GetPriceAsync(Guid commodityId,string interval, DateTime? from,DateTime? to)
        {
            // ✅ Normalize interval
            interval = interval.ToUpper();

            var validIntervals = new[] { "1M", "1D", "1W", "1MO", "1Y" };

            if (!validIntervals.Contains(interval))
                throw new ArgumentException("Invalid interval");

            // ✅ Default range protection
            if (!from.HasValue && !to.HasValue)
            {
                from = DateTime.UtcNow.AddDays(-7);
            }

            var query = context.AggregatedPrices
                .AsNoTracking()
                .Where(x => x.CommodityId == commodityId &&
                            x.Interval == interval);

            if (from.HasValue)
                query = query.Where(x => x.Timestamp >= from.Value);

            if (to.HasValue)
                query = query.Where(x => x.Timestamp <= to.Value);

            return await query
                .OrderBy(x => x.Timestamp)
                .Select(x => new MarketInsightChartResponseDto
                {
                    CommodityId = x.CommodityId,
                    Timestamp = x.Timestamp,
                    Interval = x.Interval,

                    AvgBid = x.AvgBid,
                    HighBid = x.HighBid,
                    LowBid = x.LowBid,

                    AvgOffer = x.AvgOffer,
                    HighOffer = x.HighOffer,
                    LowOffer = x.LowOffer,

                    PositionCount = x.PositionCount
                })
                .ToListAsync();
        }



    }
}
