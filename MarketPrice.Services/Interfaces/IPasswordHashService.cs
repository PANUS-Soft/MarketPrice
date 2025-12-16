using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Services.Interfaces
{

    public interface IPasswordHashService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="salt"></param>
        /// <returns></returns>
        public string GenerateSalt(string salt);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="salt"></param>
        /// <returns></returns>

        public string HashPassword(string key, string salt);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="salt"></param>
        /// <returns></returns>


        public string VerifyPassword(string key, string salt);



    }
}
