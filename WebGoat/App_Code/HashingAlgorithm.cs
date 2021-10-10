using System;
using System.Security.Cryptography;

namespace OWASP.WebGoat.NET.App_Code.DB
{
    public class HashingAlgorithm
    {
        private const int SALT_SIZE = 64;
        private const int HASH_SIZE = 64;
        private const int PBKDF2_ITERATIONS = 100000;

        public string Salt { get; set; }
        public string Digest { get; set; }

        public HashingAlgorithm(string password, byte[] providedSalt = null)
        {
            byte[] salt = providedSalt;
            if (providedSalt == null)
            {
                salt = GenerateRandomSalt();
            }

            byte[] digest = ApplyPBKDF2Algo(password, salt);

            Salt = Convert.ToBase64String(salt);
            Digest = Convert.ToBase64String(digest);
        }

        private byte[] GenerateRandomSalt()
        {
            // Generate a random salt.
            RNGCryptoServiceProvider rngProvider = new RNGCryptoServiceProvider();
            byte[] salt = new byte[SALT_SIZE];
            rngProvider.GetBytes(salt);

            return salt;
        }

        private byte[] ApplyPBKDF2Algo(string password, byte[] salt)
        {
            // Generate hash.
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, PBKDF2_ITERATIONS);
            return pbkdf2.GetBytes(HASH_SIZE);
        }
    }
}