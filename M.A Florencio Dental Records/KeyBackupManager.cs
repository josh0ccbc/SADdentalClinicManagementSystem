using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace M.A_Florencio_Dental_Records
{
    /// <summary>
    /// Handles export and import of the AES+HMAC key material to/from a .dkb backup file.
    ///
    /// FILE FORMAT (binary):
    ///   [20 bytes]  ASCII header "DENTAL_KEY_BACKUP_V2"
    ///   [8  bytes]  Creation timestamp (DateTime.UtcNow.ToBinary())
    ///   [4  bytes]  Machine name byte length (int32)
    ///   [N  bytes]  Machine name (UTF-8)
    ///   [4  bytes]  Export blob length (int32)
    ///   [?  bytes]  Export blob from CryptoHelper.ExportKeyWithPassphrase()
    ///               Internal: [16 bytes salt] + [16 bytes IV] + [AES-CBC ciphertext of 64-byte key material]
    ///
    /// Key material is 64 bytes: [32 bytes AES key | 32 bytes HMAC key].
    /// Protected by admin passphrase via PBKDF2 — NOT DPAPI.
    /// Portable across any machine.
    /// </summary>
    public static class KeyBackupManager
    {
        private const string BACKUP_FILE_HEADER = "DENTAL_KEY_BACKUP_V2";

        // =====================================================================
        // EXPORT
        // =====================================================================

        /// <summary>
        /// Exports the encryption key material to a password-protected .dkb file.
        /// Delegates key wrapping to CryptoHelper.ExportKeyWithPassphrase().
        /// Returns true on success, throws on failure.
        /// </summary>
        public static bool ExportKeyBackup(string outputPath, string adminPassword)
        {
            if (string.IsNullOrWhiteSpace(outputPath))
                throw new ArgumentException("Output path cannot be empty.");
            if (string.IsNullOrWhiteSpace(adminPassword))
                throw new ArgumentException("Password cannot be empty.");

            // Ensure destination directory exists
            string dir = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            try
            {
                // Step 1: Get the passphrase-protected key blob from CryptoHelper
                // Format: [16 bytes salt] + [16 bytes IV] + [encrypted key material]
                byte[] exportBlob = CryptoHelper.ExportKeyWithPassphrase(adminPassword);

                // Step 2: Write the .dkb file with header + metadata + blob
                using (var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                using (var bw = new BinaryWriter(fs, Encoding.UTF8))
                {
                    // Header — identifies valid backup files and format version
                    bw.Write(Encoding.UTF8.GetBytes(BACKUP_FILE_HEADER));

                    // Metadata: timestamp and source machine name (informational only)
                    bw.Write(DateTime.UtcNow.ToBinary());

                    byte[] machineBytes = Encoding.UTF8.GetBytes(Environment.MachineName);
                    bw.Write(machineBytes.Length);
                    bw.Write(machineBytes);

                    // Payload: length-prefixed passphrase-protected key blob
                    bw.Write(exportBlob.Length);
                    bw.Write(exportBlob);
                }

                System.Diagnostics.Debug.WriteLine($"[BACKUP EXPORT] ✅ Key exported to: {outputPath}");
                return true;
            }
            catch (CryptographicException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BACKUP EXPORT] ❌ DPAPI failure: {ex.Message}");
                throw new Exception(
                    "Cannot read the encryption key on this machine.\n\n" +
                    "Export must be performed on the original PC where the key was created, " +
                    "logged in as the same Windows user account.", ex);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BACKUP EXPORT] ❌ {ex.Message}");
                throw;
            }
        }

        // =====================================================================
        // IMPORT
        // =====================================================================

        /// <summary>
        /// Imports a .dkb backup file and restores the key on the current machine.
        /// Delegates key unwrapping + DB saving to CryptoHelper.ImportKeyWithPassphrase().
        /// After this completes, all existing patient data can be decrypted normally.
        /// Returns true on success, throws on failure.
        /// </summary>
        public static bool ImportKeyBackup(string backupPath, string adminPassword)
        {
            if (string.IsNullOrWhiteSpace(backupPath))
                throw new ArgumentException("Backup file path cannot be empty.");
            if (!File.Exists(backupPath))
                throw new FileNotFoundException("Backup file not found.", backupPath);
            if (string.IsNullOrWhiteSpace(adminPassword))
                throw new ArgumentException("Password cannot be empty.");

            try
            {
                byte[] exportBlob;

                using (var fs = new FileStream(backupPath, FileMode.Open, FileAccess.Read))
                using (var br = new BinaryReader(fs, Encoding.UTF8))
                {
                    // Step 1: Verify file header
                    byte[] expectedHeader = Encoding.UTF8.GetBytes(BACKUP_FILE_HEADER);
                    byte[] readHeader = br.ReadBytes(expectedHeader.Length);
                    string readHeaderStr = Encoding.UTF8.GetString(readHeader);

                    if (readHeaderStr != BACKUP_FILE_HEADER)
                    {
                        // Detect old V1 format and give a clear message
                        if (readHeaderStr.StartsWith("DENTAL_KEY_BACKUP_V1"))
                            throw new Exception(
                                "This backup was created with an older version of the application (V1).\n\n" +
                                "Please export a new backup from the original machine first.");

                        throw new Exception(
                            "Invalid backup file. This file was not created by this application.");
                    }

                    // Step 2: Read metadata — log for diagnostics, not used for decryption
                    long createdBinary = br.ReadInt64();
                    int machineNameLen = br.ReadInt32();
                    string originalMachine = Encoding.UTF8.GetString(br.ReadBytes(machineNameLen));

                    System.Diagnostics.Debug.WriteLine($"[BACKUP IMPORT] Original machine : {originalMachine}");
                    System.Diagnostics.Debug.WriteLine($"[BACKUP IMPORT] Created (UTC)     : {DateTime.FromBinary(createdBinary):yyyy-MM-dd HH:mm:ss}");

                    // Step 3: Read the passphrase-protected key blob
                    int blobLength = br.ReadInt32();
                    exportBlob = br.ReadBytes(blobLength);

                    if (exportBlob.Length != blobLength)
                        throw new Exception("Backup file appears truncated or corrupted.");
                }

                // Step 4: Delegate decryption + DPAPI re-protection + DB save to CryptoHelper
                CryptoHelper.ImportKeyWithPassphrase(exportBlob, adminPassword);

                System.Diagnostics.Debug.WriteLine("[BACKUP IMPORT] ✅ Key restored successfully.");
                return true;
            }
            catch (CryptographicException)
            {
                // Wrong passphrase produces a crypto error during AES decryption
                throw new Exception(
                    "Wrong password, or the backup file is corrupted.\n\n" +
                    "Please check the backup password and try again.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BACKUP IMPORT] ❌ {ex.Message}");
                throw;
            }
        }
    }
}