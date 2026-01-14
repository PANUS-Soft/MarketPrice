using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Ui.Common
{
    public class ApiSettings
    {
        public string BaseUrl { get; set; } = string.Empty;

        public string ClientNameHeader { get; set; } = null!;
    }
}
