using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace M.A_Florencio_Dental_Records
{
    public partial class ForgotPasswordForm : MaterialForm
    {
        string connectionString = @"Data Source=DESKTOP-ASL74A6;Initial Catalog=DentalClinicDB;Integrated Security=True";

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

            int userID = GetUserIDByEmail(txtEmail.Text);

            if (userID == 0)
            {
                MessageBox.Show("Email not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Generate reset token
            string resetToken = GenerateResetToken();

            // Store token in database
            if (StoreResetToken(userID, resetToken))
            {
                MessageBox.Show(
                    "Password reset token: " + resetToken + "\n\nUse this token to reset your password.\n(In production, this would be sent via email)",
                    "Reset Token",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // ✅ CLOSE THIS FORM (returns to LoginForm)
                this.Close();
            }
        }

        // ✅ GET USER ID BY EMAIL
        private int GetUserIDByEmail(string email)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT UserID FROM Users WHERE Email = @Email AND IsActive = 1";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);

                conn.Open();
                object result = cmd.ExecuteScalar();
                conn.Close();

                if (result != null)
                    return Convert.ToInt32(result);

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
            using (SqlConnection conn = new SqlConnection(connectionString))
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