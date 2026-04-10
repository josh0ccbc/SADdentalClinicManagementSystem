using System;
using System.Windows.Forms;
using M.A_Florencio_Dental_Records;

namespace M.A_Florencio_Dental_Records
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // ✅ STEP 1: Check if connection settings exist
            // If not, show setup wizard (only once on first run)
            if (!ConnectionSettings.Exists())
            {
                ConnectionSetupForm setupForm = new ConnectionSetupForm();
                if (setupForm.ShowDialog() != DialogResult.OK)
                {
                    // User cancelled setup
                    MessageBox.Show(
                        "Database connection setup is required to run this application.",
                        "Setup Required",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    Application.Exit();
                    return;
                }
            }

            // ✅ STEP 2: Load connection settings globally
            try
            {
                ConnectionSettings.Current = ConnectionSettings.Load();

                // Test connection is valid
                if (!ServerDiscovery.TestConnection(ConnectionSettings.Current.GetConnectionString()))
                {
                    MessageBox.Show(
                        "Cannot connect to database. Please check your connection settings.",
                        "Connection Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    Application.Exit();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading connection settings: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            // ✅ STEP 3: LOGIN LOOP - Keep showing until successful login
            while (true)
            {
                LoginForm loginForm = new LoginForm();
                DialogResult result = loginForm.ShowDialog();

                if (result == DialogResult.OK)
                {
                    // User logged in successfully
                    break;  // Exit loop and show main form
                }
                else if (result == DialogResult.Cancel)
                {
                    // User clicked X button or closed form
                    Application.Exit();
                    return;
                }
                // If anything else, loop continues (show login again)
            }

            // ✅ STEP 4: After successful login, show main form
            Application.Run(new Form1());
        }
    }
}