using MarketPrice.Data;
using MarketPrice.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MarketPrice.Services.Implementations
{
    public class LookupProviderService : ILookupProviderService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private Dictionary<(string Text, int Type), int> _lookupCache = new();

        public LookupProviderService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task InitializeAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MarketPriceDbContext>();

            var data = await context.LookupData.ToListAsync();
            _lookupCache = data.ToDictionary(
                x => (x.LookupDataValue, x.LookupDataTypeId),
                x => x.LookupDataId
            );
        }

        public int GetLookupId(string text, int typeId)
        {
            if (_lookupCache.TryGetValue((text, typeId), out int id))
                return id;

            throw new Exception($"Lookup with text '{text}' and Type '{typeId}' not found.");
        }
    }
}