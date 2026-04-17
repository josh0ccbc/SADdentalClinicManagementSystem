using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace M.A_Florencio_Dental_Records
{
    public static class KeyBackupManager
    {
        private const string BACKUP_FILE_HEADER = "DENTAL_KEY_BACKUP_V2";

        // =====================================================================
        // EXPORT
        // =====================================================================
        public static bool ExportKeyBackup(string outputPath, string adminPassword)
        {
            if (string.IsNullOrWhiteSpace(outputPath)) throw new ArgumentException("Output path cannot be empty.");
            if (string.IsNullOrWhiteSpace(adminPassword)) throw new ArgumentException("Password cannot be empty.");

            try
            {
                string dir = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                byte[] exportBlob = CryptoHelper.ExportKeyWithPassphrase(adminPassword);

                using (var fs = new FileStream(outputPath, FileMode.Create))
                using (var bw = new BinaryWriter(fs, Encoding.UTF8))
                {
                    bw.Write(Encoding.UTF8.GetBytes(BACKUP_FILE_HEADER));
                    bw.Write(DateTime.UtcNow.ToBinary());

                    byte[] machineBytes = Encoding.UTF8.GetBytes(Environment.MachineName);
                    bw.Write(machineBytes.Length);
                    bw.Write(machineBytes);

                    bw.Write(exportBlob.Length);
                    bw.Write(exportBlob);
                }

                MessageBox.Show($"✅ Backup created successfully!\n\nFile: {outputPath}",
                    "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export failed:\n\n{ex.Message}", "Export Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // =====================================================================
        // IMPORT - FIXED VERSION
        // =====================================================================
        public static bool ImportKeyBackup(string backupPath, string adminPassword)
        {
            if (string.IsNullOrWhiteSpace(backupPath) || !File.Exists(backupPath))
                throw new Exception("Backup file not found or path is empty.");

            if (string.IsNullOrWhiteSpace(adminPassword))
                throw new Exception("Password cannot be empty.");

            try
            {
                byte[] exportBlob;

                using (var fs = new FileStream(backupPath, FileMode.Open, FileAccess.Read))
                using (var br = new BinaryReader(fs, Encoding.UTF8))
                {
                    // Read header
                    byte[] headerBytes = br.ReadBytes(BACKUP_FILE_HEADER.Length);
                    string header = Encoding.UTF8.GetString(headerBytes);

                    if (header != BACKUP_FILE_HEADER)
                    {
                        if (header.StartsWith("DENTAL_KEY_BACKUP_V1"))
                            throw new Exception("This backup file is from an older version.\nPlease create a new backup from the original PC.");
                        throw new Exception("Invalid backup file format.");
                    }

                    // Skip metadata
                    br.ReadInt64();                    // timestamp
                    int machineLen = br.ReadInt32();
                    br.ReadBytes(machineLen);          // machine name

                    // Read blob
                    int blobLength = br.ReadInt32();
                    if (blobLength <= 0 || blobLength > fs.Length - fs.Position)
                        throw new Exception("Backup file is corrupted (invalid blob size).");

                    exportBlob = br.ReadBytes(blobLength);
                }

                // Pass to CryptoHelper
                CryptoHelper.ImportKeyWithPassphrase(exportBlob, adminPassword);

                MessageBox.Show("✅ Key imported successfully!\n\nYou can now use the system on this PC.\n\nPlease restart the application.",
                    "Import Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (CryptographicException)
            {
                MessageBox.Show("❌ Wrong password or the backup file is corrupted.\n\nPlease check the password and try again.",
                    "Import Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Import failed:\n\n{ex.Message}", "Import Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}