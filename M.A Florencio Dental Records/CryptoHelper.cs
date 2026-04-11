using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Data.SqlClient;

public static class CryptoHelper
{
    // ✅ Key name for storage (not the key itself)
    private const string KEY_NAME = "PatientRecordsKey";
    private const int KEY_SIZE_BYTES = 32; // 256-bit AES

    // Connection string (get from your ConnectionSettings)
    private static string GetConnectionString() =>
        ConnectionSettings.Current.GetConnectionString();

    /// <summary>
    /// Encrypt plaintext using AES-256 with DPAPI-protected key
    /// Each encryption uses a random IV
    /// </summary>
    public static string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return "";

        try
        {
            System.Diagnostics.Debug.WriteLine("[ENCRYPT] Starting encryption");

            // Step 1: Get or create the protected key
            // ✅ No null check needed — GetOrCreateAesKey() now throws instead of returning null
            byte[] aesKey = GetOrCreateAesKey();
            System.Diagnostics.Debug.WriteLine($"[ENCRYPT] AES key size: {aesKey.Length} bytes");

            // Step 2: Use AES with random IV
            using (Aes aes = Aes.Create())
            {
                aes.Key = aesKey;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                // ✅ Generate new random IV each time (CRITICAL)
                aes.GenerateIV();
                System.Diagnostics.Debug.WriteLine($"[ENCRYPT] Generated random IV: {Convert.ToBase64String(aes.IV)}");

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (var ms = new MemoryStream())
                {
                    // ✅ Prepend IV to ciphertext (so decryption knows which IV was used)
                    ms.Write(aes.IV, 0, aes.IV.Length);
                    System.Diagnostics.Debug.WriteLine($"[ENCRYPT] Wrote IV ({aes.IV.Length} bytes) to output");

                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                        sw.Flush();
                        cs.FlushFinalBlock();
                    }

                    byte[] encryptedBytes = ms.ToArray();
                    string result = Convert.ToBase64String(encryptedBytes);

                    System.Diagnostics.Debug.WriteLine($"[ENCRYPT] ✅ Encrypted {plainText.Length} chars → {result.Length} chars (base64)");
                    return result;
                }
            }
        }
        catch (CryptographicException ex)
        {
            // ✅ DPAPI failure — cannot access key on this machine/user
            System.Diagnostics.Debug.WriteLine($"[ENCRYPT] ❌ DPAPI/Crypto failure: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"[ENCRYPT] Stack: {ex.StackTrace}");

            System.Windows.Forms.MessageBox.Show(
                "❌ Cannot encrypt patient data.\n\n" +
                "This may happen if:\n" +
                "  • The app was moved to a new PC without restoring the key backup\n" +
                "  • You are logged in as a different Windows user\n" +
                "  • The encryption key was corrupted\n\n" +
                "Go to: Admin Panel → Security → Import Key Backup\n\n" +
                "Contact your system administrator if this persists.",
                "Encryption Error — Action Required",
                System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Error);

            return ""; // ✅ Return empty — do NOT save corrupted/unencrypted data
        }
        catch (Exception ex)
        {
            // ✅ Unexpected error — log it but don't crash the app
            System.Diagnostics.Debug.WriteLine($"[ENCRYPT] ❌ Unexpected error: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"[ENCRYPT] Stack: {ex.StackTrace}");
            return "";
        }
    }

    /// <summary>
    /// Decrypt ciphertext using AES-256 with DPAPI-protected key
    /// Extracts IV from beginning of ciphertext
    /// </summary>
    public static string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return "";

        try
        {
            System.Diagnostics.Debug.WriteLine("[DECRYPT] Starting decryption");

            // Step 1: Get the protected key
            // ✅ No null check needed — GetOrCreateAesKey() now throws instead of returning null
            byte[] aesKey = GetOrCreateAesKey();
            System.Diagnostics.Debug.WriteLine($"[DECRYPT] AES key size: {aesKey.Length} bytes");

            // Step 2: Decode from base64 and extract IV
            byte[] buffer = Convert.FromBase64String(cipherText);
            System.Diagnostics.Debug.WriteLine($"[DECRYPT] Decoded {buffer.Length} bytes from base64");

            if (buffer.Length < 16)
                throw new Exception("Invalid ciphertext: too short (no IV)");

            // ✅ Extract IV from first 16 bytes
            byte[] iv = new byte[16];
            Array.Copy(buffer, 0, iv, 0, 16);
            System.Diagnostics.Debug.WriteLine($"[DECRYPT] Extracted IV: {Convert.ToBase64String(iv)}");

            // Step 3: Decrypt
            using (Aes aes = Aes.Create())
            {
                aes.Key = aesKey;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                // ✅ Decrypt from byte 16 onward (skip IV)
                using (var ms = new MemoryStream(buffer, 16, buffer.Length - 16))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    string result = sr.ReadToEnd();
                    System.Diagnostics.Debug.WriteLine($"[DECRYPT] ✅ Decrypted → {result.Length} chars");
                    return result;
                }
            }
        }
        catch (CryptographicException ex)
        {
            // ✅ DPAPI failure — wrong machine or wrong Windows user
            // This is the most dangerous failure — data exists but key cannot be unprotected
            System.Diagnostics.Debug.WriteLine($"[DECRYPT] ❌ DPAPI/Crypto failure: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"[DECRYPT] Stack: {ex.StackTrace}");

            System.Windows.Forms.MessageBox.Show(
                "❌ Cannot decrypt patient data.\n\n" +
                "This may happen if:\n" +
                "  • The app was moved to a new PC without restoring the key backup\n" +
                "  • You are logged in as a different Windows user\n" +
                "  • The encryption key was corrupted\n\n" +
                "Go to: Admin Panel → Security → Import Key Backup\n\n" +
                "Contact your system administrator if this persists.",
                "Decryption Error — Action Required",
                System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Error);

            return "[ENCRYPTED - KEY MISMATCH]"; // ✅ Visible placeholder, not silent empty string
        }
        catch (FormatException ex)
        {
            // ✅ Base64 decode failed — data is corrupt or was never encrypted (legacy data)
            System.Diagnostics.Debug.WriteLine($"[DECRYPT] ❌ Base64 format error: {ex.Message}");
            return cipherText; // ✅ Return as-is (SafeDecrypt fallback behavior for legacy data)
        }
        catch (Exception ex)
        {
            // ✅ Unexpected error — log it but don't crash the app
            System.Diagnostics.Debug.WriteLine($"[DECRYPT] ❌ Unexpected error: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"[DECRYPT] Stack: {ex.StackTrace}");
            return "";
        }
    }

    /// <summary>
    /// Get existing AES key or create new one (protected with DPAPI)
    /// Scope: CurrentUser (each Windows user has isolated key)
    /// </summary>
    private static byte[] GetOrCreateAesKey()
    {
        System.Diagnostics.Debug.WriteLine("[KEY] GetOrCreateAesKey() called");

        // Step 1: Check if key exists in database
        byte[] protectedKeyData = GetProtectedKeyFromDatabase(KEY_NAME);

        if (protectedKeyData != null && protectedKeyData.Length > 0)
        {
            System.Diagnostics.Debug.WriteLine("[KEY] ✅ Found existing protected key in database");

            // ✅ NO try-catch here — if DPAPI fails, we MUST NOT silently create a new key
            // A failure here means: wrong machine, wrong user, or corrupt key
            // Generating a new key would make ALL existing patient data permanently unreadable
            byte[] unprotectedKey = ProtectedData.Unprotect(
                protectedKeyData,
                null,
                DataProtectionScope.CurrentUser);

            System.Diagnostics.Debug.WriteLine($"[KEY] ✅ Unprotected key: {unprotectedKey.Length} bytes");
            return unprotectedKey;
        }

        // Step 2: Only reaches here on a FRESH INSTALL (no key in DB at all)
        System.Diagnostics.Debug.WriteLine("[KEY] ⚠️ No existing key found — this should only happen on first run");

        byte[] newKey = new byte[KEY_SIZE_BYTES];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(newKey);
        }

        System.Diagnostics.Debug.WriteLine($"[KEY] ✅ Generated new AES key: {newKey.Length} bytes");

        // Step 3: Protect with DPAPI
        byte[] protectedNewKey = ProtectedData.Protect(
            newKey,
            null,
            DataProtectionScope.CurrentUser);

        System.Diagnostics.Debug.WriteLine($"[KEY] ✅ Protected key: {protectedNewKey.Length} bytes");

        // Step 4: Store in database
        SaveProtectedKeyToDatabase(KEY_NAME, protectedNewKey, "CurrentUser");
        System.Diagnostics.Debug.WriteLine("[KEY] ✅ Saved protected key to database");

        // ✅ PROMPT ADMIN TO BACK UP KEY IMMEDIATELY AFTER FIRST CREATION
        System.Windows.Forms.MessageBox.Show(
            "⚠️ First-Time Setup Detected\n\n" +
            "An encryption key has been generated to protect all patient data.\n\n" +
            "IMPORTANT: You must export a key backup immediately.\n" +
            "Go to: Admin Panel → Security → Export Key Backup\n\n" +
            "Without this backup, data cannot be recovered if this PC is replaced.",
            "Action Required: Key Backup",
            System.Windows.Forms.MessageBoxButtons.OK,
            System.Windows.Forms.MessageBoxIcon.Warning);

        return newKey;
    }

    /// <summary>
    /// Retrieve protected key data from database
    /// </summary>
    private static byte[] GetProtectedKeyFromDatabase(string keyName)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"[DB] GetProtectedKeyFromDatabase('{keyName}')");

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = @"
                    SELECT ProtectedKeyData 
                    FROM EncryptionKeys 
                    WHERE KeyName = @KeyName AND IsActive = 1
                    ORDER BY CreatedDate DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@KeyName", keyName);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() && !reader.IsDBNull(0))
                        {
                            int length = (int)reader.GetBytes(0, 0, null, 0, int.MaxValue);
                            byte[] buffer = new byte[length];
                            reader.GetBytes(0, 0, buffer, 0, length);

                            System.Diagnostics.Debug.WriteLine($"[DB] ✅ Retrieved protected key: {buffer.Length} bytes");
                            return buffer;
                        }
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"[DB] ⚠️ No key found for '{keyName}'");
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DB] ❌ Error retrieving key: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Save protected key to database
    /// </summary>
    private static void SaveProtectedKeyToDatabase(string keyName, byte[] protectedKeyData, string scope)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"[DB] SaveProtectedKeyToDatabase('{keyName}', scope='{scope}')");

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = @"
                    INSERT INTO EncryptionKeys 
                    (KeyName, ProtectedKeyData, DataProtectionScope, CreatedByUser, IsActive)
                    VALUES 
                    (@KeyName, @ProtectedData, @Scope, @User, 1)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@KeyName", keyName);
                    cmd.Parameters.AddWithValue("@ProtectedData", protectedKeyData);
                    cmd.Parameters.AddWithValue("@Scope", scope);
                    cmd.Parameters.AddWithValue("@User", Environment.UserName);

                    cmd.ExecuteNonQuery();
                    System.Diagnostics.Debug.WriteLine("[DB] ✅ Protected key saved to database");
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DB] ❌ Error saving key: {ex.Message}");
            throw;
        }
    }
    /// <summary>
    /// Exposes the raw AES key for backup purposes only.
    /// Only callable from KeyBackupManager.
    /// </summary>
    internal static byte[] GetAesKeyForBackup()
    {
        return GetOrCreateAesKey(); // Now throws on DPAPI failure — safe
    }

    /// <summary>
    /// Deactivates all existing key records before restoring a backup.
    /// </summary>
    internal static void DeactivateExistingKeys()
    {
        using (var conn = new SqlConnection(GetConnectionString()))
        {
            conn.Open();
            string sql = "UPDATE EncryptionKeys SET IsActive = 0 WHERE KeyName = @KeyName";
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@KeyName", KEY_NAME);
                cmd.ExecuteNonQuery();
            }
        }
    }

    /// <summary>
    /// Saves a restored key after successful backup import.
    /// </summary>
    internal static void SaveRestoredKey(byte[] protectedKeyData)
    {
        SaveProtectedKeyToDatabase(KEY_NAME, protectedKeyData, "CurrentUser-Restored");
    }
}

