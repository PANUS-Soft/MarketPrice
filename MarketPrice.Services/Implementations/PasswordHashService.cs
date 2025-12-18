using MarketPrice.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Services.Implementations
{
    /// <summary>
    /// Provides secure password hashing and verification uing PBKD2.
    /// </summary>


    public class PasswordHashService : IPasswordHashService
    {

        private const int SaltSize = 16; 
        private const int HashSize = 32;
        private const int Iterations = 100_000;

        /// <summary>
        /// Generates a cryptographically secure random salt.
        /// </summary>
        /// <returns></returns>
        public string GenerateSalt()
        {
            byte[] saltBytes = new byte[SaltSize];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
                
            }
            return Convert.ToBase64String(saltBytes);
        }

        /// <summary>
        /// Hashes a password using PBKDF2 with SHA256
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>

        public string HashPassword(string password, string salt)
        {
            if(String.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password), "password cannot be null or empty");

            if(String.IsNullOrEmpty(salt))
                throw new ArgumentNullException(nameof(salt), "Salt connot be null or empty");
            
            byte[] saltBytes = Convert.FromBase64String(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password,saltBytes, Iterations, HashAlgorithmName.SHA256);

            byte[] hashBytes = pbkdf2.GetBytes(HashSize);

            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Verifies whether a plain text password matches a stored hash.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="hash"></param>
        /// <param name="salt"></param>
        /// <returns></returns>

        public string VerifyPassword(string password, string hash, string salt)
        {
            if (String.IsNullOrEmpty(hash))
                return "Invalid";

            string computedHash = HashPassword(password, salt);

            // Timing-safe comparison
            bool matches = CryptographicOperations.FixedTimeEquals(Convert.FromBase64String(computedHash),
                Convert.FromBase64String(hash));

            return matches ? "Valid" : "Invalid";

        }
    }

}
