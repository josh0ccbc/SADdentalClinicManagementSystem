using System;
using System.IO;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace M.A_Florencio_Dental_Records
{
    public static class ConnectionHelper
    {
        // Must be exactly 16 characters each
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("FL0R3NC10D3NT4LK");
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("D3NT4L1V16BYTESS");

        // ✅ NEW SAFE PATH (AppData)
        private static string GetSettingsPath()
        {
            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "FlorencioDental"
            );

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            return Path.Combine(folder, "settings.ini");
        }

        // ===== CHECK IF SETTINGS EXIST =====
        public static bool SettingsExist()
        {
            return File.Exists(GetSettingsPath());
        }

        // ===== DELETE SETTINGS =====
        public static void DeleteSettings()
        {
            string path = GetSettingsPath();

            if (File.Exists(path))
                File.Delete(path);
        }

        // ===== SAVE ENCRYPTED SETTINGS =====
        public static void SaveConnectionString(string server, string database)
        {
            string path = GetSettingsPath();

            string encryptedServer = Encrypt(server);
            string encryptedDatabase = Encrypt(database);

            var lines = new[]
            {
                $"server={encryptedServer}",
                $"database={encryptedDatabase}"
            };

            File.WriteAllLines(path, lines);
        }

        // ===== GET DECRYPTED CONNECTION STRING =====
        public static string GetConnectionString()
        {
            try
            {
                string path = GetSettingsPath();

                if (!File.Exists(path))
                    return null;

                string server = null;
                string database = null;

                foreach (string line in File.ReadAllLines(path))
                {
                    if (line.StartsWith("server="))
                        server = Decrypt(line.Substring("server=".Length));
                    else if (line.StartsWith("database="))
                        database = Decrypt(line.Substring("database=".Length));
                }

                if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(database))
                    return null;

                return $"Server={server};Database={database};Integrated Security=True;";
            }
            catch
            {
                return null;
            }
        }

        // ===== TEST CONNECTION =====
        public static (bool success, string error) TestConnectionDetailed(
            string serverName, string databaseName = "master")
        {
            try
            {
                string connStr = $"Server={serverName};Database={databaseName};" +
                                 $"Integrated Security=True;Connection Timeout=10;";

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    return (true, null);
                }
            }
            catch (SqlException ex)
            {
                string friendlyMessage;
                switch (ex.Number)
                {
                    case 26:
                        friendlyMessage = "Error 26: Server not found.\n→ Try .\\SQLEXPRESS";
                        break;
                    case 18456:
                        friendlyMessage = "Error 18456: Login failed.";
                        break;
                    case 53:
                        friendlyMessage = "Error 53: Network path not found.";
                        break;
                    case 40:
                        friendlyMessage = "Error 40: Cannot open connection.";
                        break;
                    default:
                        friendlyMessage = $"SQL Error {ex.Number}: {ex.Message}";
                        break;
                }
                return (false, friendlyMessage);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        // ===== AES ENCRYPT =====
        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                    sw.Close();
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        // ===== AES DECRYPT =====
        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            try
            {
                byte[] buffer = Convert.FromBase64String(cipherText);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Key;
                    aes.IV = IV;

                    using (var ms = new MemoryStream(buffer))
                    using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    using (var sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
            catch
            {
                return cipherText;
            }
        }
    }
}