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
using MaterialSkin;
using MaterialSkin.Controls;

namespace M.A_Florencio_Dental_Records
{
    public partial class Form2 : MaterialForm
    {
        private Form1 mainForm;
        private bool returningToMain = false;

        public Form2(Form1 parent)
        {
            InitializeComponent();
            mainForm = parent;
        }

        public Form2()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);

            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;

          
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            returningToMain = true;

            mainForm.LoadControl(new DBcontrol());
            mainForm.Show();
            this.Close();
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button1.BackgroundImage = Properties.Resources.HoverArrowBack;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.BackgroundImage = Properties.Resources.ArrowBack;
        }

        private int CalculateAge(DateTime birthDate)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - birthDate.Year;

            if (birthDate.Date > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (dtpBirthDate.Value > DateTime.Today)
            {
                MessageBox.Show("Birth date cannot be in the future!");
                dtpBirthDate.Value = DateTime.Today.AddYears(-18);  // reset to 18 years ago
                return;
            }

            int age = CalculateAge(dtpBirthDate.Value);
            txtAge.Text = age.ToString();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            returningToMain = true;

            mainForm.LoadControl(new DBcontrol());
            mainForm.Show();
            this.Close();
        }

        private void button2_MouseHover(object sender, EventArgs e)
        {

        }

        private void button2_MouseEnter(object sender, EventArgs e)
        {
            button2.BackgroundImage = Properties.Resources.Button4;
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            button2.BackgroundImage = Properties.Resources.Button3;
        }

        private void BTNsaveRec_MouseEnter(object sender, EventArgs e)
        {
            BTNsaveRec.BackgroundImage = Properties.Resources.HoverButton;
        }

        private void BTNsaveRec_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            string connectionString = @"Data Source=DESKTOP-ASL74A6;Initial Catalog=DentalClinicDB;Integrated Security=True";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO Patients 
                    (FullName, Gender, BirthDate, Age, ContactNumber, Address, MedicalHistory)
                    VALUES
                    (@FullName, @Gender, @BirthDate, @Age, @ContactNumber, @Address, @MedicalHistory)";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@FullName", txtFullName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Gender", cmbGender.Text);
                    cmd.Parameters.AddWithValue("@BirthDate", dtpBirthDate.Value);
                    cmd.Parameters.AddWithValue("@Age", int.Parse(txtAge.Text));
                    cmd.Parameters.AddWithValue("@ContactNumber", txtContact.Text);
                    cmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim());
                    cmd.Parameters.AddWithValue("@MedicalHistory", txtMedicalHistory.Text.Trim());

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();

                    returningToMain = true;
                    MessageBox.Show("Patient Registered Successfully!");
                    mainForm.LoadControl(new DBcontrol());
                    mainForm.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Form2_MouseEnter(object sender, EventArgs e)
        {

        }

        private void BTNsaveRec_MouseLeave(object sender, EventArgs e)
        {
            BTNsaveRec.BackgroundImage = Properties.Resources.Button;
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!returningToMain && e.CloseReason == CloseReason.UserClosing)
            {
                Application.Exit();
            }
        }

        private void cmbGender_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtFullName_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtAge_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtAge_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }

        private void txtContact_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtContact_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }
        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Full Name is required!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFullName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(cmbGender.Text))
            {
                MessageBox.Show("Gender is required!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbGender.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtContact.Text))
            {
                MessageBox.Show("Contact Number is required!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtContact.Focus();
                return false;
            }

            if (!long.TryParse(txtContact.Text, out long contact))
            {
                MessageBox.Show("Contact must be numbers only!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtContact.Focus();
                return false;
            }

            if (txtContact.Text.Length < 10 || txtContact.Text.Length > 11)
            {
                MessageBox.Show("Contact must be 10-11 digits!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtContact.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                MessageBox.Show("Address is required!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAddress.Focus();
                return false;
            }

            return true;
        }
    }
}
