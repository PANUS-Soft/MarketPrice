using Microsoft.AspNetCore.SignalR;
using MarketPrice.Domain;

namespace MarketPrice.Api.Hubs
{
    public class MarketHub : Hub
    {
        public async Task JoinCommodityGroup(string commodityId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, commodityId);
        }

        public async Task LeaveCommodityGroup(string commodityId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, commodityId);
        }
    }
}