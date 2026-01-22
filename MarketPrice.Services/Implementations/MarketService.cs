using MarketPrice.Domain.Market.Commands;
using MarketPrice.Domain.Market.Dtos;
using Microsoft.EntityFrameworkCore;
using MarketPrice.Data;
using MarketPrice.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MarketPrice.Services.Implementations
{
    public class MarketService : IMarketService
    {
        private readonly int StatusId = 5001;
        private readonly int BidPosition = 6001;
        private readonly int AskPosition = 6002;
        private readonly MarketPriceDbContext _context;

        public MarketService(MarketPriceDbContext context)
        {
            _context = context;
        }

        public async Task<MarketDepthResponseDto> GetMarketTrendAsync(MarketDepthCommand command)
        {

            // We join with Commodities to filter by the Type the user clicked (e.g., Corn)
            var allPositions = await _context.Positions
                .Where(p => p.Commodity.CommodityTypeId == command.CommodityTypeId
                         && p.CurrentStatusId == StatusId) // 5001 = 'Open'
                .ToListAsync();

            var dto = new MarketDepthResponseDto();

            // Process Bids (6001)
            dto.Bids = allPositions
                .Where(p => p.PositionTypeId == BidPosition)
                .GroupBy(p => p.UnitPrice)
                .Select(group => new MarketDepthPriceLevel
                {
                    Price = group.Key,
                    //TotalQuantity = (double)group.Sum(p => p.Quantity)
                    TotalQuantity = group.Sum(p => p.Quantity),
                    CreatedAt = group.Max(p => p.Date).DateTime
                })
                .OrderByDescending(x => x.Price).ToList();

            // Process Offers (6002)
            dto.Offers = allPositions
                .Where(p => p.PositionTypeId == AskPosition)
                .GroupBy(p => p.UnitPrice)
                .Select(group => new MarketDepthPriceLevel
                {
                    Price = group.Key,
                    TotalQuantity = group.Sum(p => p.Quantity),
                    CreatedAt = group.Max(p => p.Date).DateTime
                })
                .OrderBy(x => x.Price).ToList();

            return dto;
        }
    }
}
