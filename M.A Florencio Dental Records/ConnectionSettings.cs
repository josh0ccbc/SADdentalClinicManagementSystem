using System;
using System.IO;
using System.Text.Json;

public class ConnectionSettings
{
    public string ServerName { get; set; }
    public string DatabaseName { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public static ConnectionSettings Current { get; set; }
    public bool UseIntegratedSecurity { get; set; }

    private static string settingsPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "DentalClinic", "connection.json");

    // SAVE settings to file
    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(settingsPath));
        string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(settingsPath, json);
    }

    // LOAD settings from file
    public static ConnectionSettings Load()
    {
        if (File.Exists(settingsPath))
        {
            try
            {
                string json = File.ReadAllText(settingsPath);
                return JsonSerializer.Deserialize<ConnectionSettings>(json);
            }
            catch { }
        }
        return new ConnectionSettings { UseIntegratedSecurity = true };
    }

    // BUILD connection string
    public string GetConnectionString()
    {
        if (UseIntegratedSecurity)
            return $"Data Source={ServerName};Initial Catalog={DatabaseName};Integrated Security=True";
        else
            return $"Data Source={ServerName};Initial Catalog={DatabaseName};User Id={Username};Password={Password}";
    }

    // CHECK if settings exist
    public static bool Exists()
    {
        return File.Exists(settingsPath);
    }

    // DELETE saved settings
    public static void Delete()
    {
        if (File.Exists(settingsPath))
            File.Delete(settingsPath);
    }
}