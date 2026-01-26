using MarketPrice.Domain.Home.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Services.Interfaces
{
    public interface IHomeService
    {
        Task<List<LoadHomeResponseDto>> LoadHomeAsync();
    }
}
