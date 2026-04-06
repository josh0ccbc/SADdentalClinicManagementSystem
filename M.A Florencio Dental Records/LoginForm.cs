using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace M.A_Florencio_Dental_Records
{
    public partial class LoginForm : MaterialForm
    {
        string connectionString = @"Data Source=DESKTOP-ASL74A6;Initial Catalog=DentalClinicDB;Integrated Security=True";
        public int LoggedInUserID { get; set; }
        public string LoggedInUsername { get; set; }

        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            // Set MaterialSkin theme
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Teal500, Primary.Teal700, Primary.Teal200, Accent.Teal200, TextShade.WHITE);

            this.StartPosition = FormStartPosition.CenterScreen;
        }

        // ✅ LOGIN BUTTON
        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (!ValidateLoginFields())
                return;

            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (AuthenticateUser(username, password))
            {
                // ✅ SET DIALOGRESULT.OK to signal successful login
                this.DialogResult = DialogResult.OK;

                // Pass data to Form1 (we'll get it from properties)
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password!", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Clear();
                txtUsername.Focus();
                // ✅ DON'T close the form - keep it open
            }
        }

        // ✅ AUTHENTICATE USER
        private bool AuthenticateUser(string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT UserID, PasswordHash, FullName FROM Users WHERE Username = @Username AND IsActive = 1";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string storedHash = reader["PasswordHash"].ToString();

                    // Verify password
                    if (PasswordHelper.VerifyPassword(password, storedHash))
                    {
                        LoggedInUserID = Convert.ToInt32(reader["UserID"]);
                        LoggedInUsername = username;

                        conn.Close();

                        // Log the login
                        LogAuditTrail(LoggedInUserID, "LOGIN");

                        return true;
                    }
                }

                conn.Close();
                return false;
            }
        }

        // ✅ LOG AUDIT TRAIL
        private void LogAuditTrail(int userID, string action)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO AuditLog (UserID, Action, IPAddress) VALUES (@UserID, @Action, @IPAddress)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@IPAddress", GetIPAddress());

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        private string GetIPAddress()
        {
            return "127.0.0.1"; // Localhost for now
        }

        // ✅ VALIDATE LOGIN FIELDS
        private bool ValidateLoginFields()
        {
            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                MessageBox.Show("Enter username!");
                return false;
            }

            if (string.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("Enter password!");
                return false;
            }

            return true;
        }

        // ✅ REGISTER BUTTON
        private void btnRegister_Click(object sender, EventArgs e)
        {
            RegisterForm registerForm = new RegisterForm();
            registerForm.ShowDialog();  // Wait for RegisterForm to close

            // Refresh if new user registered
            txtUsername.Clear();
            txtPassword.Clear();
        }

        // ✅ FORGOT PASSWORD BUTTON
        private void btnForgotPassword_Click(object sender, EventArgs e)
        {
            ForgotPasswordForm forgotForm = new ForgotPasswordForm();
            forgotForm.ShowDialog();  // Wait for ForgotPasswordForm to close
        }

        // ✅ CLEAR PASSWORD WHEN FORM CLOSES
        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If user clicks X button (not logging in), allow exit
            if (this.DialogResult != DialogResult.OK)
            {
                this.DialogResult = DialogResult.Cancel;  // Signal to Program.cs to exit
            }

            txtPassword.Clear();
        }

        public static int CurrentUserID { get; set; }
        public static string CurrentUsername { get; set; }
    }
}