// Program.cs
using System;
using System.Windows.Forms;

namespace M.A_Florencio_Dental_Records
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // ── STEP 1: First-run setup wizard ────────────────────────────
            if (!ConnectionHelper.SettingsExist())
            {
                using (ConnectionSetupForm setupForm = new ConnectionSetupForm())
                {
                    if (setupForm.ShowDialog() != DialogResult.OK)
                    {
                        MessageBox.Show(
                            "Database connection setup is required to run this application.",
                            "Setup Required",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }
                }
            }

            // ── STEP 2: Verify saved connection still works ───────────────
            try
            {
                string connectionString = ConnectionHelper.GetConnectionString();

                if (string.IsNullOrEmpty(connectionString))
                {
                    MessageBox.Show(
                        "Failed to load connection settings.\nSetup will run again.",
                        "Connection Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    ConnectionHelper.DeleteSettings();
                    return;
                }

                string server = GetServerFromConnectionString(connectionString);
                var (success, error) = ServerDiscovery.TestConnectionDetailed(server, "master");

                if (!success)
                {
                    var retry = MessageBox.Show(
                        $"Cannot connect to database:\n\n{error}\n\nRun setup again?",
                        "Connection Error",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Error);

                    ConnectionHelper.DeleteSettings();

                    if (retry == DialogResult.Yes)
                    {
                        using (ConnectionSetupForm setupForm = new ConnectionSetupForm())
                        {
                            if (setupForm.ShowDialog() != DialogResult.OK)
                                return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading connection settings:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // ── STEP 3: Ensure all DB tables + admin account exist ────────
            try
            {
                DatabaseInitializer.Initialize();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Database initialization failed:\n\n{ex.Message}\n\n" +
                    "Please check that SQL Server is running and the database is accessible.",
                    "Initialization Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // ── STEP 4 & 5: Login → Main Form loop ───────────────────────
            while (true)
            {
                // Show login form
                using (LoginForm loginForm = new LoginForm())
                {
                    DialogResult result = loginForm.ShowDialog();

                    if (result == DialogResult.Cancel)
                        return; // User closed login window — exit app

                    if (result != DialogResult.OK)
                        return; // Any other non-OK result — exit app
                }

                // Show main form
                using (Form1 mainForm = new Form1())
                {
                    Application.Run(mainForm);
                }
                // When Form1 closes (logout), loop repeats → login shown again
            }
        }

        private static string GetServerFromConnectionString(string connectionString)
        {
            foreach (string part in connectionString.Split(';'))
            {
                string trimmed = part.Trim();
                if (trimmed.StartsWith("Server=", StringComparison.OrdinalIgnoreCase))
                    return trimmed.Substring("Server=".Length).Trim();
            }
            return ".\\SQLEXPRESS";
        }
    }
}