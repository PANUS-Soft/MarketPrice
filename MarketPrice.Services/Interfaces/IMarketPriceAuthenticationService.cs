using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Services.Interfaces
{
    public interface IMarketPriceAuthenticationService
    {

        /// <summary>
        /// Returns true if the user with username and password exists, starts a new session and marks the user as signed in the database
        /// Otherwise returns false
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool Authenticate(string username, string password);


        /// <summary>
        /// Marks the user as signed out in the database and ends all active sessions with the application
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool SignOut(string username);

        /// <summary>
        /// Returns true if a user with the passed username exists
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool UserExists(string username);
    }
}
