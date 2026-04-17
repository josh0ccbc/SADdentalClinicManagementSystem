using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using System.Data.SqlClient;

namespace M.A_Florencio_Dental_Records
{
    public partial class Form1 : MaterialForm
    {
        Button NavButton;

        public int LoggedInUserID { get; set; }
        public string LoggedInUsername { get; set; }
        private System.Windows.Forms.Timer _inactivityTimer;
        private const int INACTIVITY_TIMEOUT_SECONDS = 300;
        private int _inactivitySecondsRemaining;

        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to logout?",
                "Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                LogAuditTrail(LoggedInUserID, "LOGOUT");
                this.Close(); // Returns to the while loop in Program.cs → login shown again
            }
        }

        private void LogAuditTrail(int userID, string action)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    string query = "INSERT INTO AuditLog (UserID, Action, IPAddress) VALUES (@UserID, @Action, @IPAddress)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    cmd.Parameters.AddWithValue("@Action", action);
                    cmd.Parameters.AddWithValue("@IPAddress", "127.0.0.1");
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }
        }

        public Form1()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;

            materialSkinManager.AddFormToManage(this);

            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;

            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Teal500,
                Primary.Teal700,
                Primary.Teal200,
                Accent.Teal200,
                TextShade.WHITE
            );        
        }

        void UseButton(Button btn)
        {
            if (NavButton != null)
            {
                NavButton.BackColor = Color.FromArgb(255, 255, 255); 
                NavButton.ForeColor = Color.Black;
            }

            NavButton = btn;
            NavButton.BackColor = Color.FromArgb(95, 158, 160); 
            NavButton.ForeColor = Color.White;
        }

        void ActivateButton(Button btn)
        {
            if (NavButton != null)
            {
                NavButton.BackColor = Color.FromArgb(255, 255, 255);
                NavButton.ForeColor = Color.Black;
            }

            button1.BackColor = Color.White;
            button1.ForeColor = Color.Black;

            NavButton = btn;
            NavButton.BackColor = Color.FromArgb(95, 158, 160);
            NavButton.ForeColor = Color.White;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoggedInUserID = LoginForm.CurrentUserID;
            LoggedInUsername = LoginForm.CurrentUsername;
            this.Text = "M.A. Florencio Dental Records - " + LoggedInUsername;

            LoadControl(new DBcontrol());
            FormPnl.Visible = true;
            ActivateButton(button1);

            // ✅ Start inactivity timer
            StartInactivityTimer();

            // ✅ Hook mouse/keyboard activity on the form
            this.MouseMove += ResetInactivityTimer;
            this.KeyPress += ResetInactivityTimer;
            Application.AddMessageFilter(new ActivityFilter(ResetInactivityTimer));
        }

        private void ResetInactivityTimer(object sender, EventArgs e)
        {
            _inactivitySecondsRemaining = INACTIVITY_TIMEOUT_SECONDS;
        }

        private void StartInactivityTimer()
        {
            _inactivitySecondsRemaining = INACTIVITY_TIMEOUT_SECONDS;

            _inactivityTimer = new System.Windows.Forms.Timer();
            _inactivityTimer.Interval = 1000;
            _inactivityTimer.Tick += (s, e) =>
            {
                _inactivitySecondsRemaining--;

                // ✅ Warn user 60 seconds before logout
                if (_inactivitySecondsRemaining == 60)
                {
                    MessageBox.Show(
                        "You will be automatically logged out in 60 seconds due to inactivity.",
                        "Inactivity Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                if (_inactivitySecondsRemaining <= 0)
                {
                    _inactivityTimer.Stop();
                    LogAuditTrail(LoggedInUserID, "AUTO_LOGOUT_INACTIVITY");
                    this.Close();
                }
            };

            _inactivityTimer.Start();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormPnl.Visible = true;
            LoadControl(new DBcontrol());
            ActivateButton((Button)sender);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ActivateButton((Button)sender);
            LoadControl(new patientControl());
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void BTNAddPatient_MouseEnter_1(object sender, EventArgs e)
        {
            
        }

        private void BTNAddPatient_MouseLeave(object sender, EventArgs e)
        {
            
        }

        private void BTNAddPatient_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
        public void LoadControl(UserControl control)
        {
            FormPnl.Controls.Clear();
            control.Width = FormPnl.Width;   // ✅ match panel width
            control.Left = 0;
            control.Top = 0;
            // ✅ NO Dock.Fill - let height be dynamic
            FormPnl.Controls.Add(control);

            FormPnl.AutoScroll = false;
            FormPnl.HorizontalScroll.Maximum = 0;
            FormPnl.HorizontalScroll.Enabled = false;
            FormPnl.HorizontalScroll.Visible = false;
            FormPnl.AutoScroll = true;
        }

        private void panelContainer_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            LoadControl(new appointmentsControl());
            ActivateButton((Button)sender);
        }

        private void NAVArchive_Click(object sender, EventArgs e)
        {
            LoadControl(new Archive());
            ActivateButton((Button)sender);
        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private class ActivityFilter : IMessageFilter
        {
            private readonly EventHandler _resetCallback;
            public ActivityFilter(EventHandler callback) { _resetCallback = callback; }

            public bool PreFilterMessage(ref Message m)
            {
                // WM_MOUSEMOVE = 0x200, WM_KEYDOWN = 0x100, WM_LBUTTONDOWN = 0x201
                if (m.Msg == 0x200 || m.Msg == 0x100 || m.Msg == 0x201)
                    _resetCallback(this, EventArgs.Empty);
                return false;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _inactivityTimer?.Stop();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string originalText = "Patient allergic to Penicillin";

            System.Diagnostics.Debug.WriteLine("=== CRYPTO TEST ===");
            System.Diagnostics.Debug.WriteLine($"Original: {originalText}");

            string encrypted = CryptoHelper.Encrypt(originalText);
            System.Diagnostics.Debug.WriteLine($"Encrypted: {encrypted}");

            string decrypted = CryptoHelper.Decrypt(encrypted);
            System.Diagnostics.Debug.WriteLine($"Decrypted: {decrypted}");

            if (decrypted == originalText)
            {
                MessageBox.Show("✅ Encryption/Decryption works!", "Test Passed");
            }
            else
            {
                MessageBox.Show("❌ Mismatch!", "Test Failed");
            }
        }

        private void btnSecurityKeyBackup_Click(object sender, EventArgs e)
        {
            // Only allow Admin role
            if (LoginForm.CurrentUserRole != "Admin")
            {
                MessageBox.Show("Access denied. Admin privileges required.",
                    "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Step 1: Ask for Admin Password
            using (AdminPasswordDialog passwordDialog = new AdminPasswordDialog())
            {
                DialogResult result = passwordDialog.ShowDialog();

                if (result != DialogResult.OK || !passwordDialog.IsVerified)
                {
                    return; // User cancelled or wrong password
                }
            }

            // Step 2: Password verified → Open Key Backup Form
            using (AdminKeyBackupForm backupForm = new AdminKeyBackupForm())
            {
                backupForm.ShowDialog();
            }
        }

        private void btnMockData_Click(object sender, EventArgs e)
        {
            if (LoginForm.CurrentUserRole != "Admin")
            {
                MessageBox.Show("Admin only.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var form = new MockDataForm();
            form.ShowDialog();
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            if (LoginForm.CurrentUserRole != "Admin")
            {
                MessageBox.Show("Access denied. Admins only.", "Restricted",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (DatabaseViewerForm viewer = new DatabaseViewerForm())
            {
                viewer.ShowDialog();
            }
        }
    }
}
