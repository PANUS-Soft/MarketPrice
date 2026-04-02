using MarketPrice.Api.Hubs;
using MarketPrice.Data.Models;
using MarketPrice.Domain.Common;
using MarketPrice.Domain.Market.DTOs;
using MarketPrice.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace MarketPrice.Api.Realtime
{
    public class SignalRMarketRealtimeService : IMarketRealtimeService
    {
        private readonly IHubContext<MarketHub> _hub;
        private readonly ILogger<SignalRMarketRealtimeService> _logger;

        public SignalRMarketRealtimeService(
            IHubContext<MarketHub> hub,
            ILogger<SignalRMarketRealtimeService> logger)
        {
            _hub = hub;
            _logger = logger;
        }

        public async Task BroadcastPositionUpdateAsync(Position position, bool isOffer)
        {
            var dto = new MarketUpdateDto
            {
                CommodityId = position.CommodityId,
                Price = position.UnitPrice,
                Type = isOffer ? "Offer" : "Bid",
                Quantity = position.Quantity,
                Timestamp = DateTime.UtcNow
            };

            try
            {
                await _hub.Clients
                    .Group(position.CommodityId.ToString())
                    .SendAsync(SignalREvents.MarketUpdate, dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error broadcasting market update");
            }
        }
    }
}