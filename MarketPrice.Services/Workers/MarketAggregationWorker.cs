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
        private readonly PeriodicTimer _timer = new(TimeSpan.FromMinutes(1));

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
                await Aggregate1Min(context, stoppingToken);
                await DailyRollup(context, stoppingToken);
                await WeeklyRollup(context, stoppingToken);
                await MonthlyRollup(context, stoppingToken);
                await YearlyRollup(context, stoppingToken);

                LastSuccessfulRun = DateTime.UtcNow;
                Status = "Healthy";
            }
            catch (Exception ex)
            {
                Status = $"Error: {ex.Message}";
                logger.LogError(ex, "An error occurred during the aggregation cycle.");
            }
        }

        // 1-Minute Aggregation
        private async Task Aggregate1Min(MarketPriceDbContext context, CancellationToken ct)
        {
            const int BID_ID = 6001;
            const int OFFER_ID = 6002;

            var now = DateTime.UtcNow;
            var bucketStart = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, DateTimeKind.Utc);
            var bucketEnd = bucketStart.AddMinutes(1);

            var aggregates = await context.Positions
                .AsNoTracking()
                .Where(p => p.StartDate <= bucketStart && p.ExpiryDate >= bucketEnd)
                .GroupBy(p => p.CommodityId)
                .Select(g => new AggregatedPrice
                {
                    CommodityId = g.Key,
                    Timestamp = bucketStart,
                    Interval = "1M",

                    AvgBid = g.Where(x => x.PositionTypeId == BID_ID).Average(x => (decimal?)x.UnitPrice) ?? 0,
                    HighBid = g.Where(x => x.PositionTypeId == BID_ID).Max(x => (decimal?)x.UnitPrice) ?? 0,
                    LowBid = g.Where(x => x.PositionTypeId == BID_ID).Min(x => (decimal?)x.UnitPrice) ?? 0,
                   
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
                logger.LogInformation("Aggregated {Count} commodities for 1M window.", aggregates.Count);
            }
        }

        // Daily Rollup (from 1M)
        private async Task DailyRollup(MarketPriceDbContext context, CancellationToken ct)
        {
            var lastDaily = await context.AggregatedPrices
                .AsNoTracking()
                .Where(ap => ap.Interval == "1D")
                .OrderByDescending(ap => ap.Timestamp)
                .FirstOrDefaultAsync(ct);

            DateTime nextDateToProcess = lastDaily?.Timestamp.AddDays(1)
                                         ?? DateTime.UtcNow.Date.AddDays(-7);

            while (nextDateToProcess < DateTime.UtcNow.Date)
            {
                var dailyAggregates = await context.AggregatedPrices
                    .AsNoTracking()
                    .Where(ap => ap.Interval == "1M" &&
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
                        LowBid = g.Min(x => x.LowBid),
                        HighOffer = g.Max(x => x.HighOffer),
                        LowOffer = g.Min(x => x.LowOffer),
                        PositionCount = g.Sum(x => x.PositionCount)
                    })
                    .ToListAsync(ct);

                if (dailyAggregates.Any())
                {
                    context.AggregatedPrices.AddRange(dailyAggregates);
                    await context.SaveChangesAsync(ct);
                    logger.LogInformation("Created {Count} Daily records for {Date}.", dailyAggregates.Count, nextDateToProcess);
                }

                nextDateToProcess = nextDateToProcess.AddDays(1);
            }
        }

        // Weekly Rollup (from 1D)
        private async Task WeeklyRollup(MarketPriceDbContext context, CancellationToken ct)
        {
            var lastWeekly = await context.AggregatedPrices
                .AsNoTracking()
                .Where(ap => ap.Interval == "1W")
                .OrderByDescending(ap => ap.Timestamp)
                .FirstOrDefaultAsync(ct);

            DateTime nextWeekStart = lastWeekly?.Timestamp.AddDays(7) ?? GetStartOfLastWeeks(4);



            while (nextWeekStart.AddDays(7) <= DateTime.UtcNow.Date)
            {
                var weeklyAggregates = await context.AggregatedPrices
                    .AsNoTracking()
                    .Where(ap => ap.Interval == "1D" &&
                                 ap.Timestamp >= nextWeekStart &&
                                 ap.Timestamp < nextWeekStart.AddDays(7))
                    .GroupBy(ap => ap.CommodityId)
                    .Select(g => new AggregatedPrice
                    {
                        CommodityId = g.Key,
                        Timestamp = nextWeekStart,
                        Interval = "1W",
                        AvgBid = g.Average(x => x.AvgBid),
                        AvgOffer = g.Average(x => x.AvgOffer),
                        HighBid = g.Max(x => x.HighBid),
                        LowBid = g.Min(x => x.LowBid),
                        HighOffer = g.Max(x => x.HighOffer),
                        LowOffer = g.Min(x => x.LowOffer),
                        PositionCount = g.Sum(x => x.PositionCount)
                    })
                    .ToListAsync(ct);

                if (weeklyAggregates.Any())
                {
                    context.AggregatedPrices.AddRange(weeklyAggregates);
                    await context.SaveChangesAsync(ct);
                    logger.LogInformation("Created {Count} Weekly records for week starting {Date}.", weeklyAggregates.Count, nextWeekStart);
                }

                nextWeekStart = nextWeekStart.AddDays(7);
            }

        }

        private DateTime GetStartOfLastWeeks(int weeksBack)
        {
            var today = DateTime.UtcNow.Date;
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            var currentMonday = today.AddDays(-1 * diff);
            return currentMonday.AddDays(-7 * weeksBack);
        }


        // Monthly Rollup (from 1D)
        private async Task MonthlyRollup(MarketPriceDbContext context, CancellationToken ct)
        {
            var lastMonthly = await context.AggregatedPrices
                .AsNoTracking()
                .Where(ap => ap.Interval == "1MTH")
                .OrderByDescending(ap => ap.Timestamp)
                .FirstOrDefaultAsync(ct);

            DateTime nextMonthStart = lastMonthly?.Timestamp.AddMonths(1)
                                     ?? new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(-6);

            while (nextMonthStart.AddMonths(1) <= DateTime.UtcNow.Date)
            {
                var monthlyAggregates = await context.AggregatedPrices
                    .AsNoTracking()
                    .Where(ap => ap.Interval == "1D" &&
                                 ap.Timestamp >= nextMonthStart &&
                                 ap.Timestamp < nextMonthStart.AddMonths(1))
                    .GroupBy(ap => ap.CommodityId)
                    .Select(g => new AggregatedPrice
                    {
                        CommodityId = g.Key,
                        Timestamp = nextMonthStart,
                        Interval = "1MTH",
                        AvgBid = g.Average(x => x.AvgBid),
                        AvgOffer = g.Average(x => x.AvgOffer),
                        HighBid = g.Max(x => x.HighBid),
                        LowBid = g.Min(x => x.LowBid),
                        HighOffer = g.Max(x => x.HighOffer),
                        LowOffer = g.Min(x => x.LowOffer),
                        PositionCount = g.Sum(x => x.PositionCount)
                    })
                    .ToListAsync(ct);

                if (monthlyAggregates.Any())
                {
                    context.AggregatedPrices.AddRange(monthlyAggregates);
                    await context.SaveChangesAsync(ct);
                    logger.LogInformation("Created {Count} Monthly records for {Month}.",
                        monthlyAggregates.Count, nextMonthStart.ToString("yyyy-MM"));
                }
                nextMonthStart = nextMonthStart.AddMonths(1);
            }
        }

        // Yearly Rollu (From 1w)
        private async Task YearlyRollup(MarketPriceDbContext context, CancellationToken ct)
        {
            // 1. Find the most recent Yearly record
            var lastYearly = await context.AggregatedPrices
                .AsNoTracking()
                .Where(ap => ap.Interval == "1Y")
                .OrderByDescending(ap => ap.Timestamp)
                .FirstOrDefaultAsync(ct);

            // 2. Determine the start of the next missing year
            DateTime nextYearStart = lastYearly?.Timestamp.AddYears(1)
                                     ?? new DateTime(DateTime.UtcNow.Year - 5, 1, 1); // backfill up to 5 years

            // 3. Loop until we reach the current year
            while (nextYearStart.AddYears(1) <= DateTime.UtcNow.Date)
            {
                var nextYearEnd = nextYearStart.AddYears(1);
                logger.LogInformation("Processing Yearly Roll-up: {Start} to {End}",
                    nextYearStart.ToShortDateString(), nextYearEnd.ToShortDateString());

                // 4. Source Data Check: Ensure weekly records exist for this year
                int weeklyCount = await context.AggregatedPrices
                    .Where(ap => ap.Interval == "1W" &&
                                 ap.Timestamp >= nextYearStart &&
                                 ap.Timestamp < nextYearEnd)
                    .Select(ap => ap.Timestamp)
                    .Distinct()
                    .CountAsync(ct);

                if (weeklyCount < 52) // not enough weekly data yet
                {
                    logger.LogWarning("Only {Count}/52 weekly records found for year {Year}. Waiting for Weekly backfill.",
                        weeklyCount, nextYearStart.Year);
                    break;
                }

                // 5. Aggregate weekly data into yearly
                var yearlyAggregates = await context.AggregatedPrices
                    .AsNoTracking()
                    .Where(ap => ap.Interval == "1W" &&
                                 ap.Timestamp >= nextYearStart &&
                                 ap.Timestamp < nextYearEnd)
                    .GroupBy(ap => ap.CommodityId)
                    .Select(g => new AggregatedPrice
                    {
                        CommodityId = g.Key,
                        Timestamp = nextYearStart,
                        Interval = "1Y",
                        AvgBid = g.Average(x => x.AvgBid),
                        AvgOffer = g.Average(x => x.AvgOffer),
                        HighBid = g.Max(x => x.HighBid),
                        LowBid = g.Min(x => x.LowBid),
                        HighOffer = g.Max(x => x.HighOffer),
                        LowOffer = g.Min(x => x.LowOffer),
                        PositionCount = g.Sum(x => x.PositionCount)
                    })
                    .ToListAsync(ct);

                if (yearlyAggregates.Any())
                {
                    context.AggregatedPrices.AddRange(yearlyAggregates);
                    await context.SaveChangesAsync(ct);
                    logger.LogInformation("Created {Count} Yearly records for {Year}.",
                        yearlyAggregates.Count, nextYearStart.Year);
                }

                nextYearStart = nextYearEnd;
            }
        }
        // Helper to find the Monday of a few weeks ago
        //private DateTime GetStartOfLastWeeks(int weeksBack)
        //{
        //    var today = DateTime.UtcNow.Date;
        //    int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
        //    var currentMonday = today.AddDays(-1 * diff);
        //    return currentMonday.AddDays(-7 * weeksBack);
        //}
    }
}
