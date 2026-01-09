using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Ui.Models
{
    public class UserSession
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpireAt { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
    }
}
