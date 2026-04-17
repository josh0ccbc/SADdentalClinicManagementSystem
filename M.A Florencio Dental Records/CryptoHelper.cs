using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace M.A_Florencio_Dental_Records
{
    public static class CryptoHelper
    {
        private const string KEY_NAME = "PatientRecordsKey";
        private const int AES_KEY_BYTES = 32;
        private const int HMAC_KEY_BYTES = 32;
        private const int TOTAL_KEY_BYTES = AES_KEY_BYTES + HMAC_KEY_BYTES;
        private const int IV_BYTES = 16;
        private const int HMAC_BYTES = 32;
        private const int PBKDF2_ITER = 200_000;

        private static readonly DataProtectionScope ProtectionScope = DataProtectionScope.LocalMachine;
        private static readonly object _keyLock = new object();

        public sealed class KeyStorageException : Exception
        {
            public KeyStorageException(string message, Exception inner = null) : base(message, inner) { }
        }

        // ====================== ENCRYPT ======================
        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return "";

            byte[] keyMaterial = null;
            try
            {
                keyMaterial = GetOrCreateKeyMaterial();
                byte[] aesKey = new byte[AES_KEY_BYTES];
                byte[] hmacKey = new byte[HMAC_KEY_BYTES];
                Array.Copy(keyMaterial, 0, aesKey, 0, AES_KEY_BYTES);
                Array.Copy(keyMaterial, AES_KEY_BYTES, hmacKey, 0, HMAC_KEY_BYTES);

                try
                {
                    byte[] iv, cipherBytes;
                    using (Aes aes = Aes.Create())
                    {
                        aes.Key = aesKey;
                        aes.Mode = CipherMode.CBC;
                        aes.Padding = PaddingMode.PKCS7;
                        aes.GenerateIV();
                        iv = aes.IV;

                        using (var ms = new MemoryStream())
                        using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        using (var sw = new StreamWriter(cs, Encoding.UTF8))
                        {
                            sw.Write(plainText);
                            sw.Flush();
                            cs.FlushFinalBlock();
                            cipherBytes = ms.ToArray();
                        }
                    }

                    byte[] mac = ComputeHmac(hmacKey, iv, cipherBytes);

                    byte[] output = new byte[IV_BYTES + cipherBytes.Length + HMAC_BYTES];
                    Array.Copy(iv, 0, output, 0, IV_BYTES);
                    Array.Copy(cipherBytes, 0, output, IV_BYTES, cipherBytes.Length);
                    Array.Copy(mac, 0, output, IV_BYTES + cipherBytes.Length, HMAC_BYTES);

                    return Convert.ToBase64String(output);
                }
                finally
                {
                    Array.Clear(aesKey, 0, aesKey.Length);
                    Array.Clear(hmacKey, 0, hmacKey.Length);
                }
            }
            finally
            {
                if (keyMaterial != null) Array.Clear(keyMaterial, 0, keyMaterial.Length);
            }
        }

        // ====================== DECRYPT ======================
        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return "";

            byte[] keyMaterial = null;
            try
            {
                keyMaterial = GetOrCreateKeyMaterial();
                byte[] aesKey = new byte[AES_KEY_BYTES];
                byte[] hmacKey = new byte[HMAC_KEY_BYTES];
                Array.Copy(keyMaterial, 0, aesKey, 0, AES_KEY_BYTES);
                Array.Copy(keyMaterial, AES_KEY_BYTES, hmacKey, 0, HMAC_KEY_BYTES);

                try
                {
                    byte[] buffer;
                    try { buffer = Convert.FromBase64String(cipherText); }
                    catch { return cipherText; }

                    if (buffer.Length < IV_BYTES + 16 + HMAC_BYTES) return cipherText;

                    byte[] iv = new byte[IV_BYTES];
                    int cipherLen = buffer.Length - IV_BYTES - HMAC_BYTES;
                    byte[] cipherBytes = new byte[cipherLen];
                    byte[] storedMac = new byte[HMAC_BYTES];

                    Array.Copy(buffer, 0, iv, 0, IV_BYTES);
                    Array.Copy(buffer, IV_BYTES, cipherBytes, 0, cipherLen);
                    Array.Copy(buffer, IV_BYTES + cipherLen, storedMac, 0, HMAC_BYTES);

                    if (!CryptographicEquals(ComputeHmac(hmacKey, iv, cipherBytes), storedMac))
                        return "[ENCRYPTED - INTEGRITY FAILURE]";

                    using (Aes aes = Aes.Create())
                    {
                        aes.Key = aesKey;
                        aes.IV = iv;
                        aes.Mode = CipherMode.CBC;
                        aes.Padding = PaddingMode.PKCS7;

                        using (var ms = new MemoryStream(cipherBytes))
                        using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                        using (var sr = new StreamReader(cs, Encoding.UTF8))
                            return sr.ReadToEnd();
                    }
                }
                finally
                {
                    Array.Clear(aesKey, 0, aesKey.Length);
                    Array.Clear(hmacKey, 0, hmacKey.Length);
                }
            }
            catch (KeyStorageException) { throw; }
            catch (CryptographicException ex)
            {
                throw new KeyStorageException("DPAPI failure - Key access error.", ex);
            }
            finally
            {
                if (keyMaterial != null) Array.Clear(keyMaterial, 0, keyMaterial.Length);
            }
        }

        // ====================== KEY MANAGEMENT ======================
        private static byte[] GetOrCreateKeyMaterial()
        {
            lock (_keyLock)
            {
                byte[] protectedKeyData = GetProtectedKeyFromDatabase(KEY_NAME);

                if (protectedKeyData != null && protectedKeyData.Length > 0)
                {
                    byte[] keyMaterial = ProtectedData.Unprotect(protectedKeyData, null, ProtectionScope);

                    if (keyMaterial.Length == AES_KEY_BYTES) // Legacy migration
                    {
                        byte[] migrated = new byte[TOTAL_KEY_BYTES];
                        Array.Copy(keyMaterial, 0, migrated, 0, AES_KEY_BYTES);
                        using (var rng = new RNGCryptoServiceProvider())
                            rng.GetBytes(migrated, AES_KEY_BYTES, HMAC_KEY_BYTES);

                        byte[] newProtected = ProtectedData.Protect(migrated, null, ProtectionScope);
                        DeactivateExistingKeys();
                        SaveProtectedKeyToDatabase(KEY_NAME, newProtected, "LocalMachine-Migrated");

                        Array.Clear(keyMaterial, 0, keyMaterial.Length);
                        return migrated;
                    }

                    if (keyMaterial.Length != TOTAL_KEY_BYTES)
                        throw new InvalidOperationException($"Invalid key length: {keyMaterial.Length}");

                    return keyMaterial;
                }

                // First time key
                byte[] newKey = new byte[TOTAL_KEY_BYTES];
                using (var rng = new RNGCryptoServiceProvider()) rng.GetBytes(newKey);

                byte[] protectedNew = ProtectedData.Protect(newKey, null, ProtectionScope);
                DeactivateExistingKeys();
                SaveProtectedKeyToDatabase(KEY_NAME, protectedNew, "LocalMachine");

                MessageBox.Show("⚠️ First-Time Setup: Encryption Key Generated\n\nPlease export a backup immediately.",
                    "Key Backup Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return newKey;
            }
        }

        internal static byte[] ExportKeyWithPassphrase(string passphrase)
        {
            if (string.IsNullOrEmpty(passphrase)) throw new ArgumentException("Passphrase cannot be empty.");

            byte[] keyMaterial = null;
            try
            {
                keyMaterial = GetOrCreateKeyMaterial();
                byte[] salt = new byte[16];
                using (var rng = new RNGCryptoServiceProvider()) rng.GetBytes(salt);

                byte[] passKey = DeriveKeyFromPassphrase(passphrase, salt, AES_KEY_BYTES);

                try
                {
                    using (Aes aes = Aes.Create())
                    {
                        aes.Key = passKey;
                        aes.Mode = CipherMode.CBC;
                        aes.Padding = PaddingMode.PKCS7;
                        aes.GenerateIV();
                        byte[] iv = aes.IV;

                        using (var ms = new MemoryStream())
                        using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(keyMaterial, 0, keyMaterial.Length);
                            cs.FlushFinalBlock();
                            byte[] encrypted = ms.ToArray();

                            byte[] output = new byte[16 + 16 + encrypted.Length];
                            Array.Copy(salt, 0, output, 0, 16);
                            Array.Copy(iv, 0, output, 16, 16);
                            Array.Copy(encrypted, 0, output, 32, encrypted.Length);
                            return output;
                        }
                    }
                }
                finally { Array.Clear(passKey, 0, passKey.Length); }
            }
            finally
            {
                if (keyMaterial != null) Array.Clear(keyMaterial, 0, keyMaterial.Length);
            }
        }

        internal static void ImportKeyWithPassphrase(byte[] exportBlob, string passphrase)
        {
            if (exportBlob == null || exportBlob.Length < 48)
                throw new ArgumentException("Invalid key backup file.");

            byte[] salt = new byte[16];
            byte[] iv = new byte[16];
            byte[] encrypted = new byte[exportBlob.Length - 32];

            Array.Copy(exportBlob, 0, salt, 0, 16);
            Array.Copy(exportBlob, 16, iv, 0, 16);
            Array.Copy(exportBlob, 32, encrypted, 0, encrypted.Length);

            byte[] passKey = DeriveKeyFromPassphrase(passphrase, salt, AES_KEY_BYTES);
            byte[] keyMaterial = null;

            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = passKey;
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var ms = new MemoryStream(encrypted))
                    using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    using (var outMs = new MemoryStream())
                    {
                        cs.CopyTo(outMs);
                        keyMaterial = outMs.ToArray();
                    }
                }

                if (keyMaterial.Length != TOTAL_KEY_BYTES)
                    throw new CryptographicException("Decrypted key has wrong length.");

                byte[] protectedKey = ProtectedData.Protect(keyMaterial, null, ProtectionScope);

                DeactivateExistingKeys();
                SaveProtectedKeyToDatabase(KEY_NAME, protectedKey, "LocalMachine-Restored");

                MessageBox.Show("✅ Encryption key imported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                Array.Clear(passKey, 0, passKey.Length);
                if (keyMaterial != null) Array.Clear(keyMaterial, 0, keyMaterial.Length);
            }
        }

        // ====================== HELPERS ======================
        private static void DeactivateExistingKeys()
        {
            using (var conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
            {
                conn.Open();
                using (var cmd = new SqlCommand("UPDATE EncryptionKeys SET IsActive = 0 WHERE KeyName = @KeyName", conn))
                {
                    cmd.Parameters.AddWithValue("@KeyName", KEY_NAME);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static byte[] GetProtectedKeyFromDatabase(string keyName)
        {
            using (var conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
            {
                conn.Open();
                const string sql = @"SELECT TOP 1 ProtectedKeyData FROM EncryptionKeys 
                                     WHERE KeyName = @KeyName AND IsActive = 1 
                                     ORDER BY CreatedDate DESC";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@KeyName", keyName);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() && !reader.IsDBNull(0))
                        {
                            long len = reader.GetBytes(0, 0, null, 0, int.MaxValue);
                            byte[] buf = new byte[len];
                            reader.GetBytes(0, 0, buf, 0, (int)len);
                            return buf;
                        }
                    }
                }
            }
            return null;
        }

        private static void SaveProtectedKeyToDatabase(string keyName, byte[] protectedKeyData, string scope)
        {
            using (var conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
            {
                conn.Open();
                const string sql = @"
                    MERGE EncryptionKeys AS target
                    USING (VALUES (@KeyName, @ProtectedData, @Scope, @User, GETDATE(), 1))
                    AS source (KeyName, ProtectedKeyData, DataProtectionScope, CreatedByUser, CreatedDate, IsActive)
                    ON target.KeyName = source.KeyName
                    WHEN MATCHED THEN UPDATE SET ProtectedKeyData = source.ProtectedKeyData, 
                        DataProtectionScope = source.DataProtectionScope, CreatedByUser = source.CreatedByUser, 
                        CreatedDate = source.CreatedDate, IsActive = 1
                    WHEN NOT MATCHED THEN
                        INSERT (KeyName, ProtectedKeyData, DataProtectionScope, CreatedByUser, CreatedDate, IsActive)
                        VALUES (source.KeyName, source.ProtectedKeyData, source.DataProtectionScope, 
                                source.CreatedByUser, source.CreatedDate, source.IsActive);";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@KeyName", keyName);
                    cmd.Parameters.AddWithValue("@ProtectedData", protectedKeyData);
                    cmd.Parameters.AddWithValue("@Scope", scope);
                    cmd.Parameters.AddWithValue("@User", Environment.UserName);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static byte[] ComputeHmac(byte[] hmacKey, byte[] iv, byte[] cipherBytes)
        {
            using (var hmac = new HMACSHA256(hmacKey))
            {
                hmac.TransformBlock(iv, 0, iv.Length, null, 0);
                hmac.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                return hmac.Hash;
            }
        }

        private static bool CryptographicEquals(byte[] a, byte[] b)
        {
            if (a?.Length != b?.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++) diff |= a[i] ^ b[i];
            return diff == 0;
        }

        private static byte[] DeriveKeyFromPassphrase(string passphrase, byte[] salt, int keyBytes)
        {
            using (var kdf = new Rfc2898DeriveBytes(passphrase, salt, PBKDF2_ITER))
                return kdf.GetBytes(keyBytes);
        }
    }
}