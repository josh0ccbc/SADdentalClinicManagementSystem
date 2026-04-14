using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

public class ConnectionSettings
{
    public string ServerName { get; set; }
    public string DatabaseName { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public bool UseIntegratedSecurity { get; set; }
    public static ConnectionSettings Current { get; set; }

    private static string settingsPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "DentalClinic", "connection.dat"); // ✅ .dat — signals binary, not plain text

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(settingsPath));
            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });

            // ✅ Encrypt with DPAPI before writing to disk
            byte[] plainBytes = Encoding.UTF8.GetBytes(json);
            byte[] encryptedBytes = ProtectedData.Protect(plainBytes, null, DataProtectionScope.CurrentUser);

            File.WriteAllBytes(settingsPath, encryptedBytes);
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show(
                "Failed to save connection settings securely:\n\n" + ex.Message,
                "Save Error", System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Error);
        }
    }

    public static ConnectionSettings Load()
    {
        if (!File.Exists(settingsPath))
            return new ConnectionSettings { UseIntegratedSecurity = true };

        try
        {
            byte[] encryptedBytes = File.ReadAllBytes(settingsPath);

            // ✅ Decrypt with DPAPI on load
            byte[] plainBytes = ProtectedData.Unprotect(encryptedBytes, null, DataProtectionScope.CurrentUser);
            string json = Encoding.UTF8.GetString(plainBytes);

            return JsonSerializer.Deserialize<ConnectionSettings>(json);
        }
        catch (CryptographicException)
        {
            // Wrong machine or user — settings unreadable
            System.Windows.Forms.MessageBox.Show(
                "Connection settings could not be decrypted.\n\n" +
                "This may happen if the app was moved to a new PC or Windows user.\n" +
                "Setup will run again.",
                "Decryption Error", System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Warning);
            return null;
        }
        catch
        {
            return new ConnectionSettings { UseIntegratedSecurity = true };
        }
    }

    public static bool Exists() => File.Exists(settingsPath);

    public static void Delete()
    {
        if (File.Exists(settingsPath))
            File.Delete(settingsPath);
    }

    public string GetConnectionString()
    {
        if (UseIntegratedSecurity)
            return $"Data Source={ServerName};Initial Catalog={DatabaseName};Integrated Security=True";
        else
            return $"Data Source={ServerName};Initial Catalog={DatabaseName};User Id={Username};Password={Password}";
    }
}