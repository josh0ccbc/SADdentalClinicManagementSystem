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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 frm = new Form1();
            frm.Show();
            this.Hide();
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button1.BackgroundImage = Properties.Resources.HoverArrowBack;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.BackgroundImage = Properties.Resources.ArrowBack;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

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
            Form1 frm = new Form1();
            frm.Show();
            this.Hide();
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
            string connectionString = @"Data Source=DESKTOP-ASL74A6;Initial Catalog=DentalClinicDB;Integrated Security=True";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO Patients 
                        (FullName, Gender, BirthDate, Age, ContactNumber, Address, MedicalHistory)
                        VALUES
                        (@FullName, @Gender, @BirthDate, @Age, @ContactNumber, @Address, @MedicalHistory)";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@FullName", txtFullName.Text);
                cmd.Parameters.AddWithValue("@Gender", cmbGender.Text);
                cmd.Parameters.AddWithValue("@BirthDate", dtpBirthDate.Value);
                cmd.Parameters.AddWithValue("@Age", txtAge.Text);
                cmd.Parameters.AddWithValue("@ContactNumber", txtContact.Text);
                cmd.Parameters.AddWithValue("@Address", txtAddress.Text);
                cmd.Parameters.AddWithValue("@MedicalHistory", txtMedicalHistory.Text);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

                MessageBox.Show("Patient Registered Successfully!");

                Form1 frm = new Form1();
                frm.Show();
                this.Hide();
            }
        }

        private void Form2_MouseEnter(object sender, EventArgs e)
        {

        }

        private void BTNsaveRec_MouseLeave(object sender, EventArgs e)
        {
            BTNsaveRec.BackgroundImage = Properties.Resources.Button;
        }
    }
}
