using System;
using System.IO;
using System.Windows.Forms;

namespace M.A_Florencio_Dental_Records
{
    public class AdminKeyBackupForm : Form
    {
        // Export controls
        private TextBox txtExportPassword;
        private TextBox txtExportConfirm;
        private TextBox txtExportPath;
        private Button btnExport;
        private Label lblExportStatus;

        // Import controls
        private TextBox txtImportPassword;
        private TextBox txtImportPath;
        private Button btnImport;
        private Label lblImportStatus;

        public AdminKeyBackupForm()
        {
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "Security — Encryption Key Backup & Recovery";
            this.Width = 620;
            this.Height = 580;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // ── EXPORT SECTION ──────────────────────────────────────
            var grpExport = new GroupBox();
            grpExport.Text = "📤 Export Key Backup (Do this on working PC)";
            grpExport.Location = new System.Drawing.Point(15, 15);
            grpExport.Width = 570;
            grpExport.Height = 230;
            grpExport.Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold);
            this.Controls.Add(grpExport);

            var lblWarning = new Label();
            lblWarning.Text = "⚠️  Store the backup file and password in a safe place (e.g. USB drive in locked drawer).\n" +
                              "     Without both, data CANNOT be recovered if this PC is replaced.";
            lblWarning.Location = new System.Drawing.Point(10, 22);
            lblWarning.Width = 545;
            lblWarning.Height = 40;
            lblWarning.Font = new System.Drawing.Font("Arial", 8);
            lblWarning.ForeColor = System.Drawing.Color.DarkRed;
            grpExport.Controls.Add(lblWarning);

            AddLabel(grpExport, "Backup Password:", 10, 68);
            txtExportPassword = AddTextBox(grpExport, 10, 88, true);

            AddLabel(grpExport, "Confirm Password:", 10, 115);
            txtExportConfirm = AddTextBox(grpExport, 10, 135, true);

            AddLabel(grpExport, "Save Backup File To:", 10, 160);
            txtExportPath = AddTextBox(grpExport, 10, 178, false);
            txtExportPath.Width = 400;
            txtExportPath.Text = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "DentalClinic_KeyBackup.dkb");

            var btnBrowseExport = new Button();
            btnBrowseExport.Text = "Browse...";
            btnBrowseExport.Location = new System.Drawing.Point(420, 176);
            btnBrowseExport.Width = 90;
            btnBrowseExport.Click += BrowseExportPath;
            grpExport.Controls.Add(btnBrowseExport);

            btnExport = new Button();
            btnExport.Text = "📤 Export Key Backup";
            btnExport.Location = new System.Drawing.Point(10, 205);
            btnExport.Width = 200;
            btnExport.Height = 32;
            btnExport.BackColor = System.Drawing.Color.SteelBlue;
            btnExport.ForeColor = System.Drawing.Color.White;
            btnExport.FlatStyle = FlatStyle.Flat;
            btnExport.Click += ExportKey;
            grpExport.Controls.Add(btnExport);

            lblExportStatus = new Label();
            lblExportStatus.Location = new System.Drawing.Point(220, 210);
            lblExportStatus.Width = 330;
            lblExportStatus.Height = 25;
            lblExportStatus.Font = new System.Drawing.Font("Arial", 8);
            grpExport.Controls.Add(lblExportStatus);

