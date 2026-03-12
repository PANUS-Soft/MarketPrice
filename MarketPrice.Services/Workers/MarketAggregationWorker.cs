using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MarketPrice.Data.Models;
using MarketPrice.Data;


namespace MarketPrice.Services.Workers
{
    public class MarketAggregationWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<MarketAggregationWorker> logger) : BackgroundService
    {
        private readonly PeriodicTimer _timer = new(TimeSpan.FromHours(2));

        // Finds whether service works
        public static DateTime LastSuccessfulRun { get; private set; }
        public static string Status { get; private set; } = "Initializing";

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Market Aggregation Service started.");
            Status = "Running - Initializing";

            // Run once immediately on startup to catch up on any gaps right away
            try
            {
                await DoWork(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Initial startup aggregation failed.");
            }

            // 2. Start the periodic timer loop
            while (await _timer.WaitForNextTickAsync(stoppingToken))
            {
                await DoWork(stoppingToken);
            }

        }

        // Helper Method
        private async Task DoWork(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<MarketPriceDbContext>();

                // These will now handle their own "Catch-up" logic
                await Aggregation(context, stoppingToken);
                await DailyRollup(context, stoppingToken);
                await WeeklyRollup(context, stoppingToken);

                LastSuccessfulRun = DateTime.UtcNow;
                Status = "Healthy";
            }
            catch (Exception ex)
            {
                Status = $"Error: {ex.Message}";
                logger.LogError(ex, "An error occurred during the aggregation cycle.");
            }
        }

        private async Task Aggregation(MarketPriceDbContext context, CancellationToken ct)
        {
            // 1. Define the "Ceiling":
            var now = DateTime.UtcNow;
            var currentWindowEnd = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, DateTimeKind.Utc);
            if (currentWindowEnd.Hour % 2 != 0) currentWindowEnd = currentWindowEnd.AddHours(-1);

            // 2. Find the "Floor": The last 2H record.
            var lastEntry = await context.AggregatedPrices
                .AsNoTracking()
                .Where(ap => ap.Interval == "2H")
                .OrderByDescending(ap => ap.Timestamp)
                .FirstOrDefaultAsync(ct);

            // 3. If no data exists, we start from the beginning of the week prior to current time
            DateTime nextBucketStart;
            if (lastEntry == null)
            {
                nextBucketStart = currentWindowEnd.AddDays(-7);
                logger.LogInformation("No existing records found. Starting initial backfill from {Start}", nextBucketStart);
            }
            else
            {
                nextBucketStart = lastEntry.Timestamp.AddHours(2);
            }

            // 4. The Backfill Loop
            // This will run multiple times if there is a gap, or zero times if we are up to date
            while (nextBucketStart < currentWindowEnd)
            {
                var nextBucketEnd = nextBucketStart.AddHours(2);

                // --- SELF-HEALING START ---
                // Check if position data exists for this bucket before attempting aggregation
                bool dataExists = await context.Positions
                    .AsNoTracking()
                    .AnyAsync(p => p.StartDate == nextBucketStart, ct);

                if (!dataExists)
                {
                    logger.LogWarning("Missing raw data for {Start}. Triggering Repair...", nextBucketStart);

                    // Call the Stored Procedure manually for this specific time
                    await context.Database.ExecuteSqlInterpolatedAsync(
                        $"EXEC [dbo].[PopulatePositionsTable] @ManualStartDate = {nextBucketStart}", ct);

                    logger.LogInformation("Repair complete for {Start}.", nextBucketStart);
                }

                // --- SELF-HEALING END ----
                try
                {
                    await Perform2HAggregation(context, nextBucketStart, nextBucketEnd, ct);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to process bucket starting at {Start}. Stopping backfill.", nextBucketStart);
                    throw;
                }

                // Move the pointer forward by 2 hours
                nextBucketStart = nextBucketEnd;
            }

            if (nextBucketStart == currentWindowEnd)
            {
                logger.LogInformation("Aggregation is up to date as of {Time}", currentWindowEnd);
            }
        }

        private async Task Perform2HAggregation(MarketPriceDbContext context, DateTime start, DateTime end, CancellationToken ct)
        {
            const int BID_ID = 6001;
            const int OFFER_ID = 6002;

            // Use .AsNoTracking() for speed and to keep memory low
            var aggregates = await context.Positions
                .AsNoTracking()
                .Where(p => p.StartDate <= start && p.ExpiryDate >= end)
                .GroupBy(p => p.CommodityId)
                .Select(g => new AggregatedPrice
                {
                    CommodityId = g.Key,
                    Timestamp = start,
                    Interval = "2H",

                    // Bids (6001) - Use nullable cast to handle empty groups safely
                    AvgBid = g.Where(x => x.PositionTypeId == BID_ID).Average(x => (decimal?)x.UnitPrice) ?? 0,
                    HighBid = g.Where(x => x.PositionTypeId == BID_ID).Max(x => (decimal?)x.UnitPrice) ?? 0,
                    LowBid = g.Where(x => x.PositionTypeId == BID_ID).Min(x => (decimal?)x.UnitPrice) ?? 0,

                    // Offers (6002)
                    AvgOffer = g.Where(x => x.PositionTypeId == OFFER_ID).Average(x => (decimal?)x.UnitPrice) ?? 0,
                    HighOffer = g.Where(x => x.PositionTypeId == OFFER_ID).Max(x => (decimal?)x.UnitPrice) ?? 0,
                    LowOffer = g.Where(x => x.PositionTypeId == OFFER_ID).Min(x => (decimal?)x.UnitPrice) ?? 0,

                    PositionCount = g.Count()
                })
                .ToListAsync(ct);

            if (aggregates.Any())
            {
                context.AggregatedPrices.AddRange(aggregates);
                await context.SaveChangesAsync(ct);
                logger.LogInformation("Successfully aggregated {Count} commodities for 2H window.", aggregates.Count);
            }
        }

        private async Task DailyRollup(MarketPriceDbContext context, CancellationToken ct)
        {
            // 1. Find the most recent Daily record in the database
            var lastDaily = await context.AggregatedPrices
                .AsNoTracking()
                .Where(ap => ap.Interval == "1D")
                .OrderByDescending(ap => ap.Timestamp)
                .FirstOrDefaultAsync(ct);

            // If no daily records exist, start from 7 days ago (or your preferred history limit)
            DateTime nextDateToProcess = lastDaily?.Timestamp.AddDays(1)
                                         ?? DateTime.UtcNow.Date.AddDays(-7);

            
            while (nextDateToProcess < DateTime.UtcNow.Date)
            {
                logger.LogInformation("Processing Daily Roll-up for {Date}", nextDateToProcess.ToShortDateString());

                // If the 2H backfill hasn't reached this date yet, we should stop and wait.
                bool sourceDataExists = await context.AggregatedPrices
                    .AnyAsync(ap => ap.Interval == "2H" && ap.Timestamp >= nextDateToProcess, ct);

                if (!sourceDataExists)
                {
                    logger.LogWarning("Missing 2H source data for {Date}. Skipping Daily Roll-up.", nextDateToProcess);
                    break;
                }

                var dailyAggregates = await context.AggregatedPrices
                    .AsNoTracking()
                    .Where(ap => ap.Interval == "2H" &&
                                 ap.Timestamp >= nextDateToProcess &&
                                 ap.Timestamp < nextDateToProcess.AddDays(1))
                    .GroupBy(ap => ap.CommodityId)
                    .Select(g => new AggregatedPrice
                    {
                        CommodityId = g.Key,
                        Timestamp = nextDateToProcess, 
                        Interval = "1D",
                        AvgBid = g.Average(x => x.AvgBid),
                        AvgOffer = g.Average(x => x.AvgOffer),
                        HighBid = g.Max(x => x.HighBid),
                        LowBid = g.Where(x => x.LowBid > 0).Min(x => (decimal?)x.LowBid) ?? 0,
                        HighOffer = g.Max(x => x.HighOffer),
                        LowOffer = g.Where(x => x.LowOffer > 0).Min(x => (decimal?)x.LowOffer) ?? 0,
                        PositionCount = g.Sum(x => x.PositionCount)
                    })
                    .ToListAsync(ct);

                if (dailyAggregates.Any())
                {
                    context.AggregatedPrices.AddRange(dailyAggregates);
                    await context.SaveChangesAsync(ct);
                    logger.LogInformation("Successfully created {Count} Daily records for {Date}.", dailyAggregates.Count, nextDateToProcess);
                }

                // Move to the next calendar day
                nextDateToProcess = nextDateToProcess.AddDays(1);
            }
        }

        private async Task WeeklyRollup(MarketPriceDbContext context, CancellationToken ct)
        {
            // 1. Find the most recent Weekly record
            var lastWeekly = await context.AggregatedPrices
                .AsNoTracking()
                .Where(ap => ap.Interval == "1W")
                .OrderByDescending(ap => ap.Timestamp)
                .FirstOrDefaultAsync(ct);

            // 2. Determine the start date for the next missing week
            // If no records exist, we go back 4 weeks. 
            // Otherwise, we start exactly 7 days after the last recorded week.
            DateTime nextWeekStart = lastWeekly?.Timestamp.AddDays(7)
                                     ?? GetStartOfLastWeeks(4);

            // 3. Loop to catch up
            // We only process a week if it is fully complete (End of week < Today)
            while (nextWeekStart.AddDays(7) <= DateTime.UtcNow.Date)
            {
                var nextWeekEnd = nextWeekStart.AddDays(7);
                logger.LogInformation("Processing Weekly Roll-up: {Start} to {End}",
                    nextWeekStart.ToShortDateString(), nextWeekEnd.ToShortDateString());

                // 4. Source Data Check: Ensure all 7 Daily records exist for this week
                int dailyCount = await context.AggregatedPrices
                    .Where(ap => ap.Interval == "1D" &&
                                 ap.Timestamp >= nextWeekStart &&
                                 ap.Timestamp < nextWeekEnd)
                    .Select(ap => ap.Timestamp)
                    .Distinct()
                    .CountAsync(ct);

                if (dailyCount < 7)
                {
                    logger.LogWarning("Only {Count}/7 daily records found for week {Start}. Waiting for Daily backfill.",
                        dailyCount, nextWeekStart.ToShortDateString());
                    break; 
                }

                var weeklyAggregates = await context.AggregatedPrices
                    .AsNoTracking()
                    .Where(ap => ap.Interval == "1D" &&
                                 ap.Timestamp >= nextWeekStart &&
                                 ap.Timestamp < nextWeekEnd)
                    .GroupBy(ap => ap.CommodityId)
                    .Select(g => new AggregatedPrice
                    {
                        CommodityId = g.Key,
                        Timestamp = nextWeekStart,
                        Interval = "1W",
                        AvgBid = g.Average(x => x.AvgBid),
                        AvgOffer = g.Average(x => x.AvgOffer),
                        HighBid = g.Max(x => x.HighBid),
                        LowBid = g.Where(x => x.LowBid > 0).Min(x => (decimal?)x.LowBid) ?? 0,
                        HighOffer = g.Max(x => x.HighOffer),
                        LowOffer = g.Where(x => x.LowOffer > 0).Min(x => (decimal?)x.LowOffer) ?? 0,
                        PositionCount = g.Sum(x => x.PositionCount)
                    })
                    .ToListAsync(ct);

                if (weeklyAggregates.Any())
                {
                    context.AggregatedPrices.AddRange(weeklyAggregates);
                    await context.SaveChangesAsync(ct);
                    logger.LogInformation("Successfully created {Count} Weekly records for week starting {Date}.",
                        weeklyAggregates.Count, nextWeekStart.ToShortDateString());
                }

                nextWeekStart = nextWeekEnd;
            }
        }

        // Helper to find the Monday of a few weeks ago
        private DateTime GetStartOfLastWeeks(int weeksBack)
        {
            var today = DateTime.UtcNow.Date;
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            var currentMonday = today.AddDays(-1 * diff);
            return currentMonday.AddDays(-7 * weeksBack);
        }
    }
}
