using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace M.A_Florencio_Dental_Records
{
    public partial class AdminPasswordDialog : Form
    {
        public bool IsVerified { get; private set; } = false;

        public AdminPasswordDialog()
        {
            InitializeComponent();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAdminPassword.Text))
            {
                MessageBox.Show("Please enter password.", "Input Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            IsVerified = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    string query = @"
                SELECT PasswordHash 
                FROM Users 
                WHERE Role = 'Admin' 
                  AND IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        string hash = cmd.ExecuteScalar()?.ToString();

                        if (!string.IsNullOrEmpty(hash) &&
                            PasswordHelper.VerifyPassword(txtAdminPassword.Text.Trim(), hash))
                        {
                            IsVerified = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error during verification.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            if (IsVerified)
                this.DialogResult = DialogResult.OK;
            else
                MessageBox.Show("Incorrect admin password!", "Access Denied",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void AdminPasswordDialog_Load(object sender, EventArgs e)
        {

        }
    }
}
