using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Position.Commands
{
    public class UpdatePositionCommand : CreatePositionCommand
    {
        public Guid PositionId { get; set; }
    }
}
