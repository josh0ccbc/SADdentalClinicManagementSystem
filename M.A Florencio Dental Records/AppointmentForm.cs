using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using MaterialSkin;
using MaterialSkin.Controls;

namespace M.A_Florencio_Dental_Records
{
    public partial class AppointmentForm : MaterialForm
    {
        string connectionString = @"Data Source=DESKTOP-ASL74A6;Initial Catalog=DentalClinicDB;Integrated Security=True";


        public AppointmentForm()
        {
            InitializeComponent();
        }

        private void AppointmentForm_Load(object sender, EventArgs e)
        {
            LoadServices();
            LoadTimeComboBoxes();
        }

        private void LoadTimeComboBoxes()
        {
            // Hours (1-12)
            for (int i = 1; i <= 12; i++)
            {
                cmbHour.Items.Add(i.ToString("00"));
            }

            // Minutes (00-59)
            for (int i = 0; i <= 59; i++)
            {
                cmbMinute.Items.Add(i.ToString("00"));
            }

            // AM/PM
            cmbAMPM.Items.Add("AM");
            cmbAMPM.Items.Add("PM");

            // Set defaults
            cmbHour.SelectedIndex = 0;
            cmbMinute.SelectedIndex = 0;
            cmbAMPM.SelectedIndex = 0;
        }

        private void LoadServices()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT ServiceID, ServiceName FROM DentalServices ORDER BY ServiceName";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    cmbService.Items.Add(new ServiceItem
                    {
                        ServiceID = Convert.ToInt32(reader["ServiceID"]),
                        ServiceName = reader["ServiceName"].ToString()
                    });
                }
                conn.Close();
            }

            cmbService.DisplayMember = "ServiceName";
            cmbService.ValueMember = "ServiceID";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateFields())
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO Appointments 
                    (PatientName, AppointmentDate, AppointmentTime, ServiceID, ServiceType, Status, Notes)
                    VALUES
                    (@PatientName, @AppointmentDate, @AppointmentTime, @ServiceID, @ServiceType, @Status, @Notes)";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    int hour = int.Parse(cmbHour.Text);
                    if (cmbAMPM.Text == "PM" && hour != 12)
                        hour += 12;
                    if (cmbAMPM.Text == "AM" && hour == 12)
                        hour = 0;

                    TimeSpan appointmentTime = new TimeSpan(hour, int.Parse(cmbMinute.Text), 0);

                    cmd.Parameters.AddWithValue("@PatientName", txtPatientName.Text);
                    cmd.Parameters.AddWithValue("@AppointmentDate", dtpAppointmentDate.Value.Date);
                    cmd.Parameters.AddWithValue("@AppointmentTime", appointmentTime);
                    cmd.Parameters.AddWithValue("@ServiceID", ((ServiceItem)cmbService.SelectedItem).ServiceID);
                    cmd.Parameters.AddWithValue("@ServiceType", ((ServiceItem)cmbService.SelectedItem).ServiceName);
                    cmd.Parameters.AddWithValue("@Status", "Pending");
                    cmd.Parameters.AddWithValue("@Notes", txtNotes.Text ?? "");

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Appointment scheduled!");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(txtPatientName.Text))
            {
                MessageBox.Show("Enter patient name!");
                return false;
            }

            if (cmbService.SelectedIndex == -1)
            {
                MessageBox.Show("Select service!");
                return false;
            }

            if (string.IsNullOrEmpty(cmbHour.Text) || string.IsNullOrEmpty(cmbMinute.Text) || string.IsNullOrEmpty(cmbAMPM.Text))
            {
                MessageBox.Show("Select time!");
                return false;
            }

            return true;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnBack_MouseEnter(object sender, EventArgs e)
        {
            btnBack.BackgroundImage = Properties.Resources.HoverArrowBack;
        }

        private void btnBack_MouseLeave(object sender, EventArgs e)
        {
            btnBack.BackgroundImage = Properties.Resources.ArrowBack;
        }

        private void txtPatientName_TextChanged(object sender, EventArgs e)
        {

        }
    }

    public class ServiceItem
    {
        public int ServiceID { get; set; }
        public string ServiceName { get; set; }
        public override string ToString() => ServiceName;
    }
}