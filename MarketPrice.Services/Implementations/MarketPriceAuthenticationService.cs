using MarketPrice.Data;
using MarketPrice.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace MarketPrice.Services.Implementations
{
    public class MarketPriceAuthenticationService : IMarketPriceAuthenticationService
    {
        private readonly IDbContextFactory<MarketPriceDbContext> _dbContextFactory;

        public MarketPriceAuthenticationService(IDbContextFactory<MarketPriceDbContext> dbContextFactory )
        {
            _dbContextFactory = dbContextFactory;   
        }

        public bool Authenticate(string username, string password)
        {
            // use the repository to connect to the database to check that a user with the username/password combination exists
            var context = _dbContextFactory.CreateDbContext();

            var existingUser = context.Users.FirstOrDefault(u => EF.Equals(u.EmailAddress,username));
            if (existingUser == null)
            {
                return false;
            }

            return true;
        }

        public bool SignOut(string username)
        {
            throw new NotImplementedException();
        }

        public bool UserExists(string username)
        {
            throw new NotImplementedException();
        }
    }
}
