using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Activity.Command
{
    public class ActivityCommand
    {
        public required Guid UserId{ get; set; }
        public string? PositionType { get; set; }
    }
}
