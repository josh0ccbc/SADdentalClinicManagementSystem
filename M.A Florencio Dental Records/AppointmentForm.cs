using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace M.A_Florencio_Dental_Records
{
    public partial class AppointmentForm : MaterialForm
    {
        string connectionString = @"Data Source=DESKTOP-ASL74A6;Initial Catalog=DentalClinicDB;Integrated Security=True";
        public int SelectedPatientID { get; set; }
        private bool isLoadingPatientID = false;  // ✅ ADD THIS FLAG

        public AppointmentForm()
        {
            InitializeComponent();
        }

        private void AppointmentForm_Load(object sender, EventArgs e)
        {
            LoadPatientNames();
            LoadServices();
            LoadTimeComboBoxes();
        }

        // ✅ LOAD PATIENT NAMES TO DROPDOWN
        private void LoadPatientNames()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT DISTINCT FullName FROM Patients WHERE IsArchived = 0 ORDER BY FullName ASC";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                cmbPatientName.Items.Clear();

                while (reader.Read())
                {
                    cmbPatientName.Items.Add(reader["FullName"].ToString());
                }

                conn.Close();
            }

            cmbPatientName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmbPatientName.AutoCompleteSource = AutoCompleteSource.ListItems;
            cmbPatientName.DropDownStyle = ComboBoxStyle.DropDown; 
        }

        // ✅ PATIENT NAME SELECTED - LOAD PATIENT IDS
        private void cmbPatientName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbPatientName.Text))
                return;

            LoadPatientIDs(cmbPatientName.Text);
        }

        // ✅ LOAD PATIENT IDS FOR SELECTED NAME
        private void LoadPatientIDs(string patientName)
        {
            isLoadingPatientID = true;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT PatientID, FullName FROM Patients WHERE FullName = @FullName AND IsArchived = 0 ORDER BY PatientID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FullName", patientName);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    MessageBox.Show("No active patient found with this name");
                    isLoadingPatientID = false;
                    return;
                }

                List<PatientIDItem> patientIDs = new List<PatientIDItem>();

                while (reader.Read())
                {
                    patientIDs.Add(new PatientIDItem
                    {
                        PatientID = Convert.ToInt32(reader["PatientID"]),
                        PatientName = reader["FullName"].ToString()
                    });
                }

                conn.Close();

                // ✅ IF UNIQUE PATIENT - SHOW AS LABEL
                if (patientIDs.Count == 1)
                {
                    SelectedPatientID = patientIDs[0].PatientID;

                    // Hide ComboBox, show Label
                    cmbPatientID.Visible = false;
                    lblPatientID.Visible = true;
                    lblPatientID.Text = patientIDs[0].PatientID.ToString();
                }
                // ✅ IF MULTIPLE PATIENTS - SHOW AS COMBOBOX
                else
                {
                    cmbPatientID.Items.Clear();
                    cmbPatientID.DisplayMember = "DisplayText";
                    cmbPatientID.ValueMember = "PatientID";

                    foreach (var item in patientIDs)
                    {
                        cmbPatientID.Items.Add(item);
                    }

                    // Show ComboBox, hide Label
                    cmbPatientID.Visible = true;
                    lblPatientID.Visible = false;

                    // Auto-select first one
                    cmbPatientID.SelectedIndex = 0;
                }
            }

            isLoadingPatientID = false;
        }

        // ✅ PATIENT ID SELECTED FROM COMBOBOX
        private void cmbPatientID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoadingPatientID)
                return;

            if (cmbPatientID.SelectedItem is PatientIDItem selectedPatient)
            {
                SelectedPatientID = selectedPatient.PatientID;
            }
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

                    cmd.Parameters.AddWithValue("@PatientName", cmbPatientName.Text);
                    cmd.Parameters.AddWithValue("@AppointmentDate", dtpAppointmentDate.Value.Date);
                    cmd.Parameters.AddWithValue("@AppointmentTime", appointmentTime);
                    cmd.Parameters.AddWithValue("@ServiceID", ((ServiceItem)cmbService.SelectedItem).ServiceID);
                    cmd.Parameters.AddWithValue("@ServiceType", ((ServiceItem)cmbService.SelectedItem).ServiceName);
                    cmd.Parameters.AddWithValue("@Status", "Scheduled");
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
            if (string.IsNullOrWhiteSpace(cmbPatientName.Text))
            {
                MessageBox.Show("Select patient name!");
                return false;
            }

            if (SelectedPatientID == 0)
            {
                MessageBox.Show("Select patient ID!");
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
    }

    // ✅ PATIENT ID ITEM CLASS
    public class PatientIDItem
    {
        public int PatientID { get; set; }
        public string PatientName { get; set; }

        public string DisplayText => "ID: " + PatientID + " - " + PatientName;

        public override string ToString() => DisplayText;
    }

    public class ServiceItem
    {
        public int ServiceID { get; set; }
        public string ServiceName { get; set; }
        public override string ToString() => ServiceName;
    }
}