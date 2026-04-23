using MarketPrice.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Services.Interfaces
{
    public interface IMarketRealtimeService
    {
        Task BroadcastPositionUpdateAsync(Position position, bool isOffer);

    }
}
