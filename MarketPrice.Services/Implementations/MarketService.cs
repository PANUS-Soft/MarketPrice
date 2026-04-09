using MarketPrice.Services.Interfaces;
using MarketPrice.Data;
using MarketPrice.Data.Models;
using MarketPrice.Domain;
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
            var now = DateTime.UtcNow;

            // 1️ Load current market state
            var marketData = await (
                from c in _context.Commodities
                join p in _context.Positions.AsNoTracking()
                        .Where(p => p.StartDate <= now && p.ExpiryDate > now)
                    on c.CommodityId equals p.CommodityId into posGroup
                join ci in _context.CommodityImage
                    on c.CommodityId equals ci.CommodityId into ciGroup
                from ci in ciGroup.DefaultIfEmpty()

                join uom in _context.UnitOfMeasures on c.UnitOfMeasureId equals uom.UnitOfMeasureId

                select new
                {
                    Commodity = c,

                    UnitOfMeasure = uom,

                    BestBidPosition = posGroup
                        .Where(p => p.PositionTypeId == BidPosition)
                        .OrderByDescending(p => p.UnitPrice)
                        .FirstOrDefault(),

                    BestOfferPosition = posGroup
                        .Where(p => p.PositionTypeId == AskPosition)
                        .OrderBy(p => p.UnitPrice)
                        .FirstOrDefault(),

                    ImageFileName = ci != null ? ci.FileName : null,
                    CommodityImageId = ci != null ? ci.CommodityImageId : Guid.Empty
                }
            ).ToListAsync();



            // 2️ Apply market improvement logic
            var response = new List<MarketResponseDto>();

            foreach (var x in marketData)
            {
                var item = x.Commodity;
                var bestBid = x.BestBidPosition?.UnitPrice ?? 0m;
                var bestOffer = x.BestOfferPosition?.UnitPrice ?? 0m;
                bool isSoonToExpire = false;

                // Prioritize the Bid if available, otherwise check the Offer
                var displayPosition = x.BestBidPosition ?? x.BestOfferPosition;

                if (displayPosition != null)
                {
                    var totalDuration = displayPosition.ExpiryDate - displayPosition.StartDate;
                    var elapsedDuration = now - displayPosition.StartDate;
                    
                    if(totalDuration.TotalSeconds > 0)
                    {
                        isSoonToExpire = (elapsedDuration.TotalSeconds / totalDuration.TotalSeconds) >= 0.8;
                    }
                }

                // Handle Bid Logic (higher price is better)
                if (bestBid > item.LastBestBid && bestBid > 0)
                {
                    item.IsBidImproved = true;
                    item.LastBestBid = bestBid;
                }
                else if (bestBid < item.LastBestBid && bestBid > 0)
                {
                    item.IsBidImproved = false;
                    item.LastBestBid = bestBid;
                }

                // Handle Offer Logic (lower price is better)
                if (bestOffer < item.LastBestOffer && bestOffer > 0)
                {
                    item.IsOfferImproved = true;
                    item.LastBestOffer = bestOffer;
                }
                else if (item.LastBestOffer == 0 && bestOffer > 0)
                {
                    item.IsOfferImproved = true;
                    item.LastBestOffer = bestOffer;
                }
                else if (bestOffer > item.LastBestOffer && bestOffer > 0)
                {
                    item.IsOfferImproved = false;
                    item.LastBestOffer = bestOffer;
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
                    ImageUrl = $"{ApiControllers.CommodityImages}/{item.CommodityId}/image",

                    BestBid = bestBid,
                    BestOffer = bestOffer,

                    IsBidImproved = item.IsBidImproved,
                    IsOfferImproved = item.IsOfferImproved,
                    IsSoonToExpire = isSoonToExpire
                });
            }

            // 4 Single database write
            await _context.SaveChangesAsync();

            return response;
        }

        // Logic for Market Insight
        public async Task<MarketInsightResponseDto> GetMarketInsightAsync(Guid commodityId)
        {
            const int BUY = 6001;
            const int SELL = 6002;

            var commodity = await _context.Commodities.FindAsync(commodityId);
            if (commodity == null) return null;

            var now = DateTime.UtcNow;
            var since24h = now.AddHours(-24);

            // 1. ACTIVE QUERIES (For Market Depth and Best Prices)
            var activeBidsQuery = _context.Positions.Where(p =>
                p.CommodityId == commodityId &&
                p.PositionTypeId == BUY &&
                p.StartDate <= now && p.ExpiryDate > now);

            var activeOffersQuery = _context.Positions.Where(p =>
                p.CommodityId == commodityId &&
                p.PositionTypeId == SELL &&
                p.StartDate <= now && p.ExpiryDate > now);

            // Slider Functionality 
            var totalBidValue = await activeBidsQuery.SumAsync(p => (decimal?)p.UnitPrice * p.Quantity) ?? 0;
            var totalOfferValue = await activeOffersQuery.SumAsync(p => (decimal?)p.UnitPrice * p.Quantity) ?? 0;

            // Total Market Value (tmv)
            var tmv = totalBidValue + totalOfferValue;

            decimal bidPercent = 0;
            decimal offerPercent = 0;

            if (tmv > 0)
            {
                bidPercent = (totalBidValue / tmv) * 100;
                offerPercent = (totalOfferValue / tmv) * 100;

            }

            // 2. 24h HISTORICAL QUERIES (For Max/Min stats)
            // This looks at all positions placed in the last 24 hours, regardless of expiry.
            var bids24hQuery = _context.Positions.Where(p =>
                p.CommodityId == commodityId &&
                p.PositionTypeId == BUY &&
                p.StartDate < now && p.ExpiryDate > since24h);

            var offers24hQuery = _context.Positions.Where(p =>
                p.CommodityId == commodityId &&
                p.PositionTypeId == SELL &&
                p.StartDate < now && p.ExpiryDate > since24h);

            // 3. Market Depth from ACTIVE positions
            var bidsDepth = await activeBidsQuery
                .GroupBy(p => p.UnitPrice)
                .Select(g => new MarketDepthItemDto
                {
                    Price = g.Key,
                    Quantity = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.Price)
                .Take(15)
                .ToListAsync();

            var offersDepth = await activeOffersQuery
                .GroupBy(p => p.UnitPrice)
                .Select(g => new MarketDepthItemDto
                {
                    Price = g.Key,
                    Quantity = g.Sum(x => x.Quantity)
                })
                .OrderBy(x => x.Price)
                .Take(15)
                .ToListAsync();


            // 4. Compose Response
            return new MarketInsightResponseDto
            {
                CommodityTypeId = commodity.CommodityTypeId,
                CommodityId = commodityId,
                CommodityName = commodity.CommodityName,

                // Best prices come from ACTIVE market depth
                BestBid = bidsDepth.FirstOrDefault()?.Price ?? 0,
                BestOffer = offersDepth.FirstOrDefault()?.Price ?? 0,

                // Max/Min stats come from 24H HISTORICAL query
                MaxBid24H = await bids24hQuery.MaxAsync(p => (decimal?)p.UnitPrice) ?? 0,
                MinBid24H = await bids24hQuery.MinAsync(p => (decimal?)p.UnitPrice) ?? 0,
                MaxOffer24H = await offers24hQuery.MaxAsync(p => (decimal?)p.UnitPrice) ?? 0,
                MinOffer24H = await offers24hQuery.MinAsync(p => (decimal?)p.UnitPrice) ?? 0,

                // Slider Values
                TotalMarketValue = tmv,
                BidPercentage = Math.Round(bidPercent, 2),
                OfferPercentage = Math.Round(offerPercent, 2),

                // market  depth
                Bids = bidsDepth,
                Offers = offersDepth
            };
        }

        public async Task<List<MarketInsightChartResponseDto>> GetPriceChartAsync(Guid commodityId, string range)
        {
            string searchRange = range.ToUpper();
            // 1. Map the ranges to their respective intervals and look back windows
            (string interval, DateTime startDate, int minPoints) = searchRange  switch
            {
                "1D" => ("1m", DateTime.UtcNow.AddDays(-1), 12),  //60      
                "1W" => ("1D", DateTime.UtcNow.AddDays(-7), 7),  //82
                "1M" => ("1D", DateTime.UtcNow.AddMonths(-1), 30), 
                "1Y" => ("1W", DateTime.UtcNow.AddYears(-1), 52),  
                _ => ("1D", DateTime.UtcNow.AddMonths(-1), 15)
            };

            // 2. Base Query
            var query =  _context.AggregatedPrices
                .AsNoTracking()
                .Where(ap => ap.CommodityId == commodityId && ap.Interval == interval);

            // 3. Try to get data for the full time range
            var results = await query
                .Where(ap => ap.Timestamp >= startDate)
                .OrderBy(ap => ap.Timestamp)
                .Select(ap => new MarketInsightChartResponseDto
                {
                    CommodityId = ap.CommodityId,
                    Interval = ap.Interval,
                    Timestamp = ap.Timestamp,
                    AvgBid = ap.AvgBid,
                    HighBid = ap.HighBid,
                    LowBid = ap.LowBid,
                    AvgOffer = ap.AvgOffer,
                    HighOffer = ap.HighOffer,
                    LowOffer = ap.LowOffer,
                    PositionCount = ap.PositionCount,


                })
                .ToListAsync();

            // 4. Fallback: If the range is too empty (e.g., during a backfill), 
            // grab the most recent 'minPoints' regardless of the startDate.
            if (results.Count < 2)
            {
                results = await query
                    .OrderByDescending(ap => ap.Timestamp)
                    .Take(minPoints)
                    .Select(ap => new MarketInsightChartResponseDto
                    {
                        CommodityId = ap.CommodityId,
                        Interval = ap.Interval,
                        Timestamp = ap.Timestamp,
                        AvgBid = ap.AvgBid,
                        HighBid = ap.HighBid,
                        LowBid = ap.LowBid,
                        AvgOffer = ap.AvgOffer,
                        HighOffer = ap.HighOffer,
                        LowOffer = ap.LowOffer,
                        PositionCount = ap.PositionCount,

                    })
                    .OrderBy(dto => dto.Timestamp) // Important: Re-sort for the chart UI
                    .ToListAsync();
            }

            return results;
        }
    }
}
