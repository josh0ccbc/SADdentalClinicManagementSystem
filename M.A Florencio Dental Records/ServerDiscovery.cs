using System;
using System.Collections.Generic;
using System.Data.Sql;
using System.Linq;
using System.Data.SqlClient;  

public class ServerDiscovery
{
    public static List<string> FindAvailableServers()
    {
        List<string> servers = new List<string>();

        try
        {
            // Scan network for SQL Server instances
            SqlDataSourceEnumerator instance = SqlDataSourceEnumerator.Instance;
            System.Data.DataTable table = instance.GetDataSources();

            foreach (System.Data.DataRow row in table.Rows)
            {
                string serverName = row["ServerName"].ToString();
                string instanceName = row["InstanceName"].ToString();

                // Build full server name
                string fullName = string.IsNullOrEmpty(instanceName) ?
                    serverName :
                    $"{serverName}\\{instanceName}";

                if (!servers.Contains(fullName))
                    servers.Add(fullName);
            }

            // Add local server
            if (!servers.Contains("(local)"))
                servers.Insert(0, "(local)");

            if (!servers.Contains("."))
                servers.Insert(0, ".");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Server discovery error: {ex.Message}");
        }

        return servers;
    }

    public static List<string> FindAvailableDatabases(string serverName)
    {
        List<string> databases = new List<string>();

        try
        {
            string connStr = $"Data Source={serverName};Integrated Security=True;Connection Timeout=5";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // Query system databases
                string query = "SELECT name FROM sys.databases WHERE database_id > 4 ORDER BY name";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    databases.Add(reader["name"].ToString());
                }

                reader.Close();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Database discovery error: {ex.Message}");
        }

        return databases;
    }

    public static bool TestConnection(string connectionString)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                return true;
            }
        }
        catch
        {
            return false;
        }
    }
}