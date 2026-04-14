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
        public string PatientName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string ServiceName { get; set; }
        public string Status { get; set; }

        // ✅ NEW - EVENT FOR MARK AS DONE
        public event Action<int> OnMarkAsDone;

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
            PatientName = patientName;
            AppointmentDate = appointmentDate;
            AppointmentTime = appointmentTime;
            ServiceName = serviceName;
            Status = status;

            lblPatientName.Text = patientName;
            lblService.Text = "Service: " + serviceName;
            lblDateTime.Text = appointmentDate.ToString("MMM dd, yyyy") + " at " + appointmentTime.ToString(@"hh\:mm");
            lblStatus.Text = "Status: " + status;
        }

        // ✅ NEW - MARK AS DONE BUTTON
        private void btnMarkAsDone_Click(object sender, EventArgs e)
        {
            if (Status.Equals("Done", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("This appointment is already completed");
                return;
            }

            if (Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("This appointment is cancelled");
                return;
            }

            // Trigger event to open CompleteAppointmentForm
            OnMarkAsDone?.Invoke(AppointmentID);
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
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    string query = "UPDATE Appointments SET Status = 'Cancelled' WHERE AppointmentID = @AppointmentID";
                    SqlCommand cmd = new SqlCommand(query, conn);
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

        private void btnDelete_MouseEnter_1(object sender, EventArgs e)
        {
            btnDelete.BackgroundImage = Properties.Resources.cancel2;
        }

        private void btnDelete_MouseLeave_1(object sender, EventArgs e)
        {
            btnDelete.BackgroundImage = Properties.Resources.cancel;
        }

        private void btnMarkAsDone_MouseEnter(object sender, EventArgs e)
        {
            btnMarkAsDone.BackgroundImage = Properties.Resources.checkmark2;
        }

        private void btnMarkAsDone_MouseLeave(object sender, EventArgs e)
        {
            btnMarkAsDone.BackgroundImage = Properties.Resources.checkmark;
        }
    }
}