            // ── IMPORT SECTION ──────────────────────────────────────
            var grpImport = new GroupBox();
            grpImport.Text = "📥 Import Key Backup (Use this on replacement PC)";
            grpImport.Location = new System.Drawing.Point(15, 260);
            grpImport.Width = 570;
            grpImport.Height = 190;
            grpImport.Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold);
            this.Controls.Add(grpImport);

            var lblImportNote = new Label();
            lblImportNote.Text = "ℹ️  Use this after restoring the database backup to a new PC.\n" +
                                 "    The app will be able to read all existing patient data after import.";
            lblImportNote.Location = new System.Drawing.Point(10, 22);
            lblImportNote.Width = 545;
            lblImportNote.Height = 35;
            lblImportNote.Font = new System.Drawing.Font("Arial", 8);
            lblImportNote.ForeColor = System.Drawing.Color.DarkBlue;
            grpImport.Controls.Add(lblImportNote);

            AddLabel(grpImport, "Backup File:", 10, 63);
            txtImportPath = AddTextBox(grpImport, 10, 82, false);
            txtImportPath.Width = 400;

            var btnBrowseImport = new Button();
            btnBrowseImport.Text = "Browse...";
            btnBrowseImport.Location = new System.Drawing.Point(420, 80);
            btnBrowseImport.Width = 90;
            btnBrowseImport.Click += BrowseImportPath;
            grpImport.Controls.Add(btnBrowseImport);

            AddLabel(grpImport, "Backup Password:", 10, 108);
            txtImportPassword = AddTextBox(grpImport, 10, 127, true);

            btnImport = new Button();
            btnImport.Text = "📥 Import & Restore Key";
            btnImport.Location = new System.Drawing.Point(10, 155);
            btnImport.Width = 200;
            btnImport.Height = 32;
            btnImport.BackColor = System.Drawing.Color.DarkGreen;
            btnImport.ForeColor = System.Drawing.Color.White;
            btnImport.FlatStyle = FlatStyle.Flat;
            btnImport.Click += ImportKey;
            grpImport.Controls.Add(btnImport);

            lblImportStatus = new Label();
            lblImportStatus.Location = new System.Drawing.Point(220, 160);
            lblImportStatus.Width = 330;
            lblImportStatus.Height = 25;
            lblImportStatus.Font = new System.Drawing.Font("Arial", 8);
            grpImport.Controls.Add(lblImportStatus);

            // ── CLOSE BUTTON ────────────────────────────────────────
            var btnClose = new Button();
            btnClose.Text = "Close";
            btnClose.Location = new System.Drawing.Point(490, 470);
            btnClose.Width = 90;
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }

        // ── EVENT HANDLERS ──────────────────────────────────────────

        private void ExportKey(object sender, EventArgs e)
        {
            // Validate passwords match
            if (string.IsNullOrWhiteSpace(txtExportPassword.Text))
            {
                SetStatus(lblExportStatus, "❌ Password cannot be empty.", false);
                return;
            }
            if (txtExportPassword.Text != txtExportConfirm.Text)
            {
                SetStatus(lblExportStatus, "❌ Passwords do not match.", false);
                return;
            }
            if (txtExportPassword.Text.Length < 8)
            {
                SetStatus(lblExportStatus, "❌ Password must be at least 8 characters.", false);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtExportPath.Text))
            {
                SetStatus(lblExportStatus, "❌ Please choose a save location.", false);
                return;
            }

            try
            {
                btnExport.Enabled = false;
                SetStatus(lblExportStatus, "⏳ Exporting...", true);

                bool success = KeyBackupManager.ExportKeyBackup(
                    txtExportPath.Text,
                    txtExportPassword.Text);

                if (success)
                {
                    SetStatus(lblExportStatus, "✅ Backup exported successfully!", true);
                    MessageBox.Show(
                        "✅ Key backup exported successfully!\n\n" +
                        $"File: {txtExportPath.Text}\n\n" +
                        "NEXT STEPS:\n" +
                        "1. Copy this file to a USB drive\n" +
                        "2. Store the USB in a locked, secure location\n" +
                        "3. Write down the backup password and store it separately\n" +
                        "4. Never store the file and password in the same place",
                        "Export Successful",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                SetStatus(lblExportStatus, "❌ Export failed.", false);
                MessageBox.Show(
                    $"Export failed:\n\n{ex.Message}",
                    "Export Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnExport.Enabled = true;
            }
        }

        private void ImportKey(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtImportPath.Text) || !File.Exists(txtImportPath.Text))
            {
                SetStatus(lblImportStatus, "❌ Please select a valid backup file.", false);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtImportPassword.Text))
            {
                SetStatus(lblImportStatus, "❌ Password cannot be empty.", false);
                return;
            }

            // Final confirmation — this is irreversible
            var confirm = MessageBox.Show(
                "⚠️ This will replace the current encryption key on this machine.\n\n" +
                "Only proceed if:\n" +
                "  • You are on a NEW or REPLACEMENT PC\n" +
                "  • You have already restored the database backup\n\n" +
                "Are you sure you want to continue?",
                "Confirm Key Import",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                btnImport.Enabled = false;
                SetStatus(lblImportStatus, "⏳ Importing...", true);

                bool success = KeyBackupManager.ImportKeyBackup(
                    txtImportPath.Text,
                    txtImportPassword.Text);

                if (success)
                {
                    SetStatus(lblImportStatus, "✅ Key restored successfully!", true);
                    MessageBox.Show(
                        "✅ Encryption key restored successfully!\n\n" +
                        "All patient data can now be decrypted normally.\n\n" +
                        "Please restart the application.",
                        "Import Successful",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                SetStatus(lblImportStatus, "❌ Import failed.", false);
                MessageBox.Show(
                    $"Import failed:\n\n{ex.Message}",
                    "Import Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnImport.Enabled = true;
            }
        }

        private void BrowseExportPath(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Title = "Save Key Backup File";
                sfd.Filter = "Dental Key Backup (*.dkb)|*.dkb|All Files (*.*)|*.*";
                sfd.FileName = "DentalClinic_KeyBackup.dkb";
                if (sfd.ShowDialog() == DialogResult.OK)
                    txtExportPath.Text = sfd.FileName;
            }
        }

        private void BrowseImportPath(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Select Key Backup File";
                ofd.Filter = "Dental Key Backup (*.dkb)|*.dkb|All Files (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                    txtImportPath.Text = ofd.FileName;
            }
        }

        // ── HELPERS ─────────────────────────────────────────────────

        private void SetStatus(Label lbl, string message, bool success)
        {
            lbl.Text = message;
            lbl.ForeColor = success ? System.Drawing.Color.DarkGreen : System.Drawing.Color.DarkRed;
            Application.DoEvents();
        }

        private Label AddLabel(Control parent, string text, int x, int y)
        {
            var lbl = new Label();
            lbl.Text = text;
            lbl.Location = new System.Drawing.Point(x, y);
            lbl.Width = 200;
            lbl.Font = new System.Drawing.Font("Arial", 8);
            parent.Controls.Add(lbl);
            return lbl;
        }

        private TextBox AddTextBox(Control parent, int x, int y, bool isPassword)
        {
            var txt = new TextBox();
            txt.Location = new System.Drawing.Point(x, y);
            txt.Width = 500;
            if (isPassword) txt.PasswordChar = '●';
            parent.Controls.Add(txt);
            return txt;
        }
    }
}