using System;
using System.Data.SqlClient;

namespace M.A_Florencio_Dental_Records
{
    public static class ConnectionHelper
    {
        // ================== TRY THESE ONE BY ONE ==================
        // For your dev PC, try each line below (rebuild after each change)
        private static string ServerName = ".";           // Option 1 (most common for default instance)
        // private static string ServerName = "(local)";  // Option 2
        // private static string ServerName = "localhost"; // Option 3

        public static string GetConnectionString(string database = "DentalClinicDB")
        {
            return $"Server={ServerName};Database={database};Integrated Security=True;Connection Timeout=10;";
        }

        public static void SetServerName(string newServerName)
        {
            ServerName = newServerName;
        }

        public static string GetCurrentServerName() => ServerName;

        // Test connection
        public static (bool success, string error) TestConnectionDetailed(string serverName, string databaseName = "master")
        {
            try
            {
                string connStr = $"Server={serverName};Database={databaseName};Integrated Security=True;Connection Timeout=10;";
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    return (true, null);
                }
            }
            catch (SqlException ex)
            {
                string msg;
                switch (ex.Number)
                {
                    case 26: msg = $"Error 26: Server '{serverName}' not found."; break;
                    case 18456: msg = "Error 18456: Login failed."; break;
                    case 53: msg = "Error 53: Network path not found."; break;
                    case 40: msg = "Error 40: Could not open connection."; break;
                    default: msg = $"SQL Error {ex.Number}: {ex.Message}"; break;
                }
                return (false, msg);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        // Dummy methods to stop compilation errors in Program.cs and ConnectionSetupForm.cs
        public static bool SettingsExist() => true;
        public static void DeleteSettings() { }
        public static void SaveConnectionString(string server, string database = "DentalClinicDB")
        {
            ServerName = server;
        }
    }
}