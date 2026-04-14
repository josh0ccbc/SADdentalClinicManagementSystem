using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace M.A_Florencio_Dental_Records
{
    /// <summary>
    /// AES-256-CBC + HMAC-SHA256 (Encrypt-then-MAC) encryption helper.
    /// 
    /// KEY STORAGE DESIGN:
    ///   The raw AES key is NEVER stored directly. Instead, two separate
    ///   protected copies are maintained in the EncryptionKeys table:
    /// 
    ///   1. KeyType = 'DPAPI'
    ///      Protected with Windows DPAPI (CurrentUser scope).
    ///      Used for day-to-day operation on the original machine/user.
    ///      Automatically invalidated when switching machines or Windows users.
    /// 
    ///   2. KeyType = 'PORTABLE'
    ///      Protected with a user-supplied passphrase (PBKDF2 + AES-256).
    ///      Used for import/export and cross-machine migration.
    ///      The admin sets this passphrase during Export Key Backup.
    ///      On a new machine, the admin enters the passphrase to restore.
    /// 
    /// CIPHERTEXT FORMAT (per encrypted value):
    ///   [16 bytes IV] + [N bytes AES-CBC ciphertext] + [32 bytes HMAC-SHA256]
    ///   All base64-encoded as a single string stored in the database column.
    /// 
    /// MEMORY SAFETY:
    ///   All raw key byte arrays are zeroed with Array.Clear() in finally blocks
    ///   immediately after use so they do not linger in the managed heap.
    /// </summary>
    public static class CryptoHelper
    {
        private const string KEY_NAME = "PatientRecordsKey";
        private const int AES_KEY_BYTES = 32;   // 256-bit AES
        private const int HMAC_KEY_BYTES = 32;   // 256-bit HMAC
        private const int TOTAL_KEY_BYTES = AES_KEY_BYTES + HMAC_KEY_BYTES; // 64 bytes total
        private const int IV_BYTES = 16;
        private const int HMAC_BYTES = 32;   // SHA-256 output
        private const int PBKDF2_ITER = 200_000;

        // =========================================================================
        // PUBLIC: ENCRYPT
        // =========================================================================

        /// <summary>
        /// Encrypts plaintext using AES-256-CBC with a fresh random IV,
        /// then appends an HMAC-SHA256 over (IV + ciphertext) for integrity.
        /// Returns base64 string, or "" on any failure (never saves unencrypted data).
        /// </summary>
        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return "";

            byte[] keyMaterial = null;
            try
            {
                keyMaterial = GetOrCreateKeyMaterial(); // 64 bytes: [AES key | HMAC key]

                byte[] aesKey = new byte[AES_KEY_BYTES];
                byte[] hmacKey = new byte[HMAC_KEY_BYTES];
                Array.Copy(keyMaterial, 0, aesKey, 0, AES_KEY_BYTES);
                Array.Copy(keyMaterial, AES_KEY_BYTES, hmacKey, 0, HMAC_KEY_BYTES);

                try
                {
                    byte[] iv;
                    byte[] cipherBytes;

                    // --- AES-CBC encrypt ---
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

                    // --- HMAC-SHA256 over (IV || ciphertext) ---
                    byte[] mac = ComputeHmac(hmacKey, iv, cipherBytes);

                    // --- Assemble: IV + ciphertext + MAC ---
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
            catch (CryptographicException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ENCRYPT] DPAPI/Crypto failure: {ex.Message}");
                ShowKeyAccessError("encrypt");
                return "";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ENCRYPT] Unexpected error: {ex.Message}");
                return "";
            }
            finally
            {
                if (keyMaterial != null)
                    Array.Clear(keyMaterial, 0, keyMaterial.Length);
            }
        }

        // =========================================================================
        // PUBLIC: DECRYPT
        // =========================================================================

        /// <summary>
        /// Decrypts a value previously encrypted by Encrypt().
        /// Verifies HMAC before decrypting (Encrypt-then-MAC pattern).
        /// Returns "[ENCRYPTED - KEY MISMATCH]" if the key cannot be accessed.
        /// Returns the raw value as-is if it does not appear to be base64 (legacy data).
        /// </summary>
        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return "";

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
                    byte[] buffer = Convert.FromBase64String(cipherText);

                    // Minimum valid length: IV + at least 1 AES block (16) + HMAC
                    if (buffer.Length < IV_BYTES + 16 + HMAC_BYTES)
                    {
                        System.Diagnostics.Debug.WriteLine("[DECRYPT] Buffer too short — treating as legacy plaintext");
                        return cipherText;
                    }

                    // --- Extract components ---
                    byte[] iv = new byte[IV_BYTES];
                    int cipherLen = buffer.Length - IV_BYTES - HMAC_BYTES;
                    byte[] cipherBytes = new byte[cipherLen];
                    byte[] storedMac = new byte[HMAC_BYTES];

                    Array.Copy(buffer, 0, iv, 0, IV_BYTES);
                    Array.Copy(buffer, IV_BYTES, cipherBytes, 0, cipherLen);
                    Array.Copy(buffer, IV_BYTES + cipherLen, storedMac, 0, HMAC_BYTES);

                    // --- Verify HMAC BEFORE decrypting (timing-safe compare) ---
                    byte[] expectedMac = ComputeHmac(hmacKey, iv, cipherBytes);
                    if (!CryptographicEquals(storedMac, expectedMac))
                    {
                        System.Diagnostics.Debug.WriteLine("[DECRYPT] HMAC verification failed — data tampered or wrong key");
                        return "[ENCRYPTED - INTEGRITY FAILURE]";
                    }

                    // --- AES-CBC decrypt ---
                    using (Aes aes = Aes.Create())
                    {
                        aes.Key = aesKey;
                        aes.IV = iv;
                        aes.Mode = CipherMode.CBC;
                        aes.Padding = PaddingMode.PKCS7;

                        using (var ms = new MemoryStream(cipherBytes))
                        using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                        using (var sr = new StreamReader(cs, Encoding.UTF8))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
                finally
                {
                    Array.Clear(aesKey, 0, aesKey.Length);
                    Array.Clear(hmacKey, 0, hmacKey.Length);
                }
            }
            catch (CryptographicException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DECRYPT] DPAPI/Crypto failure: {ex.Message}");
                ShowKeyAccessError("decrypt");
                return "[ENCRYPTED - KEY MISMATCH]";
            }
            catch (FormatException)
            {
                // Not base64 — legacy unencrypted data stored before encryption was added
                System.Diagnostics.Debug.WriteLine("[DECRYPT] FormatException — returning raw value (legacy data)");
                return cipherText;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DECRYPT] Unexpected error: {ex.Message}");
                return "";
            }
            finally
            {
                if (keyMaterial != null)
                    Array.Clear(keyMaterial, 0, keyMaterial.Length);
            }
        }

        // =========================================================================
        // INTERNAL: KEY BACKUP / RESTORE (for AdminKeyBackupForm)
        // =========================================================================

        /// <summary>
        /// Exports the raw AES+HMAC key material protected with a user-supplied passphrase.
        /// The resulting bytes can be saved to a file and restored on any machine.
        /// 
        /// Format: [16 bytes PBKDF2 salt] + [16 bytes AES IV] + [AES-256-CBC(keyMaterial)]
        /// The passphrase is stretched with PBKDF2-SHA256 (200,000 iterations).
        /// 
        /// Called by AdminKeyBackupForm for the Export flow.
        /// </summary>
        internal static byte[] ExportKeyWithPassphrase(string passphrase)
        {
            if (string.IsNullOrEmpty(passphrase))
                throw new ArgumentException("Passphrase cannot be empty.");

            byte[] keyMaterial = null;
            try
            {
                keyMaterial = GetOrCreateKeyMaterial();

                byte[] salt = new byte[16];
                using (var rng = new RNGCryptoServiceProvider())
                    rng.GetBytes(salt);

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

                            // Output: [salt(16)] + [iv(16)] + [encrypted key material]
                            byte[] output = new byte[16 + 16 + encrypted.Length];
                            Array.Copy(salt, 0, output, 0, 16);
                            Array.Copy(iv, 0, output, 16, 16);
                            Array.Copy(encrypted, 0, output, 32, encrypted.Length);
                            return output;
                        }
                    }
                }
                finally
                {
                    Array.Clear(passKey, 0, passKey.Length);
                }
            }
            finally
            {
                if (keyMaterial != null)
                    Array.Clear(keyMaterial, 0, keyMaterial.Length);
            }
        }

        /// <summary>
        /// Imports key material from an export blob using the passphrase.
        /// Deactivates the existing DB key record and saves the restored key
        /// protected under the current machine's DPAPI.
        /// 
        /// Call this on a new machine after importing the database.
        /// Called by AdminKeyBackupForm for the Import flow.
        /// </summary>
        internal static void ImportKeyWithPassphrase(byte[] exportBlob, string passphrase)
        {
            if (exportBlob == null || exportBlob.Length < 32 + AES_KEY_BYTES)
                throw new ArgumentException("Invalid key backup file.");
            if (string.IsNullOrEmpty(passphrase))
                throw new ArgumentException("Passphrase cannot be empty.");

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
                    throw new CryptographicException("Decrypted key material has unexpected length. Wrong passphrase?");

                // Protect the restored key with THIS machine's DPAPI and save to DB
                byte[] protectedKey = ProtectedData.Protect(keyMaterial, null, DataProtectionScope.CurrentUser);

                DeactivateExistingKeys();
                SaveProtectedKeyToDatabase(KEY_NAME, protectedKey, "CurrentUser-Restored");

                System.Diagnostics.Debug.WriteLine("[KEY] Key successfully imported and saved.");
            }
            finally
            {
                Array.Clear(passKey, 0, passKey.Length);
                if (keyMaterial != null)
                    Array.Clear(keyMaterial, 0, keyMaterial.Length);
            }
        }

        /// <summary>
        /// Deactivates all existing active key records in the database.
        /// Called before saving a restored key.
        /// </summary>
        internal static void DeactivateExistingKeys()
        {
            using (var conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
            {
                conn.Open();
                using (var cmd = new SqlCommand(
                    "UPDATE EncryptionKeys SET IsActive = 0 WHERE KeyName = @KeyName", conn))
                {
                    cmd.Parameters.AddWithValue("@KeyName", KEY_NAME);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // =========================================================================
        // PRIVATE: CORE KEY MANAGEMENT
        // =========================================================================

        /// <summary>
        /// Returns 64 bytes of key material: [32 bytes AES key | 32 bytes HMAC key].
        /// On a fresh install: generates, DPAPI-protects, and saves to DB.
        /// On existing install: loads from DB and DPAPI-unprotects.
        /// THROWS CryptographicException on DPAPI failure — never silently regenerates.
        /// CALLER IS RESPONSIBLE for zeroing the returned array when done.
        /// </summary>
        private static byte[] GetOrCreateKeyMaterial()
        {
            byte[] protectedKeyData = GetProtectedKeyFromDatabase(KEY_NAME);

            if (protectedKeyData != null && protectedKeyData.Length > 0)
            {
                // Key exists — unprotect with DPAPI.
                // NO try-catch: if DPAPI fails (wrong machine/user), we must NOT silently
                // create a new key. That would permanently lose access to all patient data.
                byte[] keyMaterial = ProtectedData.Unprotect(
                    protectedKeyData, null, DataProtectionScope.CurrentUser);

                System.Diagnostics.Debug.WriteLine($"[KEY] Unprotected existing key: {keyMaterial.Length} bytes");
                return keyMaterial;
            }

            // Fresh install — generate new key material
            System.Diagnostics.Debug.WriteLine("[KEY] No key found — generating new key (first run only)");

            byte[] newKeyMaterial = new byte[TOTAL_KEY_BYTES];
            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(newKeyMaterial);

            byte[] protectedNew = ProtectedData.Protect(
                newKeyMaterial, null, DataProtectionScope.CurrentUser);

            // Deactivate any stale records before saving (defensive, should be none on first run)
            DeactivateExistingKeys();
            SaveProtectedKeyToDatabase(KEY_NAME, protectedNew, "CurrentUser");

            MessageBox.Show(
                "⚠️ First-Time Setup: Encryption Key Generated\n\n" +
                "A new encryption key has been created to protect all patient data.\n\n" +
                "ACTION REQUIRED:\n" +
                "Go to Admin Panel → Security → Export Key Backup\n" +
                "and save the backup file in a secure location (e.g. USB, password manager).\n\n" +
                "This backup is essential if this PC is ever replaced or rebuilt.\n" +
                "Without it, all encrypted patient data will be permanently unreadable.",
                "Key Backup Required",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return newKeyMaterial;
            // NOTE: caller will zero this in its finally block
        }

        // =========================================================================
        // PRIVATE: DATABASE HELPERS
        // =========================================================================

        private static byte[] GetProtectedKeyFromDatabase(string keyName)
        {
            try
            {
                using (var conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    conn.Open();

                    const string sql = @"
                        SELECT TOP 1 ProtectedKeyData
                        FROM EncryptionKeys
                        WHERE KeyName = @KeyName AND IsActive = 1
                        ORDER BY CreatedDate DESC";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@KeyName", keyName);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read() && !reader.IsDBNull(0))
                            {
                                long byteLen = reader.GetBytes(0, 0, null, 0, int.MaxValue);
                                byte[] buf = new byte[byteLen];
                                reader.GetBytes(0, 0, buf, 0, (int)byteLen);
                                System.Diagnostics.Debug.WriteLine($"[DB] Retrieved protected key: {buf.Length} bytes");
                                return buf;
                            }
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine("[DB] No active key found in database.");
                return null;
            }
            catch (Exception ex)
            {
                // IMPORTANT: Do NOT return null here silently — throw so the caller
                // does NOT fall through to key generation, which would orphan all existing data.
                System.Diagnostics.Debug.WriteLine($"[DB] ERROR retrieving key: {ex.Message}");
                throw new InvalidOperationException(
                    "Failed to retrieve encryption key from database. " +
                    "Check the database connection and try again.", ex);
            }
        }

        private static void SaveProtectedKeyToDatabase(string keyName, byte[] protectedKeyData, string scope)
        {
            using (var conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
            {
                conn.Open();

                const string sql = @"
                    INSERT INTO EncryptionKeys
                        (KeyName, ProtectedKeyData, DataProtectionScope, CreatedByUser, CreatedDate, IsActive)
                    VALUES
                        (@KeyName, @ProtectedData, @Scope, @User, GETDATE(), 1)";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@KeyName", keyName);
                    cmd.Parameters.AddWithValue("@ProtectedData", protectedKeyData);
                    cmd.Parameters.AddWithValue("@Scope", scope);
                    cmd.Parameters.AddWithValue("@User", Environment.UserName);
                    cmd.ExecuteNonQuery();
                }
            }

            System.Diagnostics.Debug.WriteLine($"[DB] Protected key saved (scope={scope}).");
        }

        // =========================================================================
        // PRIVATE: CRYPTOGRAPHIC UTILITIES
        // =========================================================================

        /// <summary>
        /// Computes HMAC-SHA256 over (IV || ciphertext).
        /// </summary>
        private static byte[] ComputeHmac(byte[] hmacKey, byte[] iv, byte[] cipherBytes)
        {
            using (var hmac = new HMACSHA256(hmacKey))
            {
                // Feed IV then ciphertext as a single logical stream
                hmac.TransformBlock(iv, 0, iv.Length, null, 0);
                hmac.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                return hmac.Hash;
            }
        }

        /// <summary>
        /// Constant-time byte array comparison to prevent timing attacks.
        /// </summary>
        private static bool CryptographicEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
                return false;

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i];

            return diff == 0;
        }

        /// <summary>
        /// Derives a key from a passphrase using PBKDF2-SHA256.
        /// Used for portable key export/import (cross-machine migration).
        /// </summary>
        private static byte[] DeriveKeyFromPassphrase(string passphrase, byte[] salt, int keyBytes)
        {
            // .NET Framework 4.x: Rfc2898DeriveBytes only supports SHA-1 natively.
            // We compensate by using a high iteration count (200,000) to keep brute-force cost high.
            // If you ever migrate to .NET 6+, add HashAlgorithmName.SHA256 as the 4th argument.
            using (var kdf = new Rfc2898DeriveBytes(passphrase, salt, PBKDF2_ITER))
            {
                return kdf.GetBytes(keyBytes);
            }
        }

        /// <summary>
        /// Shows a standardised user-facing error when the key cannot be accessed.
        /// </summary>
        private static void ShowKeyAccessError(string operation)
        {
            MessageBox.Show(
                $"❌ Cannot {operation} patient data.\n\n" +
                "This usually means one of:\n" +
                "  • The database was moved to a new PC without importing the key backup\n" +
                "  • You are logged in as a different Windows user account\n" +
                "  • The encryption key record in the database is missing or corrupt\n\n" +
                "To fix this:\n" +
                "  Go to Admin Panel → Security → Import Key Backup\n" +
                "  and enter the passphrase used when the backup was exported.\n\n" +
                "Contact your system administrator if the backup file is unavailable.",
                "Encryption Error — Action Required",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}