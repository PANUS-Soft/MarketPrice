using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Position.DTOs
{
    public class PositionResponseDto
    {
        public Guid PositionId { get; set; }
        public string Message { get; set; }
    }
}
