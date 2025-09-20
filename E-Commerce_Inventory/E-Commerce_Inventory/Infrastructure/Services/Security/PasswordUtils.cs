using E_Commerce_Inventory.Application.Services;
using System.Security.Cryptography;

namespace E_Commerce_Inventory.Infrastructure.Services.Security
{
    public class PasswordUtils : IPasswordUtils
    {
        private const int SaltSize = 16; // Size of the salt
        private const int HashSize = 32; // Size of the hash
        private const int Iterations = 10000; // Number of iterations for PBKDF2

        public string HashPassword(string password)
        {
            // Generate a random salt
            byte[] salt = new byte[SaltSize];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            // Hash the password with the salt using PBKDF2
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                byte[] hash = pbkdf2.GetBytes(HashSize);

                // Combine the salt and hash into a single array
                byte[] hashBytes = new byte[SaltSize + HashSize];
                Array.Copy(salt, 0, hashBytes, 0, SaltSize);
                Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

                // Convert to Base64 string for storage
                return Convert.ToBase64String(hashBytes);
            }
        }
        public bool VerifyPassword(string password, string storedHash)
        {
            // Convert the stored hash from Base64 string to byte array
            byte[] hashBytes = Convert.FromBase64String(storedHash);

            // Extract the salt from the hash (first part of the byte array)
            byte[] salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            // Extract the hash (second part of the byte array)
            byte[] storedHashBytes = new byte[HashSize];
            Array.Copy(hashBytes, SaltSize, storedHashBytes, 0, HashSize);

            // Hash the incoming password using the extracted salt
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                byte[] computedHash = pbkdf2.GetBytes(HashSize);

                // Compare the hashes (stored vs computed)
                for (int i = 0; i < HashSize; i++)
                {
                    if (storedHashBytes[i] != computedHash[i])
                    {
                        return false; // Passwords do not match
                    }
                }
            }

            return true; // Passwords match
        }
    }
}
