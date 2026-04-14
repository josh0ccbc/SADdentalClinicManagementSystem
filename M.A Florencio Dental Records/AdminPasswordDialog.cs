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
            string enteredPassword = txtAdminPassword.Text;

            using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
            {
                string query = "SELECT PasswordHash FROM Users WHERE Role = 'Admin' AND IsActive = 1";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string hash = reader["PasswordHash"].ToString();

                    // Use your existing PasswordHelper to verify
                    if (PasswordHelper.VerifyPassword(enteredPassword, hash))
                    {
                        IsVerified = true;
                        break;
                    }
                }
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
