using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace M.A_Florencio_Dental_Records
{
    public partial class ForgotPasswordForm : MaterialForm
    {
        public ForgotPasswordForm()
        {
            InitializeComponent();
        }

        private void ForgotPasswordForm_Load(object sender, EventArgs e)
        {
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Teal500, Primary.Teal700, Primary.Teal200, Accent.Teal200, TextShade.WHITE);

            this.StartPosition = FormStartPosition.CenterScreen;
        }

        // ✅ RESET PASSWORD BUTTON
        private void btnReset_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtEmail.Text))
            {
                MessageBox.Show("Enter email address!");
                return;
            }

            // ✅ Check if email exists and get user info
            string role = "";
            int userID = GetUserIDByEmail(txtEmail.Text, out role);

            if (userID == 0)
            {
                MessageBox.Show("Email not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // ✅ Block admin from using forgot password
            if (role == "Admin")
            {
                MessageBox.Show("Admin accounts cannot use forgot password.\nContact your system administrator.",
                    "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ✅ Staff — require admin password to authorize reset
            AdminPasswordDialog adminPrompt = new AdminPasswordDialog();
            if (adminPrompt.ShowDialog() != DialogResult.OK)
            {
                MessageBox.Show("Password reset cancelled. Admin authorization required.",
                    "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // ✅ Admin authorized — prompt for new password
            SetNewPasswordDialog setPassword = new SetNewPasswordDialog();
            if (setPassword.ShowDialog() == DialogResult.OK)
            {
                string newHash = PasswordHelper.HashPassword(setPassword.NewPassword);
                ResetStaffPassword(userID, newHash);
            }
        }

        private void ResetStaffPassword(int userID, string newHash)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    string query = "UPDATE Users SET PasswordHash = @PasswordHash WHERE UserID = @UserID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PasswordHash", newHash);
                    cmd.Parameters.AddWithValue("@UserID", userID);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();

                    MessageBox.Show("Password reset successfully! The staff member can now log in with the new password.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error resetting password: " + ex.Message);
            }
        }

        // ✅ GET USER ID BY EMAIL
        private int GetUserIDByEmail(string email, out string role)
        {
            role = "";
            using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
            {
                string query = "SELECT UserID, Role FROM Users WHERE Email = @Email AND IsActive = 1";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    role = reader["Role"].ToString();
                    int id = Convert.ToInt32(reader["UserID"]);
                    conn.Close();
                    return id;
                }

                conn.Close();
                return 0;
            }
        }

        // ✅ GENERATE RESET TOKEN
        private string GenerateResetToken()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 20);
        }

        // ✅ STORE RESET TOKEN
        private bool StoreResetToken(int userID, string token)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
            {
                string query = @"
                    INSERT INTO PasswordResetTokens (UserID, Token, ExpiryDate)
                    VALUES (@UserID, @Token, @ExpiryDate)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.Parameters.AddWithValue("@Token", token);
                cmd.Parameters.AddWithValue("@ExpiryDate", DateTime.Now.AddHours(1)); // Token valid for 1 hour

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}