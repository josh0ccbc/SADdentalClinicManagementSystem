using System;
using System.Drawing;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Windows.Forms;

namespace M.A_Florencio_Dental_Records
{
    public class AdminKeyBackupForm : Form
    {
        // Controls
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

        private const string BackupFolder = @"C:\DentalClinicBackups";
        private const string SqlInstance = @".\SQLEXPRESS";   // ← Fixed for your setup

        public AdminKeyBackupForm()
        {
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "Security — Backup & Recovery";
            this.Width = 760;
            this.Height = 730;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new Size(0, 680);

            int y = 15;

            // ====================== ENCRYPTION KEY EXPORT ======================
            var grpKeyExport = new GroupBox
            {
                Text = "📤 Encryption Key Backup (Do this on working PC)",
                Location = new Point(20, y),
                Width = 710,
                Height = 265
            };
            this.Controls.Add(grpKeyExport);

            var lblKeyWarning = new Label
            {
                Text = "⚠️ Store this backup file and password safely.\nWithout both, patient data cannot be recovered.",
                Location = new Point(20, 25),
                Width = 660,
                Height = 40,
                ForeColor = Color.DarkRed
            };
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

            btnExportKey = new Button
            {
                Text = "📤 Export Encryption Key",
                Location = new Point(20, 225),
                Width = 250,
                Height = 40,
                BackColor = Color.SteelBlue,
                ForeColor = Color.White
            };
            btnExportKey.Click += ExportKey;
            grpKeyExport.Controls.Add(btnExportKey);

            lblExportStatus = new Label { Location = new Point(285, 230), Width = 400, Height = 30 };
            grpKeyExport.Controls.Add(lblExportStatus);

            y += 280;

            // ====================== ENCRYPTION KEY IMPORT ======================
            var grpKeyImport = new GroupBox
            {
                Text = "📥 Import Encryption Key (On new or replacement PC)",
                Location = new Point(20, y),
                Width = 710,
                Height = 190
            };
            this.Controls.Add(grpKeyImport);

            AddLabel(grpKeyImport, "Key Backup File:", 20, 28);
            txtImportPath = AddTextBox(grpKeyImport, 20, 48, false, 530);

            var btnBrowseImport = new Button { Text = "Browse...", Location = new Point(565, 46), Width = 110 };
            btnBrowseImport.Click += BrowseImportPath;
            grpKeyImport.Controls.Add(btnBrowseImport);

            AddLabel(grpKeyImport, "Backup Password:", 20, 82);
            txtImportPassword = AddTextBox(grpKeyImport, 20, 102, true, 480);

            btnImportKey = new Button
            {
                Text = "📥 Import & Restore Key",
                Location = new Point(20, 140),
                Width = 250,
                Height = 42,
                BackColor = Color.DarkGreen,
                ForeColor = Color.White
            };
            btnImportKey.Click += ImportKey;
            grpKeyImport.Controls.Add(btnImportKey);

            lblImportStatus = new Label { Location = new Point(285, 145), Width = 400, Height = 30 };
            grpKeyImport.Controls.Add(lblImportStatus);

            y += 205;

            // ====================== FULL DATABASE BACKUP & RESTORE ======================
            var grpDb = new GroupBox
            {
                Text = "💾 Full Database Backup & Restore",
                Location = new Point(20, y),
                Width = 710,
                Height = 205
            };
            this.Controls.Add(grpDb);

            var lblDbNote = new Label
            {
                Text = "This backs up EVERYTHING: patients, medical history, appointments, prescriptions, etc.",
                Location = new Point(20, 25),
                Width = 660,
                Height = 35,
                ForeColor = Color.DarkBlue
            };
            grpDb.Controls.Add(lblDbNote);

            var lblWarning = new Label
            {
                Text = "⚠️ Recommended: Save backups to C:\\DentalClinicBackups\n(SQL Server service needs write permission)",
                Location = new Point(20, 62),
                Width = 660,
                Height = 38,
                ForeColor = Color.DarkOrange,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            grpDb.Controls.Add(lblWarning);

            btnDbBackup = new Button
            {
                Text = "📤 Backup Full Database (.bak)",
                Location = new Point(20, 105),
                Width = 320,
                Height = 48,
                BackColor = Color.Teal,
                ForeColor = Color.White
            };
            btnDbBackup.Click += BtnDbBackup_Click;
            grpDb.Controls.Add(btnDbBackup);

            btnDbRestore = new Button
            {
                Text = "📥 Restore Database from .bak",
                Location = new Point(360, 105),
                Width = 320,
                Height = 48,
                BackColor = Color.OrangeRed,
                ForeColor = Color.White
            };
            btnDbRestore.Click += BtnDbRestore_Click;
            grpDb.Controls.Add(btnDbRestore);

            lblDbStatus = new Label
            {
                Location = new Point(20, 160),
                Width = 660,
                Height = 35,
                Font = new Font("Arial", 8)
            };
            grpDb.Controls.Add(lblDbStatus);

            // Close Button
            var btnClose = new Button
            {
                Text = "Close",
                Location = new Point(640, this.Height - 75),
                Width = 90,
                Height = 35,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }

        // ====================== BACKUP & RESTORE (Using SQLEXPRESS) ======================
        private void BtnDbBackup_Click(object sender, EventArgs e)
        {
            EnsureBackupFolderExists();

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.InitialDirectory = BackupFolder;
                sfd.Filter = "SQL Backup (*.bak)|*.bak";
                sfd.FileName = $"DentalClinicDB_Backup_{DateTime.Now:yyyyMMdd_HHmm}.bak";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    var result = ServerDiscovery.BackupDatabase(SqlInstance, "DentalClinicDB", sfd.FileName);
                    MessageBox.Show(result.message,
                        result.success ? "Success" : "Failed",
                        MessageBoxButtons.OK,
                        result.success ? MessageBoxIcon.Information : MessageBoxIcon.Error);
                }
            }
        }

        private void BtnDbRestore_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                "⚠️ WARNING: This will REPLACE the entire current database.\nAll current data will be lost.\n\nContinue?",
                "Confirm Restore", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = BackupFolder;
                ofd.Filter = "SQL Backup (*.bak)|*.bak";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var result = ServerDiscovery.RestoreDatabase(SqlInstance, "DentalClinicDB", ofd.FileName);

                    MessageBox.Show(result.message,
                        result.success ? "Success" : "Failed",
                        MessageBoxButtons.OK,
                        result.success ? MessageBoxIcon.Information : MessageBoxIcon.Error);

                    if (result.success)
                        MessageBox.Show("Please restart the application.", "Restart Required");
                }
            }
        }

        // ====================== AUTO FOLDER & PERMISSIONS ======================
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
                MessageBox.Show($"Could not auto-create folder.\n\nPlease manually create:\n{BackupFolder}\nand give Full Control to 'NT SERVICE\\MSSQL$SQLEXPRESS'",
                    "Folder Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void GrantFullControlToSqlService(string folderPath)
        {
            try
            {
                var di = new DirectoryInfo(folderPath);
                var security = di.GetAccessControl();

                // For SQL Express
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
            var lbl = new Label
            {
                Text = text,
                Location = new Point(x, y),
                Width = 200,
                Font = new Font("Arial", 8)
            };
            parent.Controls.Add(lbl);
            return lbl;
        }

        private TextBox AddTextBox(Control parent, int x, int y, bool isPassword, int width = 450)
        {
            var txt = new TextBox
            {
                Location = new Point(x, y),
                Width = width
            };
            if (isPassword) txt.PasswordChar = '●';
            parent.Controls.Add(txt);
            return txt;
        }
    }
}