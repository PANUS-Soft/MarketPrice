using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Services.Interfaces
{
    public interface IVerificationNotificationService
    {
        Task SendAsync(string method, string destination, string code, CancellationToken cancellation = default);
    }
}
