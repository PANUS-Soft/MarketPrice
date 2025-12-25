using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Services.Interfaces
{

    /// <summary>
    /// Contract for secure password management.
    /// </summary>

    public interface IPasswordHashService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="salt"></param>
        /// <returns></returns>
        string GenerateSalt();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        string HashPassword(string password, string passwordSalt);

       /// <summary>
       /// 
       /// </summary>
       /// <param name="passwor"></param>
       /// <param name="hash"></param>
       /// <param name="salt"></param>
       /// <returns></returns>

        bool VerifyPassword(string password, string passwordHash, string passwordSalt);


    }
}
