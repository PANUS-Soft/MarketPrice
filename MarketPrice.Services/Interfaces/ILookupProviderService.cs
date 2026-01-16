using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Services.Interfaces
{
    public interface ILookupProviderService
    {
        Task InitializeAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textEnglish"></param>
        /// <param name="typeId"></param>
        /// <returns></returns>
        int GetLookupId(string textEnglish, int typeId);
    }
}