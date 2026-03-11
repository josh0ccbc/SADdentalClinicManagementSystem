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
    public partial class AddAppointmentForm : Form
    {
        string connectionString = @"Data Source=DESKTOP-ASL74A6;Initial Catalog=DentalClinicDB;Integrated Security=True";

        public AddAppointmentForm()
        {
            InitializeComponent();
        }

        private void AddAppointmentForm_Load(object sender, EventArgs e)
        {

        }

        private void BTNsave_Click(object sender, EventArgs e)
        {
            int hour = (int)numHour.Value;
            int minute = (int)numMinute.Value;

            if (cmbAMPM.Text == "PM" && hour != 12)
                hour += 12;

            if (cmbAMPM.Text == "AM" && hour == 12)
                hour = 0;

            TimeSpan appointmentTime = new TimeSpan(hour, minute, 0);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO Appointments
                        (PatientName, AppointmentDate, AppointmentTime, Notes)
                        VALUES (@name, @date, @time, @notes)";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@name", txtPatientName.Text);
                cmd.Parameters.AddWithValue("@date", dtpDate.Value.Date);
                cmd.Parameters.AddWithValue("@time", appointmentTime);
                cmd.Parameters.AddWithValue("@notes", txtNotes.Text);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Appointment Saved");

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
