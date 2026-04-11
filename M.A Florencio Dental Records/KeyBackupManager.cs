using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

public static class KeyBackupManager
{
    private const string BACKUP_FILE_HEADER = "DENTAL_KEY_BACKUP_V1";

    /// <summary>
    /// Export the AES key to an encrypted backup file.
    /// The backup file is protected by an admin password — NOT by DPAPI.
    /// This means it can be restored on ANY machine.
    /// </summary>
    public static bool ExportKeyBackup(string outputPath, string adminPassword)
    {
        try
        {
            // Step 1: Get the real AES key via DPAPI (only works on current machine)
            byte[] aesKey = CryptoHelper.GetAesKeyForBackup();
            if (aesKey == null || aesKey.Length == 0)
                throw new Exception("Could not retrieve encryption key from database.");

            // Step 2: Generate a random salt for key derivation
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(salt);

            // Step 3: Derive an encryption key from the admin password
            using (var pdb = new Rfc2898DeriveBytes(adminPassword, salt, 100000))
            {
                byte[] passwordKey = pdb.GetBytes(32); // 256-bit
                byte[] passwordIV = pdb.GetBytes(16); // 128-bit

                using (Aes aes = Aes.Create())
                {
                    aes.Key = passwordKey;
                    aes.IV = passwordIV;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var fs = new FileStream(outputPath, FileMode.Create))
                    using (var bw = new BinaryWriter(fs))
                    {
                        // Step 4: Write file header (identifies valid backup files)
                        bw.Write(Encoding.UTF8.GetBytes(BACKUP_FILE_HEADER));

                        // Step 5: Write salt (needed to re-derive password key on import)
                        bw.Write(salt);

                        // Step 6: Write creation metadata
                        bw.Write(DateTime.UtcNow.ToBinary());
                        bw.Write(Encoding.UTF8.GetByteCount(Environment.MachineName));
                        bw.Write(Encoding.UTF8.GetBytes(Environment.MachineName));

                        // Step 7: Write encrypted AES key
                        using (var cs = new CryptoStream(fs, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(aesKey, 0, aesKey.Length);
                            cs.FlushFinalBlock();
                        }
                    }
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[BACKUP EXPORT] ❌ {ex.Message}");
            throw; // Let the form handle the error message
        }
    }

    /// <summary>
    /// Import a key backup file and re-protect it with DPAPI on the current machine.
    /// After this runs, the app can decrypt all existing patient data normally.
    /// </summary>
    public static bool ImportKeyBackup(string backupPath, string adminPassword)
    {
        try
        {
            byte[] recoveredKey;

            using (var fs = new FileStream(backupPath, FileMode.Open, FileAccess.Read))
            using (var br = new BinaryReader(fs))
            {
                // Step 1: Verify file header
                byte[] headerBytes = br.ReadBytes(Encoding.UTF8.GetByteCount(BACKUP_FILE_HEADER));
                string header = Encoding.UTF8.GetString(headerBytes);
                if (header != BACKUP_FILE_HEADER)
                    throw new Exception("Invalid backup file. This file was not created by this application.");

                // Step 2: Read salt
                byte[] salt = br.ReadBytes(16);

                // Step 3: Read metadata (skip — just for record keeping)
                long createdBinary = br.ReadInt64();
                int machineNameLength = br.ReadInt32();
                string originalMachine = Encoding.UTF8.GetString(br.ReadBytes(machineNameLength));
                System.Diagnostics.Debug.WriteLine($"[BACKUP IMPORT] Key was created on: {originalMachine}");
                System.Diagnostics.Debug.WriteLine($"[BACKUP IMPORT] Created: {DateTime.FromBinary(createdBinary)}");

                // Step 4: Re-derive password key from admin password + salt
                using (var pdb = new Rfc2898DeriveBytes(adminPassword, salt, 100000))
                {
                    byte[] passwordKey = pdb.GetBytes(32);
                    byte[] passwordIV = pdb.GetBytes(16);

                    using (Aes aes = Aes.Create())
                    {
                        aes.Key = passwordKey;
                        aes.IV = passwordIV;
                        aes.Mode = CipherMode.CBC;
                        aes.Padding = PaddingMode.PKCS7;

                        // Step 5: Decrypt the AES key
                        recoveredKey = new byte[32];
                        using (var cs = new CryptoStream(fs, aes.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            int bytesRead = cs.Read(recoveredKey, 0, 32);
                            if (bytesRead != 32)
                                throw new Exception("Key recovery failed. Wrong password or corrupted file.");
                        }
                    }
                }
            }

            // Step 6: Re-protect with DPAPI on THIS machine
            byte[] newProtectedKey = ProtectedData.Protect(
                recoveredKey,
                null,
                DataProtectionScope.CurrentUser);

            // Step 7: Deactivate old key record, save new one
            CryptoHelper.DeactivateExistingKeys();
            CryptoHelper.SaveRestoredKey(newProtectedKey);

            System.Diagnostics.Debug.WriteLine("[BACKUP IMPORT] ✅ Key restored and re-protected successfully");
            return true;
        }
        catch (CryptographicException)
        {
            // Wrong password produces a crypto error during decryption
            throw new Exception("Wrong password. Please check your admin backup password and try again.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[BACKUP IMPORT] ❌ {ex.Message}");
            throw;
        }
    }
}