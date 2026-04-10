using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace M.A_Florencio_Dental_Records
{
    public partial class ConnectionSetupForm : Form
    {
        private ConnectionSettings settings = new ConnectionSettings();

        // ✅ ADD THESE AS CLASS-LEVEL VARIABLES (so all methods can access them)
        private ComboBox cmbServer;
        private ComboBox cmbDatabase;
        private Label lblConnStr;
        private Label lblStatus;

        public ConnectionSetupForm()
        {
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "Database Connection Setup";
            this.Width = 600;
            this.Height = 500;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Title Label
            Label lblTitle = new Label();
            lblTitle.Text = "🔍 Finding Database Servers...";
            lblTitle.Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold);
            lblTitle.Location = new System.Drawing.Point(20, 20);
            lblTitle.Width = 550;
            this.Controls.Add(lblTitle);

            // Server Selection
            Label lblServerLabel = new Label();
            lblServerLabel.Text = "Select Server:";
            lblServerLabel.Location = new System.Drawing.Point(20, 60);
            lblServerLabel.Width = 200;
            this.Controls.Add(lblServerLabel);

            // ✅ USE CLASS-LEVEL VARIABLE
            cmbServer = new ComboBox();
            cmbServer.Location = new System.Drawing.Point(20, 85);
            cmbServer.Width = 500;
            cmbServer.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbServer.SelectedIndexChanged += (s, e) => LoadDatabases(cmbServer);
            this.Controls.Add(cmbServer);

            // Database Selection
            Label lblDatabaseLabel = new Label();
            lblDatabaseLabel.Text = "Select Database:";
            lblDatabaseLabel.Location = new System.Drawing.Point(20, 120);
            lblDatabaseLabel.Width = 200;
            this.Controls.Add(lblDatabaseLabel);

            // ✅ USE CLASS-LEVEL VARIABLE
            cmbDatabase = new ComboBox();
            cmbDatabase.Location = new System.Drawing.Point(20, 145);
            cmbDatabase.Width = 500;
            cmbDatabase.DropDownStyle = ComboBoxStyle.DropDownList;
            this.Controls.Add(cmbDatabase);

            // Connection Info Box
            GroupBox grpConnection = new GroupBox();
            grpConnection.Text = "Connection Details";
            grpConnection.Location = new System.Drawing.Point(20, 180);
            grpConnection.Width = 500;
            grpConnection.Height = 150;
            this.Controls.Add(grpConnection);

            // ✅ USE CLASS-LEVEL VARIABLE
            lblConnStr = new Label();
            lblConnStr.Text = "Connection String: ";
            lblConnStr.Location = new System.Drawing.Point(10, 20);
            lblConnStr.Width = 470;
            lblConnStr.Height = 100;
            lblConnStr.AutoSize = false;
            lblConnStr.Font = new System.Drawing.Font("Courier New", 9);
            lblConnStr.BackColor = System.Drawing.Color.WhiteSmoke;
            lblConnStr.Padding = new Padding(5);
            grpConnection.Controls.Add(lblConnStr);

            // Test Button
            Button btnTest = new Button();
            btnTest.Text = "🧪 Test Connection";
            btnTest.Location = new System.Drawing.Point(20, 350);
            btnTest.Width = 230;
            btnTest.Height = 40;
            btnTest.Click += (s, e) => TestConnection();
            this.Controls.Add(btnTest);

            // Save Button
            Button btnSave = new Button();
            btnSave.Text = "✅ Save & Continue";
            btnSave.Location = new System.Drawing.Point(290, 350);
            btnSave.Width = 230;
            btnSave.Height = 40;
            btnSave.Click += (s, e) => SaveSettings();
            this.Controls.Add(btnSave);

            // ✅ USE CLASS-LEVEL VARIABLE
            lblStatus = new Label();
            lblStatus.Text = "";
            lblStatus.Location = new System.Drawing.Point(20, 410);
            lblStatus.Width = 500;
            lblStatus.Height = 50;
            lblStatus.AutoSize = false;
            lblStatus.Font = new System.Drawing.Font("Arial", 10);
            this.Controls.Add(lblStatus);

            // Load servers on startup
            LoadServers();
        }

        private void LoadServers()
        {
            UpdateStatus("🔍 Scanning network for SQL Server instances...");

            List<string> servers = ServerDiscovery.FindAvailableServers();

            if (servers.Count == 0)
            {
                cmbServer.Items.Add("No servers found");
                UpdateStatus("❌ No SQL Server instances found. Please enter manually:");
                cmbServer.DropDownStyle = ComboBoxStyle.Simple;
                cmbServer.Items.Clear();
            }
            else
            {
                foreach (string server in servers)
                {
                    cmbServer.Items.Add(server);
                }
                cmbServer.SelectedIndex = 0;
                UpdateStatus($"✅ Found {servers.Count} server(s)");
            }
        }

        private void LoadDatabases(ComboBox cmbServer)
        {
            if (cmbServer.SelectedItem == null)
                return;

            string serverName = cmbServer.SelectedItem.ToString();
            UpdateStatus($"🔍 Finding databases on {serverName}...");

            cmbDatabase.Items.Clear();

            // Find databases
            List<string> databases = ServerDiscovery.FindAvailableDatabases(serverName);

            if (databases.Count == 0)
            {
                cmbDatabase.Items.Add("No databases found");
                UpdateStatus($"❌ No databases found on {serverName}");
                return;
            }

            // Look for DentalClinicDB first
            int selectedIndex = 0;
            foreach (string db in databases)
            {
                cmbDatabase.Items.Add(db);
                if (db == "DentalClinicDB")
                    selectedIndex = cmbDatabase.Items.Count - 1;
            }

            cmbDatabase.SelectedIndex = selectedIndex;
            UpdateStatus($"✅ Found {databases.Count} database(s)");

            // Update connection string display
            UpdateConnectionString();
        }

        private void UpdateConnectionString()
        {
            // ✅ CHECK IF CONTROLS ARE NULL BEFORE ACCESSING
            if (cmbServer == null || cmbDatabase == null || lblConnStr == null)
                return;

            if (cmbServer.SelectedItem == null || cmbDatabase.SelectedItem == null)
                return;

            settings.ServerName = cmbServer.SelectedItem.ToString();
            settings.DatabaseName = cmbDatabase.SelectedItem.ToString();
            settings.UseIntegratedSecurity = true;

            lblConnStr.Text = "Connection String:\n\n" + settings.GetConnectionString();
        }

        private void TestConnection()
        {
            UpdateStatus("🧪 Testing connection...");

            if (!TestConnectionAttempt())
            {
                UpdateStatus("❌ Connection FAILED!\n\nPlease verify:\n- Server name is correct\n- Database exists\n- You have network access\n- SQL Server is running");
                return;
            }

            UpdateStatus("✅ Connection SUCCESSFUL! \n\nYou can now click 'Save & Continue'");
        }

        private bool TestConnectionAttempt()
        {
            try
            {
                return ServerDiscovery.TestConnection(settings.GetConnectionString());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Connection test error: {ex.Message}");
                return false;
            }
        }

        private void SaveSettings()
        {
            if (!TestConnectionAttempt())
            {
                MessageBox.Show(
                    "Connection test failed!\n\nPlease verify your settings before saving.",
                    "Connection Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            settings.Save();
            MessageBox.Show(
                "✅ Connection settings saved successfully!\n\nThe application will now start.",
                "Success",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void UpdateStatus(string message)
        {
            if (lblStatus != null)
            {
                lblStatus.Text = message;
                Application.DoEvents();
            }
        }
    }
}