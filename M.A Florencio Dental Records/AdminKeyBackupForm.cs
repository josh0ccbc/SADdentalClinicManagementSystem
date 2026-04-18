using System;
using System.Drawing;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace M.A_Florencio_Dental_Records
{
    public class AdminKeyBackupForm : Form
    {
        private TextBox txtExportPassword;
        private TextBox txtExportConfirm;
        private TextBox txtExportPath;
        private Button btnExportKey;
        private Label lblExportStatus;
        private TextBox txtImportPath;
        private TextBox txtImportPassword;
        private Button btnImportKey;
        private Label lblImportStatus;
        private Button btnDbBackup;
        private Button btnDbRestore;
        private Label lblDbStatus;
        private Label lblUsbStatus;
        private TextBox txtServerName;

        private const string BackupFolder = @"C:\DentalClinicBackups";

        public AdminKeyBackupForm()
        {
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "Security — Backup & Recovery";
            this.Width = 760;
            this.Height = 1100;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new Size(0, 1060);

            int y = 15;

            // ====================== ENCRYPTION KEY EXPORT ======================
            var grpKeyExport = new GroupBox { Text = "📤 Encryption Key Backup (Do this on working PC)", Location = new Point(20, y), Width = 710, Height = 265 };
            this.Controls.Add(grpKeyExport);
            var lblKeyWarning = new Label { Text = "⚠️ Store this backup file and password safely.\nWithout both, patient data cannot be recovered.", Location = new Point(20, 25), Width = 660, Height = 40, ForeColor = Color.DarkRed };
            grpKeyExport.Controls.Add(lblKeyWarning);
            AddLabel(grpKeyExport, "Backup Password:", 20, 72);
            txtExportPassword = AddTextBox(grpKeyExport, 20, 92, true, 480);
            AddLabel(grpKeyExport, "Confirm Password:", 20, 122);
            txtExportConfirm = AddTextBox(grpKeyExport, 20, 142, true, 480);
            AddLabel(grpKeyExport, "Save Key Backup To:", 20, 172);
            txtExportPath = AddTextBox(grpKeyExport, 20, 192, false, 530);
            var btnBrowseExport = new Button { Text = "Browse...", Location = new Point(565, 190), Width = 110 };
            btnBrowseExport.Click += BrowseExportPath;
            grpKeyExport.Controls.Add(btnBrowseExport);
            btnExportKey = new Button { Text = "📤 Export Encryption Key", Location = new Point(20, 225), Width = 250, Height = 40, BackColor = Color.SteelBlue, ForeColor = Color.White };
            btnExportKey.Click += ExportKey;
            grpKeyExport.Controls.Add(btnExportKey);
            lblExportStatus = new Label { Location = new Point(285, 230), Width = 400, Height = 30 };
            grpKeyExport.Controls.Add(lblExportStatus);
            y += 280;

            // ====================== ENCRYPTION KEY IMPORT ======================
            var grpKeyImport = new GroupBox { Text = "📥 Import Encryption Key (On new or replacement PC)", Location = new Point(20, y), Width = 710, Height = 190 };
            this.Controls.Add(grpKeyImport);
            AddLabel(grpKeyImport, "Key Backup File:", 20, 28);
            txtImportPath = AddTextBox(grpKeyImport, 20, 48, false, 530);
            var btnBrowseImport = new Button { Text = "Browse...", Location = new Point(565, 46), Width = 110 };
            btnBrowseImport.Click += BrowseImportPath;
            grpKeyImport.Controls.Add(btnBrowseImport);
            AddLabel(grpKeyImport, "Backup Password:", 20, 82);
            txtImportPassword = AddTextBox(grpKeyImport, 20, 102, true, 480);
            btnImportKey = new Button { Text = "📥 Import & Restore Key", Location = new Point(20, 140), Width = 250, Height = 42, BackColor = Color.DarkGreen, ForeColor = Color.White };
            btnImportKey.Click += ImportKey;
            grpKeyImport.Controls.Add(btnImportKey);
            lblImportStatus = new Label { Location = new Point(285, 145), Width = 400, Height = 30 };
            grpKeyImport.Controls.Add(lblImportStatus);
            y += 205;

            // ====================== FULL DATABASE BACKUP & RESTORE ======================
            var grpDb = new GroupBox { Text = "💾 Full Database Backup & Restore", Location = new Point(20, y), Width = 710, Height = 205 };
            this.Controls.Add(grpDb);
            var lblDbNote = new Label { Text = "This backs up EVERYTHING: patients, medical history, appointments, prescriptions, etc.", Location = new Point(20, 25), Width = 660, Height = 35, ForeColor = Color.DarkBlue };
            grpDb.Controls.Add(lblDbNote);
            var lblWarning = new Label { Text = "⚠️ Recommended: Save backups to C:\\DentalClinicBackups\n(SQL Server service needs write permission)", Location = new Point(20, 62), Width = 660, Height = 38, ForeColor = Color.DarkOrange, Font = new Font("Arial", 9, FontStyle.Bold) };
            grpDb.Controls.Add(lblWarning);
            btnDbBackup = new Button { Text = "📤 Backup Full Database (.bak)", Location = new Point(20, 105), Width = 320, Height = 48, BackColor = Color.Teal, ForeColor = Color.White };
            btnDbBackup.Click += BtnDbBackup_Click;
            grpDb.Controls.Add(btnDbBackup);
            btnDbRestore = new Button { Text = "📥 Restore Database from .bak", Location = new Point(360, 105), Width = 320, Height = 48, BackColor = Color.OrangeRed, ForeColor = Color.White };
            btnDbRestore.Click += BtnDbRestore_Click;
            grpDb.Controls.Add(btnDbRestore);
            lblDbStatus = new Label { Location = new Point(20, 160), Width = 660, Height = 35, Font = new Font("Arial", 8) };
            grpDb.Controls.Add(lblDbStatus);
            y += 210;

            // ====================== REGISTERED USB BACKUP DRIVE ======================
            var grpUsb = new GroupBox { Text = "📌 Registered Backup USB Flash Drive (Auto-Backup on Insert)", Location = new Point(20, y), Width = 710, Height = 230 };
            this.Controls.Add(grpUsb);
            var lblUsbInfo = new Label
            {
                Text = "Insert the official backup flash drive and click Register.\nThe system will ONLY backup to this exact USB (even if drive letter changes).",
                Location = new Point(20, 25),
                Width = 660,
                Height = 50,
                ForeColor = Color.DarkBlue
            };
            grpUsb.Controls.Add(lblUsbInfo);

            var btnRegisterUsb = new Button { Text = "📍 Register Current USB Drive", Location = new Point(20, 85), Width = 320, Height = 48, BackColor = Color.Purple, ForeColor = Color.White };
            btnRegisterUsb.Click += BtnRegisterUsb_Click;
            grpUsb.Controls.Add(btnRegisterUsb);

            var btnClearUsb = new Button { Text = "🗑️ Clear / Change Registered Drive", Location = new Point(360, 85), Width = 320, Height = 48, BackColor = Color.Gray, ForeColor = Color.White };
            btnClearUsb.Click += BtnClearUsb_Click;
            grpUsb.Controls.Add(btnClearUsb);

            var btnFixDb = new Button { Text = "🔧 Fix Database (Create Missing Tables)", Location = new Point(20, 145), Width = 320, Height = 40, BackColor = Color.Orange, ForeColor = Color.White };
            btnFixDb.Click += BtnFixDb_Click;
            grpUsb.Controls.Add(btnFixDb);

            // NEW: quick write test button
            var btnTestWrite = new Button { Text = "🧪 Test DB Write Access", Location = new Point(360, 145), Width = 320, Height = 40, BackColor = Color.DarkCyan, ForeColor = Color.White };
            btnTestWrite.Click += BtnTestDbWrite_Click;
            grpUsb.Controls.Add(btnTestWrite);

            lblUsbStatus = new Label { Location = new Point(20, 198), Width = 660, Height = 25, Font = new Font("Arial", 9, FontStyle.Bold) };
            grpUsb.Controls.Add(lblUsbStatus);
            y += 240;

            // ====================== MANUAL SQL SERVER CONNECTION ======================
            var grpConn = new GroupBox { Text = "🔧 Manual SQL Server Connection (Fix Connection Issues)", Location = new Point(20, y), Width = 710, Height = 170 };
            this.Controls.Add(grpConn);

            AddLabel(grpConn, "SQL Server Instance Name:", 20, 25);
            txtServerName = new TextBox { Location = new Point(20, 45), Width = 400, Text = ConnectionHelper.GetCurrentServerName() };
            grpConn.Controls.Add(txtServerName);

            var btnTestConn = new Button { Text = "Test Connection", Location = new Point(440, 42), Width = 130, Height = 35, BackColor = Color.DodgerBlue, ForeColor = Color.White };
            btnTestConn.Click += BtnTestConnection_Click;
            grpConn.Controls.Add(btnTestConn);

            var lblHint = new Label
            {
                Text = "Try these values:\n• .   or   (local)\n• localhost\n• .\\SQLEXPRESS",
                Location = new Point(20, 85),
                Width = 660,
                Height = 60,
                ForeColor = Color.DarkOrange
            };
            grpConn.Controls.Add(lblHint);
            y += 180;

            // ====================== CLOSE BUTTON ======================
            var btnClose = new Button { Text = "Close", Location = new Point(640, y + 5), Width = 90, Height = 35 };
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }

        // ====================== TEST CONNECTION ======================
        private void BtnTestConnection_Click(object sender, EventArgs e)
        {
            string server = txtServerName.Text.Trim();
            if (string.IsNullOrEmpty(server))
            {
                MessageBox.Show("Please enter a server name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = ConnectionHelper.TestConnectionDetailed(server);
            if (result.success)
            {
                ConnectionHelper.SetServerName(server);
                MessageBox.Show(
                    $"✅ Connection Successful!\n\nServer '{server}' saved.\nYou can now click Fix Database or Register USB.",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("❌ " + result.error, "Connection Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ====================== TEST DB WRITE ======================
        private void BtnTestDbWrite_Click(object sender, EventArgs e)
        {
            string connStr = ConnectionHelper.GetConnectionString();
            try
            {
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // 1. Check table exists
                    string tableCheck;
                    using (var cmd = new SqlCommand(
                        @"IF EXISTS (SELECT * FROM sysobjects WHERE name='EncryptionKeys' AND xtype='U')
                      SELECT 'OK' ELSE SELECT 'TABLE_MISSING'", conn))
                        tableCheck = cmd.ExecuteScalar()?.ToString();

                    if (tableCheck == "TABLE_MISSING")
                    {
                        MessageBox.Show(
                            "⚠️ EncryptionKeys table does not exist.\n\nClick 'Fix Database' to create it.",
                            "Table Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // 2. Try a safe write — INSERT then DELETE, no MERGE, no GETDATE() in VALUES
                    const string testKey = "__WriteTest__";
                    byte[] testData = System.Text.Encoding.UTF8.GetBytes("test");

                    // Delete any leftover test row first
                    using (var cmd = new SqlCommand(
                        "DELETE FROM EncryptionKeys WHERE KeyName = @k", conn))
                    {
                        cmd.Parameters.AddWithValue("@k", testKey);
                        cmd.ExecuteNonQuery();
                    }

                    // INSERT using GETDATE() as a column expression, not inside VALUES()
                    using (var cmd = new SqlCommand(@"
                INSERT INTO EncryptionKeys
                    (KeyName, ProtectedKeyData, DataProtectionScope, CreatedByUser, CreatedDate, IsActive)
                VALUES
                    (@k, @d, 'Test', @u, GETDATE(), 0)", conn))
                    {
                        cmd.Parameters.AddWithValue("@k", testKey);
                        cmd.Parameters.Add("@d", System.Data.SqlDbType.VarBinary).Value = testData;
                        cmd.Parameters.AddWithValue("@u", Environment.UserName);
                        cmd.ExecuteNonQuery();
                    }

                    // Clean up
                    using (var cmd = new SqlCommand(
                        "DELETE FROM EncryptionKeys WHERE KeyName = @k", conn))
                    {
                        cmd.Parameters.AddWithValue("@k", testKey);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show(
                        $"✅ DB write test passed!\n\nServer: {ConnectionHelper.GetCurrentServerName()}" +
                        $"\nDatabase: DentalClinicDB\n\nYou can now register the USB drive.",
                        "Write Test OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    $"❌ DB Write Test FAILED\n\nSQL Error #{ex.Number}: {ex.Message}\n\n" +
                    $"Connection string:\n{connStr}\n\nSteps to fix:\n1. Test Connection first\n2. Click Fix Database\n3. Retry",
                    "Write Test Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ Write test error ({ex.GetType().Name}):\n{ex.Message}\n\nConnection string:\n{connStr}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ====================== FIX DATABASE ======================
        private void BtnFixDb_Click(object sender, EventArgs e)
        {
            try
            {
                lblUsbStatus.Text = "⏳ Creating missing tables...";
                lblUsbStatus.ForeColor = Color.Orange;
                Application.DoEvents();

                string server = txtServerName.Text.Trim();
                if (string.IsNullOrEmpty(server))
                    server = ConnectionHelper.GetCurrentServerName();

                // Also update ConnectionHelper so subsequent calls use this server
                ConnectionHelper.SetServerName(server);

                var result = ServerDiscovery.CreateDatabaseIfNotExists(server, "DentalClinicDB");

                if (result.success)
                {
                    lblUsbStatus.Text = "✅ Database tables fixed successfully!";
                    lblUsbStatus.ForeColor = Color.DarkGreen;
                    MessageBox.Show("Database tables have been created/fixed.\n\nYou can now try registering the USB again.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    lblUsbStatus.Text = "❌ Failed to fix database.";
                    lblUsbStatus.ForeColor = Color.DarkRed;
                    MessageBox.Show("Fix failed:\n\n" + result.error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Fix Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ====================== USB BUTTON HANDLERS ======================
        private void BtnRegisterUsb_Click(object sender, EventArgs e)
        {
            // Diagnostic info shown on failure
            string connStr = ConnectionHelper.GetConnectionString();
            string server = ConnectionHelper.GetCurrentServerName();

            try
            {
                var result = UsbBackupManager.RegisterCurrentUsbDrive();

                lblUsbStatus.Text = result.success
                    ? "✅ USB registered!"
                    : "❌ Registration failed.";
                lblUsbStatus.ForeColor = result.success ? Color.DarkGreen : Color.DarkRed;

                if (result.success)
                {
                    MessageBox.Show(result.message, "✅ Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Show the real error from UsbBackupManager (which now propagates SQL exceptions)
                    MessageBox.Show(
                        result.message +
                        $"\n\n── Diagnostics ──\nServer: {server}\nConn: {connStr}",
                        "⚠️ Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                // Fallback — should not normally reach here since RegisterCurrentUsbDrive catches internally
                MessageBox.Show(
                    $"❌ Unexpected error during USB registration:\n\n{ex.GetType().Name}: {ex.Message}\n\nServer: {server}\nConn: {connStr}",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClearUsb_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Clear the registered backup USB?\n\nAuto-backup will be disabled until you register a new one.",
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            try
            {
                using (var conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(
                        "UPDATE EncryptionKeys SET IsActive = 0 WHERE KeyName = 'BackupUsbDriveSerial'", conn))
                        cmd.ExecuteNonQuery();
                }
                lblUsbStatus.Text = "✅ Registered USB cleared. You can register a new drive.";
                lblUsbStatus.ForeColor = Color.DarkGreen;
            }
            catch (Exception ex)
            {
                lblUsbStatus.Text = "❌ Failed to clear registration.";
                lblUsbStatus.ForeColor = Color.DarkRed;
                MessageBox.Show("Clear failed:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ====================== BACKUP & RESTORE ======================
        private void BtnDbBackup_Click(object sender, EventArgs e)
        {
            EnsureBackupFolderExists();
            using (var sfd = new SaveFileDialog())
            {
                sfd.InitialDirectory = BackupFolder;
                sfd.Filter = "SQL Backup (*.bak)|*.bak";
                sfd.FileName = $"DentalClinicDB_Backup_{DateTime.Now:yyyyMMdd_HHmm}.bak";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string server = ConnectionHelper.GetCurrentServerName();
                    var result = ServerDiscovery.BackupDatabase(server, "DentalClinicDB", sfd.FileName);
                    MessageBox.Show(result.message, result.success ? "Success" : "Failed",
                        MessageBoxButtons.OK, result.success ? MessageBoxIcon.Information : MessageBoxIcon.Error);
                }
            }
        }

        private void BtnDbRestore_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                "⚠️ WARNING: This will REPLACE the entire current database.\nAll current data will be lost.\n\nContinue?",
                "Confirm Restore", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            using (var ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = BackupFolder;
                ofd.Filter = "SQL Backup (*.bak)|*.bak";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string server = ConnectionHelper.GetCurrentServerName();
                    var result = ServerDiscovery.RestoreDatabase(server, "DentalClinicDB", ofd.FileName);
                    MessageBox.Show(result.message, result.success ? "Success" : "Failed",
                        MessageBoxButtons.OK, result.success ? MessageBoxIcon.Information : MessageBoxIcon.Error);
                    if (result.success)
                        MessageBox.Show("Please restart the application.", "Restart Required");
                }
            }
        }

        private void EnsureBackupFolderExists()
        {
            try
            {
                if (!Directory.Exists(BackupFolder))
                    Directory.CreateDirectory(BackupFolder);
                GrantFullControlToSqlService(BackupFolder);
            }
            catch
            {
                MessageBox.Show(
                    $"Could not auto-create folder.\n\nPlease manually create:\n{BackupFolder}\nand give Full Control to 'NT SERVICE\\MSSQL$SQLEXPRESS'",
                    "Folder Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void GrantFullControlToSqlService(string folderPath)
        {
            try
            {
                var di = new DirectoryInfo(folderPath);
                var security = di.GetAccessControl();
                var sid = new SecurityIdentifier("S-1-5-80-956008885-3418522649-1831038044-1853292631-2271478464");
                security.AddAccessRule(new FileSystemAccessRule(
                    sid, FileSystemRights.FullControl,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                    PropagationFlags.None, AccessControlType.Allow));
                di.SetAccessControl(security);
            }
            catch { }
        }

        // ====================== ENCRYPTION KEY METHODS ======================
        private void ExportKey(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtExportPassword.Text) || txtExportPassword.Text != txtExportConfirm.Text)
            {
                SetStatus(lblExportStatus, "❌ Passwords do not match or are empty.", false);
                return;
            }
            try
            {
                btnExportKey.Enabled = false;
                SetStatus(lblExportStatus, "⏳ Exporting key...", true);
                bool success = KeyBackupManager.ExportKeyBackup(txtExportPath.Text, txtExportPassword.Text);
                if (success)
                    SetStatus(lblExportStatus, "✅ Key backup exported successfully!", true);
            }
            catch (Exception ex)
            {
                SetStatus(lblExportStatus, "❌ Export failed.", false);
                MessageBox.Show(ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { btnExportKey.Enabled = true; }
        }

        private void ImportKey(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtImportPath.Text) || !File.Exists(txtImportPath.Text))
            {
                SetStatus(lblImportStatus, "❌ Please select a valid backup file.", false);
                return;
            }
            var confirm = MessageBox.Show("This will replace the current encryption key.\nContinue?",
                "Confirm Import", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            try
            {
                btnImportKey.Enabled = false;
                SetStatus(lblImportStatus, "⏳ Importing key...", true);
                bool success = KeyBackupManager.ImportKeyBackup(txtImportPath.Text, txtImportPassword.Text);
                if (success)
                    SetStatus(lblImportStatus, "✅ Key imported successfully!", true);
            }
            catch (Exception ex)
            {
                SetStatus(lblImportStatus, "❌ Import failed.", false);
                MessageBox.Show(ex.Message, "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { btnImportKey.Enabled = true; }
        }

        private void BrowseExportPath(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "Dental Key Backup (*.dkb)|*.dkb";
                sfd.FileName = "DentalClinic_KeyBackup.dkb";
                if (sfd.ShowDialog() == DialogResult.OK)
                    txtExportPath.Text = sfd.FileName;
            }
        }

        private void BrowseImportPath(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Dental Key Backup (*.dkb)|*.dkb";
                if (ofd.ShowDialog() == DialogResult.OK)
                    txtImportPath.Text = ofd.FileName;
            }
        }

        private void SetStatus(Label lbl, string message, bool success)
        {
            lbl.Text = message;
            lbl.ForeColor = success ? Color.DarkGreen : Color.DarkRed;
        }

        private Label AddLabel(Control parent, string text, int x, int y)
        {
            var lbl = new Label { Text = text, Location = new Point(x, y), Width = 200, Font = new Font("Arial", 8) };
            parent.Controls.Add(lbl);
            return lbl;
        }

        private TextBox AddTextBox(Control parent, int x, int y, bool isPassword, int width = 450)
        {
            var txt = new TextBox { Location = new Point(x, y), Width = width };
            if (isPassword) txt.PasswordChar = '●';
            parent.Controls.Add(txt);
            return txt;
        }
    }
}