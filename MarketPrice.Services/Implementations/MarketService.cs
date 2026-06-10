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

            // We join all tables first to get a simple, translatable list
            var flatData = await (
    from c in _context.Commodities
    join uom in _context.UnitOfMeasures on c.UnitOfMeasureId equals uom.UnitOfMeasureId

    join p in _context.Positions.AsNoTracking()
        on c.CommodityId equals p.CommodityId into pGroup
    from p in pGroup.Where(pos => pos.StartDate <= now && pos.ExpiryDate > now).DefaultIfEmpty()

    join dd in _context.DeliveryDetails
        on (p == null ? (Guid?)null : p.PositionId) equals dd.PositionId into ddGroup
    from dd in ddGroup.DefaultIfEmpty()

    join loc in _context.Locations
        on (dd == null ? (Guid?)null : dd.OriginLocationId) equals loc.LocationId into locGroup
    from loc in locGroup.DefaultIfEmpty()

    join ld in _context.LookupData
        on (loc == null ? -1 : loc.RegionId) equals ld.LookupDataId into ldGroup
    from ld in ldGroup.Where(l => l.LookupDataTypeId == 7000).DefaultIfEmpty()

    join ci in _context.CommodityImage on c.CommodityId equals ci.CommodityId into ciGroup
    from ci in ciGroup.DefaultIfEmpty()

    select new
    {
        c.CommodityId,
        c.CommodityTypeId,
        c.CommodityName,
        c.LotSize,
        c.LastBestBid,
        c.LastBestOffer,
        CommodityEntity = c,
        UnitOfMeasureCode = uom.UnitOfMeasureCodeEnglish,
        CommodityImageId = ci != null ? ci.CommodityImageId : Guid.Empty,

        Position = p,
        LocationName = ld != null ? ld.LookupDataTextEnglish : "N/A"
    }
).ToListAsync();
            // 2. Group the data in-memory (Client-side)
            var groupedData = flatData
                .GroupBy(x => x.CommodityId)
                .ToList();

            var response = new List<MarketResponseDto>();

            foreach (var group in groupedData)
            {
                // Take the first item to get commodity-level info
                var first = group.First();
                var commodity = first.CommodityEntity;

                var allPositions = group.Select(g => new { g.Position, g.LocationName }).ToList();
                var bids = allPositions.Where(ap => ap.Position != null && ap.Position.PositionTypeId == BidPosition).ToList();
                var offers = allPositions.Where(ap => ap.Position != null && ap.Position.PositionTypeId == AskPosition).ToList();

                // 3. Best Positions Logic
                var bestBidPos = bids.OrderByDescending(ap => ap.Position.UnitPrice).FirstOrDefault();
                var bestOfferPos = offers.OrderBy(ap => ap.Position.UnitPrice).FirstOrDefault();

                // 4. Market Depth
                var bidDepth = bids
                    .GroupBy(ap => ap.Position.UnitPrice)
                    .OrderByDescending(g => g.Key)
                    .Select(g => new MarketDepthDto
                    {
                        Price = g.Key,
                        Locations = g.Select(ap => ap.LocationName).Distinct().ToList(),
                        TotalActivePosforPrice = g.Count()
                    }).ToList();

                var offerDepth = offers
                    .GroupBy(ap => ap.Position.UnitPrice)
                    .OrderBy(g => g.Key)
                    .Select(g => new MarketDepthDto
                    {
                        Price = g.Key,
                        Locations = g.Select(ap => ap.LocationName).Distinct().ToList(),
                        TotalActivePosforPrice = g.Count()
                    }).ToList();

                // --- 3. Extract Best Prices for Improvement Logic ---
                var bestBid = bestBidPos?.Position.UnitPrice ?? 0m;
                var bestOffer = bestOfferPos?.Position.UnitPrice ?? 0m;

                // Handle Bid Logic (higher price is better)
                if (bestBid > commodity.LastBestBid && bestBid > 0)
                {
                    commodity.IsBidImproved = true;
                    commodity.LastBestBid = bestBid;
                }
                else if (bestBid < commodity.LastBestBid && bestBid > 0)
                {
                    commodity.IsBidImproved = false;
                    commodity.LastBestBid = bestBid;
                }

                // Handle Offer Logic (lower price is better)
                if (bestOffer < commodity.LastBestOffer && bestOffer > 0)
                {
                    commodity.IsOfferImproved = true;
                    commodity.LastBestOffer = bestOffer;
                }
                else if (commodity.LastBestOffer == 0 && bestOffer > 0)
                {
                    commodity.IsOfferImproved = true;
                    commodity.LastBestOffer = bestOffer;
                }
                else if (bestOffer > commodity.LastBestOffer && bestOffer > 0)
                {
                    commodity.IsOfferImproved = false;
                    commodity.LastBestOffer = bestOffer;
                }

                commodity.DateUpdated = DateTimeOffset.Now;

                // In case GroupBy broke tracking, this ensures the update is sent.
                _context.Entry(commodity).State = EntityState.Modified;

                // 3 Build response DTO
                response.Add(new MarketResponseDto
                {
                    CommodityId = first.CommodityId,
                    CommodityTypeId = first.CommodityTypeId,
                    CommodityName = first.CommodityName,
                    CommodityImageId = first.CommodityImageId,
                    LotSize = (short?)first.LotSize,
                    UnitOfMeasure = first.UnitOfMeasureCode,
                    ImageUrl = $"{ApiControllers.CommodityImages}/{first.CommodityId}/image",
                    // Expiry flags (True if the BEST position is expiring)
                    IsBestBidSoonToExpire = bestBidPos != null && IsExpired(bestBidPos.Position.StartDate, bestBidPos.Position.ExpiryDate, now),
                    IsBestOfferSoonToExpire = bestOfferPos != null && IsExpired(bestOfferPos.Position.StartDate, bestOfferPos.Position.ExpiryDate, now),

                    // Market Depth & Counts
                    BidDepth = bidDepth,
                    OfferDepth = offerDepth,
                    IsBidImproved = commodity.IsBidImproved,
                    IsOfferImproved = commodity.IsOfferImproved,

                });
            }

            // 4 Single database write
            await _context.SaveChangesAsync();

            return response;
        }

        // Helper method to calculate soon to expire
        private bool IsExpired(DateTimeOffset start, DateTimeOffset expiry, DateTime now)
        {
            var total = (expiry - start).TotalSeconds;
            if (total <= 0) return false;
            return ((now - start).TotalSeconds / total) >= 0.8;
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
                    PositionCount = g.Count(),
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
                    PositionCount = g.Count(),
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

        // New Method for the Insight Page Overview
        public async Task<List<MarketCommodityDto>> GetMarketOverviewAsync(int positionTypeId)
        {
            var now = DateTime.UtcNow;
            var yesterday = now.AddHours(-24);

            var query = await (
                from c in _context.Commodities
                join ci in _context.CommodityImage
                    on c.CommodityId equals ci.CommodityId into imageGroup
                from ci in imageGroup.DefaultIfEmpty()

                select new
                {
                    c.CommodityId,
                    c.CommodityName,
                    c.LotSize,
                    ImageFileName = ci != null ? ci.FileName : null,

                    UomCode = c.UnitOfMeasure != null
                        ? c.UnitOfMeasure.UnitOfMeasureCodeEnglish
                        : "",

                    CurrentBestPrice = positionTypeId == BidPosition
                        ? _context.Positions
                            .Where(p => p.CommodityId == c.CommodityId &&
                                        p.PositionTypeId == positionTypeId &&
                                        p.StartDate <= now &&
                                        p.ExpiryDate > now)
                            .Max(p => (decimal?)p.UnitPrice) ?? 0m
                        : _context.Positions
                            .Where(p => p.CommodityId == c.CommodityId &&
                                        p.PositionTypeId == positionTypeId &&
                                        p.StartDate <= now &&
                                        p.ExpiryDate > now)
                            .Min(p => (decimal?)p.UnitPrice) ?? 0m,

                    PreviousBestPrice = positionTypeId == BidPosition
                        ? _context.Positions
                            .Where(p => p.CommodityId == c.CommodityId &&
                                        p.PositionTypeId == positionTypeId &&
                                        p.StartDate <= yesterday &&
                                        p.ExpiryDate > yesterday)
                            .Max(p => (decimal?)p.UnitPrice) ?? 0m
                        : _context.Positions
                            .Where(p => p.CommodityId == c.CommodityId &&
                                        p.PositionTypeId == positionTypeId &&
                                        p.StartDate <= yesterday &&
                                        p.ExpiryDate > yesterday)
                            .Min(p => (decimal?)p.UnitPrice) ?? 0m
                }).ToListAsync();

            var marketData = new List<MarketCommodityDto>();

            foreach (var item in query)
            {
                decimal difference = 0;

                if (item.CurrentBestPrice > 0 && item.PreviousBestPrice > 0)
                {
                    difference = item.CurrentBestPrice - item.PreviousBestPrice;
                }

                marketData.Add(new MarketCommodityDto
                {
                    CommodityId = item.CommodityId,
                    CommodityName = item.CommodityName,
                    LotSizeDisplay = $"{item.LotSize} {item.UomCode}",
                    CurrentPrice = item.CurrentBestPrice,
                    PriceDifference = difference,

                    // IMPORTANT
                    ImageUrl = item.ImageFileName != null
                        ? $"{ApiControllers.CommodityImages}/{item.CommodityId}/image"
                        : null
                });
            }

            return marketData;
        }

        public async Task<List<MarketInsightChartResponseDto>> GetPriceChartAsync(Guid commodityId, string range)
        {
            string searchRange = range.ToUpper();
            // 1. Map the ranges to their respective intervals and look back windows
            (string interval, DateTime startDate, int minPoints) = searchRange switch
            {
                "1D" => ("1m", DateTime.UtcNow.AddDays(-1), 12),  //60      
                "1W" => ("1D", DateTime.UtcNow.AddDays(-7), 7),  //82
                "1M" => ("1D", DateTime.UtcNow.AddMonths(-1), 30),
                "1Y" => ("1W", DateTime.UtcNow.AddYears(-1), 52),
                _ => ("1D", DateTime.UtcNow.AddMonths(-1), 15)
            };

            // 2. Base Query
            var query = _context.AggregatedPrices
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