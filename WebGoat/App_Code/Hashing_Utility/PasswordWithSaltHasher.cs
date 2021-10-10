using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace OWASP.WebGoat.NET.App_Code.Hashing_Utility
{
    public class HashingAlgo
    {
        public static HashWithSaltResult GetHashDigestUsingSalt(string password, byte[] salt)
        {
            PasswordWithSaltHasher psHasher = new PasswordWithSaltHasher();
            HashWithSaltResult hashSHA256 = psHasher.HashWithSalt(password, 64, SHA256.Create(), salt);

            return hashSHA256;
        }

        public static HashWithSaltResult GetHashDigestWithSalt(string password)
        {
            PasswordWithSaltHasher psHasher = new PasswordWithSaltHasher();
            HashWithSaltResult hashSHA256 = psHasher.HashWithSalt(password, 64, SHA256.Create());

            return hashSHA256;
        }
    }

    public class HashWithSaltResult
    {
        public string Salt { get; set; }

        public string Digest { get; set; }

        public HashWithSaltResult(string salt, string digest)
        {
            Salt = salt;
            Digest = digest;
        }
    }

    public class PasswordWithSaltHasher
    {
        public HashWithSaltResult HashWithSalt(string password, int saltLength, HashAlgorithm hashAlgo, byte[] providedSalt = null)
        {
            RNG rng = new RNG();
            byte[] passwordAsBytes = Encoding.UTF8.GetBytes(password);

            // Get the salt as a bytes stream.
            byte[] saltBytes = providedSalt;
            
            if (providedSalt == null)
            {
                saltBytes = rng.GenerateRandomCryptographicBytes(saltLength);
            }

            // Append salt and password byte stream.
            List<byte> passwordWithSaltBytes = new List<byte>();
            passwordWithSaltBytes.AddRange(passwordAsBytes);
            passwordWithSaltBytes.AddRange(saltBytes);

            byte[] digestBytes = hashAlgo.ComputeHash(passwordWithSaltBytes.ToArray());
            return new HashWithSaltResult(Convert.ToBase64String(saltBytes), Convert.ToBase64String(digestBytes));
        }
    }

    public class RNG
    {
        public string GenerateRandomCryptographicKey(int keyLength)
        {
            return Convert.ToBase64String(GenerateRandomCryptographicBytes(keyLength));
        }

        public byte[] GenerateRandomCryptographicBytes(int keyLength)
        {
            RNGCryptoServiceProvider rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            byte[] randomBytes = new byte[keyLength];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return randomBytes;
        }
    }
}