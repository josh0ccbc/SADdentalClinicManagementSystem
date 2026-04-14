using System;
using System.Security.Cryptography;

namespace M.A_Florencio_Dental_Records
{
    public static class PasswordHelper
    {
        // 🔐 CONFIG
        private const int SaltSize = 16;   // 128-bit
        private const int HashSize = 32;   // 256-bit
        private const int Iterations = 10000;

        // ==========================
        // HASH PASSWORD
        // ==========================
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password cannot be empty.");

            // Generate random salt
            byte[] salt = new byte[SaltSize];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            // Generate hash
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                byte[] hash = pbkdf2.GetBytes(HashSize);

                return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
            }
        }

        // ==========================
        // VERIFY PASSWORD
        // ==========================
        public static bool VerifyPassword(string password, string storedHash)
        {
            try
            {
                // fallback for old SHA256 hashes
                if (!storedHash.Contains(":"))
                {
                    using (var sha256 = SHA256.Create())
                    {
                        var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                        string oldHash = Convert.ToBase64String(hashBytes);
                        return oldHash == storedHash;
                    }
                }

                if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash))
                    return false;

                var parts = storedHash.Split(':');
                if (parts.Length != 2)
                    return false;

                byte[] salt = Convert.FromBase64String(parts[0]);
                byte[] stored = Convert.FromBase64String(parts[1]);

                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
                {
                    byte[] computed = pbkdf2.GetBytes(HashSize);

                    return SlowEquals(stored, computed);
                }
            }
            catch
            {
                return false;
            }
        }

        // ==========================
        // SECURE COMPARISON
        // ==========================
        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;

            for (int i = 0; i < a.Length && i < b.Length; i++)
                diff |= (uint)(a[i] ^ b[i]);

            return diff == 0;
        }
    }
}