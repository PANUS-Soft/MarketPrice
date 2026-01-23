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
using MarketPrice.Data.Models;


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

        public async Task<List<MarketInsightResponseDto>> GetMarketTrendAsync(MarketInsightCommand command)
        {
            // Validate input
            if (command == null) throw new ArgumentNullException(nameof(command));

            
            IQueryable<Commodity> commoditiesQuery = _context.Commodities.AsNoTracking();
            if (command.CommodityTypeId != Guid.Empty)
            {
                commoditiesQuery = commoditiesQuery.Where(c => c.CommodityTypeId == command.CommodityTypeId);
            }

            //if (command.CommodityTypeId == Guid.Empty) return new List<MarketDepthResponseDto>();

            // Query all commodities for the requested commodity type
            // and compute BestBid/BestOffer from positions (only open positions).
            // This returns one DTO per commodity (e.g., "Fresh Corn", "Dry Corn"). 
            
            var query = from c in commoditiesQuery
                        join p in _context.Positions
                            .AsNoTracking()
                            .Where(p => p.CurrentStatusId == StatusId)
                            on c.CommodityId equals p.CommodityId into posGroup
                        orderby c.CommodityName
                        select new MarketInsightResponseDto
                        {
                            CommodityId = c.CommodityId,
                            CommodityTypeId = c.CommodityTypeId,
                            CommodityName = c.CommodityName,
                            CommodityCode = c.CommodityType != null
                                ? c.CommodityType.Code
                                : string.Empty,
                            ImageUrl = c.ImageUrl,

                            BestBid = posGroup
                                .Where(p => p.PositionTypeId == BidPosition)
                                .Select(p => (decimal?)p.UnitPrice)
                                .Max() ?? 0m,

                            BestOffer = posGroup
                                .Where(p => p.PositionTypeId == AskPosition)
                                .Select(p => (decimal?)p.UnitPrice)
                                .Min() ?? 0m
                        };

            var data = await query.ToListAsync();

            var result = data.Select(x => new MarketInsightResponseDto
            {
                CommodityId = x.CommodityId,
                CommodityTypeId = x.CommodityTypeId,
                CommodityName = x.CommodityName,
                CommodityCode = x.CommodityCode,
                ImageUrl = x.ImageUrl,

                BestBid = x.BestBid,
                BestOffer = x.BestOffer,

                // TEMP logic (until snapshot/history exists)
                IsBidUp = false,
                IsOfferDown = false
            }).ToList();

            return result;


            //return await query.ToListAsync();

        }
    }
}
