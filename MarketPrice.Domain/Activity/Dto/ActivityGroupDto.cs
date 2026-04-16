using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Activity.Dto
{
   public class ActivityGroupDto
    {
        public List<ActivityResponseDto> Today { get; set; }
        public List<ActivityResponseDto> Yesterday { get; set; }
        public List<ActivityResponseDto> ThisWeek { get; set; }
        public List<ActivityResponseDto> LastWeek { get; set; }
        public List<ActivityResponseDto> ThisMonth { get; set; }
        public List<ActivityResponseDto> LastMonth { get; set; }
    }
}
