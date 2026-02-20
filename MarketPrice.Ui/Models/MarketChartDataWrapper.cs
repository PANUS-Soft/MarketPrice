using MarketPrice.Domain.Market.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Ui.Models
{
    public class MarketChartDataWrapper
    {
        public List<MarketInsightChartResponseDto>? Data { get; set; }

        public bool IsRunning { get; set; }
        public DateTime LastRun { get; set; }
        public string? CurrentStatus { get; set; }
    }
}
