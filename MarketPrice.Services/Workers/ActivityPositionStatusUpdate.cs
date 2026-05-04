using MarketPrice.Data;
using MarketPrice.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Services.Workers
{
    public class ActivityPositionStatusUpdate : BackgroundService
    {

        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMarketRealtimeService _realtime;


        public ActivityPositionStatusUpdate(IServiceScopeFactory serviceScopeFactory, IMarketRealtimeService realtime)
        {
            _realtime = realtime;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<MarketPriceDbContext>();

                var now = DateTime.UtcNow;

                var position = await context.Positions.Where(p => p.ExpiryDate >= now.AddMinutes(-1)).ToListAsync();

                foreach (var p in position)
                {
                    string newState = now < p.StartDate ? "Pending" :
                        now <= p.ExpiryDate ? "Open" : "Close";

                    await _realtime.BroadcastActivityPositionStatusUpdateAsync(p, newState);
                }

                await Task.Delay(10000, stoppingToken);
                
            }
        }

    }
}
