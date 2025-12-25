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

        public string HashPassword(string password, string passwordSalt)
        {
            if(String.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password), "password cannot be null or empty");

            if(String.IsNullOrEmpty(passwordSalt))
                throw new ArgumentNullException(nameof(passwordSalt), "Salt connot be null or empty");
            
            byte[] saltBytes = Convert.FromBase64String(passwordSalt);

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

        public bool VerifyPassword(string password, string passwordHash, string passwordSalt)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordHash) || string.IsNullOrEmpty(passwordSalt))
                return false;

            string computedHash = HashPassword(password, passwordSalt);

            // Timing-safe comparison
            return CryptographicOperations.FixedTimeEquals(Convert.FromBase64String(computedHash),
                Convert.FromBase64String(passwordHash));

            //return matches ? true : false;

        }
    }

}
