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


        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to logout?",
                "Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Log logout
                LogAuditTrail(LoggedInUserID, "LOGOUT");

                // ✅ CLOSE FORM1 - Program.cs will show login again
                this.Close();
            }
        }

        private void LogAuditTrail(int userID, string action)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionSettings.Current.GetConnectionString()))
            {
                string query = "INSERT INTO AuditLog (UserID, Action, IPAddress) VALUES (@UserID, @Action, @IPAddress)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@IPAddress", "127.0.0.1");

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                catch { }
            }
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

            // Show username in form (optional)
            this.Text = "M.A. Florencio Dental Records - " + LoggedInUsername;

            LoadControl(new DBcontrol());
            FormPnl.Visible = true;
            ActivateButton(button1);
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

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
            // Only allow admin users
            if (LoginForm.CurrentUserRole != "Admin")
            {
                MessageBox.Show("Access denied. Admin privileges required.",
                    "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var form = new AdminKeyBackupForm();
            form.ShowDialog();
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
    }
}
