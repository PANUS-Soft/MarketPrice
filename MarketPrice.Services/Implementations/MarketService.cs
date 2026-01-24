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

        public async Task<List<MarketInsightResponseDto>> GetMarketTrendAsync()
        {        
            IQueryable<Commodity> commoditiesQuery = _context.Commodities.AsNoTracking();

            var query =
                from c in commoditiesQuery
                join p in _context.Positions
                    .AsNoTracking()
                    .Where(p => p.CurrentStatusId == StatusId)
                    on c.CommodityId equals p.CommodityId into posGroup
                orderby c.CommodityName
                select new
                {
                    c.CommodityId,
                    c.CommodityTypeId,
                    c.CommodityName,
                    CommodityImage = c.CommodityImage != null
                        ? c.CommodityImage.FileName
                        : string.Empty,

                    CommodityCode = c.CommodityType != null
                        ? c.CommodityType.Code
                        : string.Empty,

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

            var result = new List<MarketInsightResponseDto>();

            foreach (var x in data)
            {
                // 🔹 TEMP previous values (replace with snapshot later)
                decimal previousBestBid = await _context.Commodities.Where(c => c.CommodityName == x.CommodityName).MaxAsync(c => c.LastBestBid);
                decimal previousBestOffer = await _context.Commodities.Where(c => c.CommodityName == x.CommodityName).MaxAsync(c => c.LastBestOffer);

                bool isBidUp = x.BestBid > previousBestBid;
                bool isOfferDown = x.BestOffer > previousBestOffer;

                if(isBidUp) await _context.Commodities.Where(c => c.CommodityName == x.CommodityName).ExecuteUpdateAsync(c => c.SetProperty(c => c.LastBestBid, c => x.BestBid));
                if(!isOfferDown) await _context.Commodities.Where(c => c.CommodityName == x.CommodityName).ExecuteUpdateAsync(c => c.SetProperty(c => c.LastBestOffer, c => x.BestOffer));

                result.Add(new MarketInsightResponseDto
                {
                    CommodityId = x.CommodityId,
                    CommodityTypeId = x.CommodityTypeId,
                    CommodityName = x.CommodityName,
                    CommodityCode = x.CommodityCode,
                    CommodityImage = x.CommodityImage,
                    BestBid = x.BestBid,
                    BestOffer = x.BestOffer,
                    IsBidUp = isBidUp,
                    IsOfferDown = isOfferDown
                });
            }

            return result;
        }

    }
}
