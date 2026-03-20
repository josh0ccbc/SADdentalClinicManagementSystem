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
    public partial class AppointmentCard : UserControl
    {
        public int AppointmentID { get; set; }

        public AppointmentCard()
        {
            InitializeComponent();
            lblPatientName.Font = new Font("Segoe UI", 12, FontStyle.Bold);
        }

        private void AppointmentCard_Load(object sender, EventArgs e)
        {
        }

        public void SetAppointment(int appointmentID, string patientName, DateTime appointmentDate,
                           TimeSpan appointmentTime, string serviceName, string status, string notes)
        {
            AppointmentID = appointmentID;

            lblPatientName.Text = patientName;
            lblService.Text = "Service: " + serviceName;
            lblDateTime.Text = appointmentDate.ToString("MMM dd, yyyy") + " at " + appointmentTime.ToString(@"hh\:mm");
            lblStatus.Text = "Status: " + status;
            //lblNotes.Text = "Notes: " + (string.IsNullOrEmpty(notes) ? "N/A" : notes);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Edit appointment ID: " + AppointmentID);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Cancel this appointment?",
                "Cancel Appointment",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                CancelAppointment();
            }
        }

        private void CancelAppointment()
        {
            string connectionString = @"Data Source=DESKTOP-ASL74A6;Initial Catalog=DentalClinicDB;Integrated Security=True";

            try
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connectionString))
                {
                    string query = "UPDATE Appointments SET Status = 'Cancelled' WHERE AppointmentID = @AppointmentID";
                    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@AppointmentID", AppointmentID);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Appointment cancelled!");
                    this.Parent.Controls.Remove(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnEdit_MouseEnter(object sender, EventArgs e)
        {
            btnEdit.BackgroundImage = Properties.Resources.edit2;
        }

        private void btnEdit_MouseLeave(object sender, EventArgs e)
        {

        }

        private void btnDelete_MouseEnter(object sender, EventArgs e)
        {
            btnDelete.BackgroundImage = Properties.Resources.bin2;
        }

        private void btnDelete_MouseLeave(object sender, EventArgs e)
        {
            btnDelete.BackgroundImage = Properties.Resources.bin;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void lblNotes_Click(object sender, EventArgs e)
        {

        }
    }
}