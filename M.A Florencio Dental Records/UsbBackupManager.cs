using System;
using System.IO;
using System.Management;
using System.Windows.Forms;
using System.Linq;
using System.Data.SqlClient;
using System.Text;
using System.Data;

namespace M.A_Florencio_Dental_Records
{
    public static class UsbBackupManager
    {
        private const string RegisteredKeyName = "BackupUsbDriveSerial";

        public static (bool success, string message) RegisterCurrentUsbDrive()
        {
            try
            {
                var drives = DriveInfo.GetDrives()
                    .Where(d => d.IsReady && d.DriveType == DriveType.Removable)
                    .ToList();

                if (drives.Count == 0)
                    return (false, "❌ No USB flash drive detected.\nPlease insert the drive first.");

                if (drives.Count > 1)
                    return (false, "❌ Multiple USB drives detected.\nPlease remove all other USB devices first.");

                var usb = drives[0];
                string serial = GetVolumeSerialNumber(usb.Name);

                if (string.IsNullOrEmpty(serial))
                    return (false, "❌ Could not read USB serial number.");

                // This will now throw if the DB is unreachable
                EnsureEncryptionKeysTableExists();

                bool saved = SaveRegisteredSerial(serial, usb.VolumeLabel ?? "Unnamed USB");
                if (saved)
                    return (true, $"✅ USB Registered Successfully!\n\nDrive: {usb.Name} ({usb.VolumeLabel})\nSerial: {serial}");

                return (false, "❌ Failed to save registration. (SaveRegisteredSerial returned false)");
            }
            catch (SqlException ex)
            {
                string connStr = ConnectionHelper.GetConnectionString();
                return (false,
                    $"❌ SQL Error during USB registration.\n\n" +
                    $"SQL Error #{ex.Number}: {ex.Message}\n\n" +
                    $"Connection string used:\n{connStr}\n\n" +
                    $"Tip: Click 'Test Connection' first, then 'Fix Database', then retry.");
            }
            catch (Exception ex)
            {
                return (false, $"❌ Registration error ({ex.GetType().Name}):\n{ex.Message}");
            }
        }

        public static string GetRegisteredSerial()
        {
            try
            {
                EnsureEncryptionKeysTableExists();

                using (var conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(
                        "SELECT TOP 1 ProtectedKeyData FROM EncryptionKeys WHERE KeyName = @KeyName AND IsActive = 1",
                        conn))
                    {
                        cmd.Parameters.AddWithValue("@KeyName", RegisteredKeyName);
                        var result = cmd.ExecuteScalar() as byte[];
                        return result != null && result.Length > 0 ? Encoding.UTF8.GetString(result) : "";
                    }
                }
            }
            catch
            {
                // If we can't read the serial, treat as no registered drive
                return "";
            }
        }

        /// <summary>
        /// Ensures the EncryptionKeys table exists. THROWS on failure so callers know the real error.
        /// </summary>
        private static void EnsureEncryptionKeysTableExists()
        {
            string connStr = ConnectionHelper.GetConnectionString();
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open(); // Throws SqlException if server/DB unreachable

                string sql = @"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='EncryptionKeys' AND xtype='U')
                    CREATE TABLE EncryptionKeys (
                        KeyID INT IDENTITY(1,1) PRIMARY KEY,
                        KeyName NVARCHAR(100) NOT NULL UNIQUE,
                        ProtectedKeyData VARBINARY(MAX) NOT NULL,
                        DataProtectionScope NVARCHAR(50) NOT NULL,
                        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
                        CreatedByUser NVARCHAR(100) NOT NULL,
                        IsActive BIT NOT NULL DEFAULT 1
                    );";
                using (var cmd = new SqlCommand(sql, conn))
                    cmd.ExecuteNonQuery();
            }
        }

        private static bool SaveRegisteredSerial(string serial, string label)
        {
            using (var conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
            {
                conn.Open();
                byte[] serialBytes = Encoding.UTF8.GetBytes(serial);

                // Check if exists first, then UPDATE or INSERT separately.
                // Avoids GETDATE() inside VALUES() which causes Error #241
                // on some SQL Server locale/collation configurations.
                string checkSql = "SELECT COUNT(*) FROM EncryptionKeys WHERE KeyName = @KeyName";
                int exists;
                using (var cmd = new SqlCommand(checkSql, conn))
                {
                    cmd.Parameters.AddWithValue("@KeyName", RegisteredKeyName);
                    exists = (int)cmd.ExecuteScalar();
                }

                if (exists > 0)
                {
                    string updateSql = @"
                UPDATE EncryptionKeys
                SET ProtectedKeyData    = @Data,
                    CreatedByUser       = @User,
                    CreatedDate         = GETDATE(),
                    IsActive            = 1
                WHERE KeyName = @KeyName";
                    using (var cmd = new SqlCommand(updateSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@KeyName", RegisteredKeyName);
                        cmd.Parameters.Add("@Data", SqlDbType.VarBinary).Value = serialBytes;
                        cmd.Parameters.AddWithValue("@User", Environment.UserName);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    string insertSql = @"
                INSERT INTO EncryptionKeys
                    (KeyName, ProtectedKeyData, DataProtectionScope, CreatedByUser, CreatedDate, IsActive)
                VALUES
                    (@KeyName, @Data, 'LocalMachine', @User, GETDATE(), 1)";
                    using (var cmd = new SqlCommand(insertSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@KeyName", RegisteredKeyName);
                        cmd.Parameters.Add("@Data", SqlDbType.VarBinary).Value = serialBytes;
                        cmd.Parameters.AddWithValue("@User", Environment.UserName);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return true;
        }

        public static bool IsRegisteredDrivePresent(out string driveLetter)
        {
            driveLetter = null;
            try
            {
                string registered = GetRegisteredSerial();
                if (string.IsNullOrEmpty(registered)) return false;

                foreach (var drive in DriveInfo.GetDrives())
                {
                    if (drive.IsReady && drive.DriveType == DriveType.Removable)
                    {
                        if (GetVolumeSerialNumber(drive.Name) == registered)
                        {
                            driveLetter = drive.Name;
                            return true;
                        }
                    }
                }
            }
            catch { }
            return false;
        }

        private static string GetVolumeSerialNumber(string driveLetter)
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher(
                    $"SELECT VolumeSerialNumber FROM Win32_LogicalDisk WHERE DeviceID = '{driveLetter.TrimEnd('\\')}'"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                        return obj["VolumeSerialNumber"]?.ToString() ?? "";
                }
            }
            catch { }
            return "";
        }

        public static void PerformBackupToUsb()
        {
            if (!IsRegisteredDrivePresent(out string usbRoot)) return;

            string backupFolder = Path.Combine(usbRoot, "DentalClinicBackups");
            if (!Directory.Exists(backupFolder))
                Directory.CreateDirectory(backupFolder);

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string backupPath = Path.Combine(backupFolder, $"DentalClinicDB_{timestamp}.bak");

            string server = ConnectionHelper.GetCurrentServerName();
            ServerDiscovery.BackupDatabase(server, "DentalClinicDB", backupPath);
        }
    }
}