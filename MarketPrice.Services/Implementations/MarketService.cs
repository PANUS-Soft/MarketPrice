using MarketPrice.Services.Interfaces;
using MarketPrice.Data;
using MarketPrice.Data.Models;
using MarketPrice.Domain.Market.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MarketPrice.Services.Implementations
{
    public class MarketService : IMarketService
    {
        private readonly MarketPriceDbContext _context;

        private const int StatusId = 5001;     // Open
        private const int BidPosition = 6001;  // Bid
        private const int AskPosition = 6002;  // Offer

        public MarketService(MarketPriceDbContext context)
        {
            _context = context;
        }

        public async Task<List<MarketResponseDto>> GetMarketTrendAsync()
        {
            // 1️ Load current market state
            var marketData = await (
                from c in _context.Commodities
                join p in _context.Positions.AsNoTracking()
                        .Where(p => p.StartDate <= DateTime.UtcNow && p.ExpiryDate > DateTime.UtcNow)
                    on c.CommodityId equals p.CommodityId into posGroup
                join ci in _context.CommodityImage
                    on c.CommodityId equals ci.CommodityId into ciGroup
                from ci in ciGroup.DefaultIfEmpty()

                join uom in _context.UnitOfMeasures on c.UnitOfMeasureId equals uom.UnitOfMeasureId

                select new
                {
                    Commodity = c,

                    UnitOfMeasure = uom,

                    BestBid = posGroup
                        .Where(p => p.PositionTypeId == BidPosition)
                        .Select(p => (decimal?)p.UnitPrice)
                        .Max() ?? 0m,

                    BestOffer = posGroup
                        .Where(p => p.PositionTypeId == AskPosition)
                        .Select(p => (decimal?)p.UnitPrice)
                        .Min() ?? 0m,

                    ImageFileName = ci != null ? ci.FileName : null,
                    CommodityImageId = ci != null ? ci.CommodityImageId : Guid.Empty
                }
            ).ToListAsync();

            // 2️ Apply market improvement logic
            var response = new List<MarketResponseDto>();

            foreach (var x in marketData)
            {
                var item = x.Commodity;

                // Handle Bid Logic (higher price is better)
                if (x.BestBid > item.LastBestBid && x.BestBid > 0)
                {
                    item.IsBidImproved = true;
                    item.LastBestBid = x.BestBid;
                }
                else if(x.BestBid < item.LastBestBid && x.BestBid > 0) { 
                    item.IsBidImproved = false; 
                    item.LastBestBid = x.BestBid;
                }

                // Handle Offer Logic (lower price is better)
                if (x.BestOffer < item.LastBestOffer && x.BestOffer > 0)
                {
                    item.IsOfferImproved = true;
                    item.LastBestOffer = x.BestOffer;
                }
                else if (item.LastBestOffer == 0 && x.BestOffer > 0)
                {
                    item.IsOfferImproved = true;
                    item.LastBestOffer = x.BestOffer;
                }
                else if (x.BestOffer > item.LastBestOffer && x.BestOffer > 0)
                {
                    item.IsOfferImproved = false;
                    item.LastBestOffer = x.BestOffer;
                }

                item.DateUpdated = DateTimeOffset.Now;

                // In case GroupBy broke tracking, this ensures the update is sent.
                _context.Entry(item).State = EntityState.Modified;

                // 3 Build response DTO
                response.Add(new MarketResponseDto
                {
                    CommodityId = item.CommodityId,
                    CommodityTypeId = item.CommodityTypeId,
                    CommodityName = item.CommodityName,
                    CommodityImageId = x.CommodityImageId,
                    LotSize = item.LotSize,
                    UnitOfMeasure = x.UnitOfMeasure.UnitOfMeasureCodeEnglish,
                    ImageUrl = $"CommodityImages/{item.CommodityId}/image",

                    BestBid = x.BestBid,
                    BestOffer = x.BestOffer,

                    IsBidImproved = item.IsBidImproved,
                    IsOfferImproved = item.IsOfferImproved
                });
            }

            // 4 Single database write
            await _context.SaveChangesAsync();

            return response;
        }
    }
}
