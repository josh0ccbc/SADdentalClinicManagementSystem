using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using MaterialSkin;
using MaterialSkin.Controls;


namespace M.A_Florencio_Dental_Records
{
    public partial class DBcontrol : UserControl
    {
        public DBcontrol()
        {
            InitializeComponent();

            int totalPatients = GetPatientCount();
            lblPatientCount.Text = "" + totalPatients;

            int todayAppointments = GetTodayAppointments();
            lblTodayAppointments.Text = todayAppointments.ToString();

            int upcoming = GetUpcomingAppointments();
            lblUpcomingAppointments.Text = upcoming.ToString();
        }

        private void panelContainer_Paint(object sender, PaintEventArgs e)
        {

        }

        private void DBcontrol_Load(object sender, EventArgs e)
        {

        }

        private void BTNAddPatient_Click(object sender, EventArgs e)
        {
            Form1 mainForm = (Form1)this.FindForm(); // get the main form
            Form2 frm = new Form2(mainForm);
            mainForm.Hide();
            frm.Show();
            this.Hide(); // hide DBcontrol panel temporarily
        }

        private void BTNAddPatient_MouseEnter(object sender, EventArgs e)
        {
            BTNAddPatient.BackgroundImage = Properties.Resources.HoverButton;
        }

        private void BTNAddPatient_MouseLeave(object sender, EventArgs e)
        {
            BTNAddPatient.BackgroundImage = Properties.Resources.Button;
        }

        public int GetPatientCount()
        {
            int count = 0;

            using (SqlConnection conn = new SqlConnection(ConnectionSettings.Current.GetConnectionString()))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Patients", conn);
                    count = (int)cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

            return count;
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
        public int GetTodayAppointments()
        {
            int count = 0;

            using (SqlConnection conn = new SqlConnection(ConnectionSettings.Current.GetConnectionString()))
            {
                string query = "SELECT COUNT(*) FROM Appointments WHERE AppointmentDate = @today";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@today", DateTime.Today);

                conn.Open();
                count = (int)cmd.ExecuteScalar();
            }

            return count;
        }

        public int GetUpcomingAppointments()
        {
            int count = 0;

            using (SqlConnection conn = new SqlConnection(ConnectionSettings.Current.GetConnectionString()))
            {
                string query = @"SELECT COUNT(*) 
                         FROM Appointments
                         WHERE AppointmentDate > CAST(GETDATE() AS DATE)";

                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                count = (int)cmd.ExecuteScalar();
            }

            return count;
        }

    }
}
