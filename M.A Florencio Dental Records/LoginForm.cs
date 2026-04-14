using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace M.A_Florencio_Dental_Records
{
    public partial class LoginForm : MaterialForm
    {
        public static int CurrentUserID { get; set; }
        public static string CurrentUsername { get; set; }
        public static string CurrentUserRole { get; set; }

        // ✅ Brute force protection
        private int _failedAttempts = 0;
        private const int MAX_ATTEMPTS = 5;
        private bool _isLocked = false;
        private System.Windows.Forms.Timer _lockoutTimer;
        private int _lockoutSecondsRemaining = 30;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Teal500, Primary.Teal700, Primary.Teal200, Accent.Teal200, TextShade.WHITE);

            this.StartPosition = FormStartPosition.CenterScreen;
            txtUsername.Font = new System.Drawing.Font("Segoe UI", 10f);
            txtPassword.Font = new System.Drawing.Font("Segoe UI", 10f);

            this.Shown += LoginForm_Shown;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            // ✅ Block login if locked out
            if (_isLocked)
            {
                MessageBox.Show(
                    $"Too many failed attempts.\nPlease wait {_lockoutSecondsRemaining} seconds.",
                    "Account Locked", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateLoginFields()) return;

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (AuthenticateUser(username, password))
            {
                _failedAttempts = 0;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                _failedAttempts++;
                int remaining = MAX_ATTEMPTS - _failedAttempts;

                if (_failedAttempts >= MAX_ATTEMPTS)
                {
                    StartLockout();
                }
                else
                {
                    MessageBox.Show(
                        $"Invalid username or password!\n\n{remaining} attempt(s) remaining before lockout.",
                        "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                txtPassword.Clear();
                txtUsername.Focus();
            }
        }

        private void StartLockout()
        {
            _isLocked = true;
            _lockoutSecondsRemaining = 30;
            btnLogin.Enabled = false;
            txtUsername.Enabled = false;
            txtPassword.Enabled = false;

            // ✅ Log the lockout event
            TryLogAudit(0, $"LOGIN_LOCKOUT: {txtUsername.Text.Trim()}");

            _lockoutTimer = new System.Windows.Forms.Timer();
            _lockoutTimer.Interval = 1000; // 1 second
            _lockoutTimer.Tick += (s, e) =>
            {
                _lockoutSecondsRemaining--;
                btnLogin.Text = $"Wait {_lockoutSecondsRemaining}s...";

                if (_lockoutSecondsRemaining <= 0)
                {
                    _lockoutTimer.Stop();
                    _isLocked = false;
                    _failedAttempts = 0;
                    btnLogin.Enabled = true;
                    btnLogin.Text = "Login";
                    txtUsername.Enabled = true;
                    txtPassword.Enabled = true;
                    txtPassword.Clear();
                    txtUsername.Focus();
                }
            };

            _lockoutTimer.Start();
            MessageBox.Show(
                $"Too many failed login attempts.\nAccount locked for {_lockoutSecondsRemaining} seconds.",
                "Account Locked", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private bool AuthenticateUser(string username, string password)
        {
            try
            {
                string connStr = ConnectionHelper.GetConnectionString();
                if (string.IsNullOrEmpty(connStr))
                {
                    MessageBox.Show("Database not configured. Please run setup again.");
                    return false;
                }

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // ✅ Also check IsActive so disabled accounts can't login
                    string query = @"SELECT UserID, PasswordHash, Role 
                                     FROM Users 
                                     WHERE Username = @Username AND IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read()) return false;

                            string storedHash = reader["PasswordHash"]?.ToString();
                            if (string.IsNullOrEmpty(storedHash)) return false;

                            if (PasswordHelper.VerifyPassword(password, storedHash))
                            {
                                CurrentUserID = Convert.ToInt32(reader["UserID"]);
                                CurrentUsername = username;
                                CurrentUserRole = reader["Role"]?.ToString();

                                TryLogAudit(CurrentUserID, "LOGIN");
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Login error: " + ex.Message);
                return false;
            }
        }

        private void TryLogAudit(int userID, string action)
        {
            try
            {
                string connStr = ConnectionHelper.GetConnectionString();
                if (string.IsNullOrEmpty(connStr)) return;

                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO AuditLog (UserID, Action, IPAddress) VALUES (@UserID, @Action, @IPAddress)", conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    cmd.Parameters.AddWithValue("@Action", action);
                    cmd.Parameters.AddWithValue("@IPAddress", "127.0.0.1");
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }
        }

        private bool ValidateLoginFields()
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Enter username!"); return false;
            }
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Enter password!"); return false;
            }
            return true;
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            using (RegisterForm registerForm = new RegisterForm())
                registerForm.ShowDialog();

            txtUsername.Clear();
            txtPassword.Clear();
        }

        private void btnForgotPassword_Click(object sender, EventArgs e)
        {
            using (ForgotPasswordForm forgotForm = new ForgotPasswordForm())
                forgotForm.ShowDialog();
        }

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _lockoutTimer?.Stop();

            if (this.DialogResult != DialogResult.OK)
                this.DialogResult = DialogResult.Cancel;

            txtPassword.Clear();
        }

        private void LoginForm_Shown(object sender, EventArgs e)
        {
            txtUsername.Font = new System.Drawing.Font("Segoe UI", 10f);
            txtPassword.Font = new System.Drawing.Font("Segoe UI", 10f);
        }
    }
}