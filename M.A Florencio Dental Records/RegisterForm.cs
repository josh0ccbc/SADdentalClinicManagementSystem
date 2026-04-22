using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Linq; // ✅ fixes Enumerable error
using MaterialSkin;
using MaterialSkin.Controls;

namespace M.A_Florencio_Dental_Records
{
    public partial class RegisterForm : MaterialForm
    {
        private bool isUpdating = false;

        public RegisterForm()
        {
            InitializeComponent();
        }

        private void RegisterForm_Load(object sender, EventArgs e)
        {
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Teal500, Primary.Teal700, Primary.Teal200, Accent.Teal200, TextShade.WHITE);

            this.StartPosition = FormStartPosition.CenterScreen;

            txtUsername.TextChanged += txtUsername_TextChanged;
            txtEmail.TextChanged += txtEmail_TextChanged;
            txtEmail.Text = "";
            txtEmail.SelectionStart = 0;  // ✅ Cursor at beginning
            lblUsernameStatus.Text = "";
            lblEmailStatus.Text = "";
        }

        // ✅ REGISTER BUTTON
        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (!ValidateFields())
                return;

            // ── Require admin password before creating account ────────────
            using (AdminPasswordDialog adminDialog = new AdminPasswordDialog())
            {
                DialogResult adminResult = adminDialog.ShowDialog();

                if (adminResult != DialogResult.OK || !adminDialog.IsVerified)
                {
                    MessageBox.Show(
                        "Admin verification failed. Staff account was not created.",
                        "Access Denied",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
            }

            string username = txtUsername.Text;
            string email = txtEmail.Text;
            string password = txtPassword.Text;
            string fullName = txtFullName.Text;

            if (UserExists(username, email))
            {
                MessageBox.Show(
                    "Username or email already exists!",
                    "Registration Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // Hash password
            string passwordHash = PasswordHelper.HashPassword(password);

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    string query = @"
                        INSERT INTO Users (Username, Email, PasswordHash, FullName, Role, IsActive)
                        VALUES (@Username, @Email, @PasswordHash, @FullName, @Role, @IsActive)";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                    cmd.Parameters.AddWithValue("@FullName", fullName);
                    cmd.Parameters.AddWithValue("@Role", "Staff");
                    cmd.Parameters.AddWithValue("@IsActive", 1);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();

                    MessageBox.Show(
                        "Account created successfully! The new staff member can now login.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error: " + ex.Message,
                    "Registration Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ✅ CHECK IF USER EXISTS
        private bool UserExists(string username, string email)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
            {
                string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username OR Email = @Email";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Email", email);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                conn.Close();

                return count > 0;
            }
        }

        // ✅ VALIDATE FIELDS
        private bool ValidateFields()
        {
            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                MessageBox.Show("Enter username!");
                return false;
            }

            if (string.IsNullOrEmpty(txtEmail.Text))
            {
                MessageBox.Show("Enter email!");
                return false;
            }

            if (string.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("Enter password!");
                return false;
            }

            if (string.IsNullOrEmpty(txtConfirmPassword.Text))
            {
                MessageBox.Show("Confirm password!");
                return false;
            }

            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Passwords do not match!");
                return false;
            }

            if (txtPassword.Text.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters!");
                return false;
            }

            if (string.IsNullOrEmpty(txtFullName.Text))
            {
                MessageBox.Show("Enter full name!");
                return false;
            }

            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private bool UsernameExists(string username)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
            {
                string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        private bool EmailExists(string email)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
            {
                string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        private void txtEmail_TextChanged(object sender, EventArgs e)
        {
            if (isUpdating) return;
            isUpdating = true;

            string suffix = "@gmail.com";
            string current = txtEmail.Text;

            if (string.IsNullOrEmpty(current))
            {
                lblEmailStatus.Text = "";
                isUpdating = false;
                return;
            }

            if (current.EndsWith(suffix))
            {
                int pos = current.Length - suffix.Length;
                if (txtEmail.SelectionStart > pos)
                    txtEmail.SelectionStart = pos;
            }
            else
            {
                string baseText = current;
                for (int len = suffix.Length; len >= 1; len--)
                {
                    if (baseText.EndsWith(suffix.Substring(0, len)))
                    {
                        baseText = baseText.Substring(0, baseText.Length - len);
                        break;
                    }
                }
                txtEmail.Text = baseText + suffix;
                txtEmail.SelectionStart = baseText.Length;
            }

            isUpdating = false;

            // ✅ Check email after suffix logic
            string email = txtEmail.Text.Trim();
            if (email != suffix) // don't check if only "@gmail.com"
            {
                if (EmailExists(email))
                {
                    lblEmailStatus.Text = "✗ Email already registered";
                    lblEmailStatus.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblEmailStatus.Text = "✓ Email available";
                    lblEmailStatus.ForeColor = System.Drawing.Color.Green;
                }
            }
            else
            {
                lblEmailStatus.Text = "";
            }
        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();

            if (string.IsNullOrEmpty(username))
            {
                lblUsernameStatus.Text = "";
                return;
            }

            if (UsernameExists(username))
            {
                lblUsernameStatus.Text = "✗ Username already taken";
                lblUsernameStatus.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                lblUsernameStatus.Text = "✓ Username available";
                lblUsernameStatus.ForeColor = System.Drawing.Color.Green;
            }
        }
    }
}