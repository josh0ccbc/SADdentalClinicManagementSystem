using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace M.A_Florencio_Dental_Records
{
    public class ConnectionSetupForm : Form
    {
        private ComboBox cmbServer;
        private TextBox txtDatabase;
        private Label lblStatus;
        private Button btnTest;
        private Button btnSave;
        private Button btnCancel;
        private ProgressBar progressBar;
        private Label lblProgress;
        private Label lblServerStatus;
        private Label lblAutoStatus;

        public ConnectionSetupForm()
        {
            SetupUI();
            AutoConnect(); // Try to connect automatically on open
        }

        private void SetupUI()
        {
            this.Text = "M.A Florencio Dental Records - Database Setup";
            this.Size = new Size(580, 680);
            this.MinimumSize = new Size(580, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.WhiteSmoke;

            // ===== HEADER =====
            Panel panelHeader = new Panel();
            panelHeader.BackColor = Color.FromArgb(0, 102, 102);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 90;

            Label lblTitle = new Label();
            lblTitle.Text = "Database Connection Setup";
            lblTitle.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 15);
            lblTitle.Size = new Size(520, 38);

            Label lblSubtitle = new Label();
            lblSubtitle.Text = "Setting up your database automatically...";
            lblSubtitle.Font = new Font("Segoe UI", 10);
            lblSubtitle.ForeColor = Color.LightGray;
            lblSubtitle.Location = new Point(20, 55);
            lblSubtitle.Size = new Size(520, 22);

            panelHeader.Controls.Add(lblTitle);
            panelHeader.Controls.Add(lblSubtitle);

            // ===== BUTTON PANEL =====
            Panel panelButtons = new Panel();
            panelButtons.BackColor = Color.WhiteSmoke;
            panelButtons.Dock = DockStyle.Bottom;
            panelButtons.Height = 75;

            btnTest = new Button();
            btnTest.Text = "Test Connection";
            btnTest.Location = new Point(20, 17);
            btnTest.Size = new Size(155, 40);
            btnTest.BackColor = Color.FromArgb(90, 90, 90);
            btnTest.ForeColor = Color.White;
            btnTest.FlatStyle = FlatStyle.Flat;
            btnTest.FlatAppearance.BorderSize = 0;
            btnTest.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnTest.Cursor = Cursors.Hand;

            btnSave = new Button();
            btnSave.Text = "Save && Connect";
            btnSave.Location = new Point(195, 17);
            btnSave.Size = new Size(155, 40);
            btnSave.BackColor = Color.FromArgb(0, 102, 102);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnSave.Cursor = Cursors.Hand;
            btnSave.Enabled = false;

            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(375, 17);
            btnCancel.Size = new Size(155, 40);
            btnCancel.BackColor = Color.FromArgb(200, 200, 200);
            btnCancel.ForeColor = Color.Black;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnCancel.Cursor = Cursors.Hand;

            panelButtons.Controls.Add(btnTest);
            panelButtons.Controls.Add(btnSave);
            panelButtons.Controls.Add(btnCancel);

            // ===== CONTENT PANEL =====
            Panel panelContent = new Panel();
            panelContent.BackColor = Color.White;
            panelContent.Dock = DockStyle.Fill;

            // ===== AUTO STATUS BOX =====
            Panel panelAuto = new Panel();
            panelAuto.BackColor = Color.FromArgb(240, 248, 240);
            panelAuto.Location = new Point(20, 20);
            panelAuto.Size = new Size(530, 90);
            panelAuto.BorderStyle = BorderStyle.FixedSingle;

            Label lblAutoTitle = new Label();
            lblAutoTitle.Text = "⚡ Auto-Setup";
            lblAutoTitle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblAutoTitle.ForeColor = Color.FromArgb(0, 102, 0);
            lblAutoTitle.Location = new Point(10, 10);
            lblAutoTitle.Size = new Size(500, 20);

            lblAutoStatus = new Label();
            lblAutoStatus.Text = "Searching for SQL Server on your machine...";
            lblAutoStatus.Font = new Font("Segoe UI", 9);
            lblAutoStatus.ForeColor = Color.FromArgb(60, 60, 60);
            lblAutoStatus.Location = new Point(10, 35);
            lblAutoStatus.Size = new Size(510, 45);
            lblAutoStatus.AutoSize = false;

            panelAuto.Controls.Add(lblAutoTitle);
            panelAuto.Controls.Add(lblAutoStatus);

            // ===== DIVIDER =====
            Label lblDivider = new Label();
            lblDivider.Text = "─── Or configure manually below ───";
            lblDivider.Font = new Font("Segoe UI", 9, FontStyle.Italic);
            lblDivider.ForeColor = Color.Gray;
            lblDivider.Location = new Point(20, 125);
            lblDivider.Size = new Size(530, 20);
            lblDivider.TextAlign = ContentAlignment.MiddleCenter;

            // ===== SERVER =====
            Label lblServer = new Label();
            lblServer.Text = "SQL Server Name:";
            lblServer.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblServer.ForeColor = Color.FromArgb(40, 40, 40);
            lblServer.Location = new Point(20, 155);
            lblServer.Size = new Size(530, 22);

            cmbServer = new ComboBox();
            cmbServer.Location = new Point(20, 180);
            cmbServer.Size = new Size(525, 30);
            cmbServer.Font = new Font("Segoe UI", 10);
            cmbServer.DropDownStyle = ComboBoxStyle.DropDown;
            cmbServer.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmbServer.AutoCompleteSource = AutoCompleteSource.ListItems;

            lblServerStatus = new Label();
            lblServerStatus.Text = "Searching for instances...";
            lblServerStatus.Font = new Font("Segoe UI", 8, FontStyle.Italic);
            lblServerStatus.ForeColor = Color.DarkOrange;
            lblServerStatus.Location = new Point(20, 213);
            lblServerStatus.Size = new Size(525, 18);

            // ===== DATABASE =====
            Label lblDatabase = new Label();
            lblDatabase.Text = "Database Name:";
            lblDatabase.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblDatabase.ForeColor = Color.FromArgb(40, 40, 40);
            lblDatabase.Location = new Point(20, 245);
            lblDatabase.Size = new Size(530, 22);

            txtDatabase = new TextBox();
            txtDatabase.Text = "DentalClinicDB";
            txtDatabase.Location = new Point(20, 270);
            txtDatabase.Size = new Size(525, 30);
            txtDatabase.Font = new Font("Segoe UI", 10);

            Label lblDatabaseHint = new Label();
            lblDatabaseHint.Text = "Will be auto-created if it doesn't exist.";
            lblDatabaseHint.Font = new Font("Segoe UI", 8, FontStyle.Italic);
            lblDatabaseHint.ForeColor = Color.Gray;
            lblDatabaseHint.Location = new Point(20, 303);
            lblDatabaseHint.Size = new Size(525, 18);

            // ===== STATUS =====
            Panel line = new Panel();
            line.BackColor = Color.FromArgb(220, 220, 220);
            line.Location = new Point(20, 330);
            line.Size = new Size(525, 1);

            lblStatus = new Label();
            lblStatus.Text = "";
            lblStatus.Location = new Point(20, 340);
            lblStatus.Size = new Size(525, 160);
            lblStatus.Font = new Font("Segoe UI", 9);
            lblStatus.ForeColor = Color.Gray;
            lblStatus.AutoSize = false;

            // ===== PROGRESS =====
            progressBar = new ProgressBar();
            progressBar.Location = new Point(0, 505);
            progressBar.Size = new Size(580, 8);
            progressBar.Visible = false;
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.MarqueeAnimationSpeed = 30;

            lblProgress = new Label();
            lblProgress.Text = "";
            lblProgress.Location = new Point(20, 518);
            lblProgress.Size = new Size(525, 20);
            lblProgress.Font = new Font("Segoe UI", 9, FontStyle.Italic);
            lblProgress.ForeColor = Color.FromArgb(0, 102, 102);
            lblProgress.Visible = false;

            panelContent.Controls.Add(panelAuto);
            panelContent.Controls.Add(lblDivider);
            panelContent.Controls.Add(lblServer);
            panelContent.Controls.Add(cmbServer);
            panelContent.Controls.Add(lblServerStatus);
            panelContent.Controls.Add(lblDatabase);
            panelContent.Controls.Add(txtDatabase);
            panelContent.Controls.Add(lblDatabaseHint);
            panelContent.Controls.Add(line);
            panelContent.Controls.Add(lblStatus);
            panelContent.Controls.Add(progressBar);
            panelContent.Controls.Add(lblProgress);

            // ===== WIRE EVENTS =====
            btnTest.Click += BtnTest_Click;
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;

            // ===== ADD TO FORM (order matters for docking) =====
            this.Controls.Add(panelContent);
            this.Controls.Add(panelButtons);
            this.Controls.Add(panelHeader);
        }

        // ===== AUTO CONNECT ON STARTUP =====
        private void AutoConnect()
        {
            SetBusy(true, "Auto-detecting SQL Server...");
            lblAutoStatus.Text = "Scanning your machine for SQL Server instances...";

            BackgroundWorker autoWorker = new BackgroundWorker();

            autoWorker.DoWork += (s, e1) =>
            {
                // Step 1: find a working server
                string workingServer = ServerDiscovery.FindWorkingServer();
                e1.Result = workingServer;
            };

            autoWorker.RunWorkerCompleted += (s, e2) =>
            {
                string workingServer = e2.Result as string;

                if (workingServer != null)
                {
                    // Found one — populate the combobox
                    cmbServer.Items.Clear();
                    cmbServer.Items.Add(workingServer);
                    cmbServer.SelectedIndex = 0;

                    lblAutoStatus.Text = $"✅ Found SQL Server: {workingServer}\r\nCreating database automatically...";
                    lblServerStatus.Text = $"✅ Auto-detected: {workingServer}";
                    lblServerStatus.ForeColor = Color.Green;

                    // Step 2: auto-create database immediately
                    SetBusy(true, "Creating database and tables...");

                    BackgroundWorker createWorker = new BackgroundWorker();

                    createWorker.DoWork += (s2, e3) =>
                    {
                        e3.Result = ServerDiscovery.CreateDatabaseIfNotExists(
                            workingServer,
                            txtDatabase.Text.Trim());
                    };

                    createWorker.RunWorkerCompleted += (s2, e4) =>
                    {
                        SetBusy(false, "");
                        var (success, error) = ((bool, string))e4.Result;

                        if (success)
                        {
                            // Save and close automatically
                            ConnectionHelper.SaveConnectionString(
                                workingServer,
                                txtDatabase.Text.Trim());

                            lblAutoStatus.Text =
                                $"✅ All done! Database ready on: {workingServer}";

                            MessageBox.Show(
                                "✅ Database setup complete!\r\n\r\nThe application will now start.\r\nDefault login:  admin / admin123",
                                "Setup Complete",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else
                        {
                            // Server found but DB creation failed — let user see manual section
                            lblAutoStatus.Text =
                                $"⚠ Server found but database creation failed:\r\n{error}";
                            lblStatus.ForeColor = Color.OrangeRed;
                            lblStatus.Text = $"Auto-setup partially failed: {error}\r\n\r\nYou can still try manually below.";
                            btnSave.Enabled = true;
                        }
                    };

                    createWorker.RunWorkerAsync();
                }
                else
                {
                    // No server found at all — fall back to manual
                    SetBusy(false, "");
                    lblAutoStatus.Text =
                        "⚠ No SQL Server found automatically.\r\n" +
                        "Please install SQL Server Express or enter details manually below.";
                    lblAutoStatus.ForeColor = Color.OrangeRed;

                    // Still populate dropdown with options to try
                    DiscoverServers();

                    lblStatus.ForeColor = Color.OrangeRed;
                    lblStatus.Text =
                        "SQL Server was not found on this machine.\r\n\r\n" +
                        "Please install SQL Server Express (free):\r\n" +
                        "https://www.microsoft.com/en-us/sql-server/sql-server-downloads\r\n\r\n" +
                        "After installing, restart this application and setup will complete automatically.";
                }
            };

            autoWorker.RunWorkerAsync();
        }

        // ===== DISCOVER SERVERS (for manual dropdown) =====
        private void DiscoverServers()
        {
            BackgroundWorker discoverWorker = new BackgroundWorker();

            discoverWorker.DoWork += (s, e1) =>
            {
                e1.Result = ServerDiscovery.FindAvailableServers();
            };

            discoverWorker.RunWorkerCompleted += (s, e2) =>
            {
                if (e2.Result is List<string> servers)
                {
                    cmbServer.Items.Clear();
                    foreach (string srv in servers)
                        cmbServer.Items.Add(srv);

                    if (cmbServer.Items.Count > 0)
                    {
                        cmbServer.SelectedIndex = 0;
                        lblServerStatus.Text = $"✅ Found {cmbServer.Items.Count} option(s)";
                        lblServerStatus.ForeColor = Color.Green;
                    }
                    else
                    {
                        lblServerStatus.Text = "No instances found. Type manually.";
                        lblServerStatus.ForeColor = Color.Red;
                    }
                }
            };

            discoverWorker.RunWorkerAsync();
        }

        // ===== TEST BUTTON =====
        private void BtnTest_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cmbServer.Text))
            {
                lblStatus.ForeColor = Color.Red;
                lblStatus.Text = "❌ Please enter or select a SQL Server name.";
                return;
            }

            SetBusy(true, "Testing connection...");
            lblStatus.Text = "";

            BackgroundWorker testWorker = new BackgroundWorker();

            testWorker.DoWork += (s, e3) =>
            {
                e3.Result = ServerDiscovery.TestConnectionDetailed(
                    cmbServer.Text.Trim(), "master");
            };

            testWorker.RunWorkerCompleted += (s, e4) =>
            {
                SetBusy(false, "");
                var (success, error) = ((bool, string))e4.Result;

                if (success)
                {
                    lblStatus.ForeColor = Color.FromArgb(0, 130, 0);
                    lblStatus.Text = "✅ Connection successful! Click 'Save && Connect' to finish.";
                    btnSave.Enabled = true;
                }
                else
                {
                    lblStatus.ForeColor = Color.Red;
                    lblStatus.Text =
                        $"❌ Connection failed:\r\n{error}\r\n\r\n" +
                        "Try:\r\n" +
                        "  • .\\SQLEXPRESS\r\n" +
                        "  • localhost\\SQLEXPRESS\r\n" +
                        "  • Start SQL Server in services.msc";
                    btnSave.Enabled = false;
                }
            };

            testWorker.RunWorkerAsync();
        }

        // ===== SAVE BUTTON =====
        private void BtnSave_Click(object sender, EventArgs e)
        {
            SetBusy(true, "Creating database...");

            BackgroundWorker createWorker = new BackgroundWorker();

            createWorker.DoWork += (s, e5) =>
            {
                e5.Result = ServerDiscovery.CreateDatabaseIfNotExists(
                    cmbServer.Text.Trim(),
                    txtDatabase.Text.Trim());
            };

            createWorker.RunWorkerCompleted += (s, e6) =>
            {
                SetBusy(false, "");
                var (success, error) = ((bool, string))e6.Result;

                if (success)
                {
                    ConnectionHelper.SaveConnectionString(
                        cmbServer.Text.Trim(),
                        txtDatabase.Text.Trim());

                    MessageBox.Show(
                        "✅ Database setup complete!\r\n\r\nThe application will now start.\r\nDefault login:  admin / admin123",
                        "Setup Complete",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    lblStatus.ForeColor = Color.Red;
                    lblStatus.Text = $"❌ Failed:\r\n{error}";
                    btnTest.Enabled = true;
                    btnSave.Enabled = false;
                }
            };

            createWorker.RunWorkerAsync();
        }

        // ===== CANCEL =====
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // ===== HELPER =====
        private void SetBusy(bool busy, string message)
        {
            progressBar.Visible = busy;
            lblProgress.Visible = busy;
            lblProgress.Text = message;
            btnTest.Enabled = !busy;
            btnSave.Enabled = !busy;
        }
    }
}