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

            dtpAppointmentDate.ValueChanged += dtpAppointmentDate_ValueChanged;
        }

        private void dtpAppointmentDate_ValueChanged(object sender, EventArgs e)
        {
            if (dtpAppointmentDate.Value.DayOfWeek == DayOfWeek.Wednesday ||
                dtpAppointmentDate.Value.DayOfWeek == DayOfWeek.Sunday)
            {
                MessageBox.Show("Clinic is closed on Wednesday and Sunday.");
                dtpAppointmentDate.Value = DateTime.Today;
                return;
            }

            LoadAvailableTimes();
        }

        // ✅ LOAD PATIENT NAMES TO DROPDOWN
        private void LoadPatientNames()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionSettings.Current.GetConnectionString()))
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
            }

            cmbPatientName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmbPatientName.AutoCompleteSource = AutoCompleteSource.ListItems;
            cmbPatientName.DropDownStyle = ComboBoxStyle.DropDown;
        }


        private void cmbPatientName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbPatientName.Text))
                return;

            LoadPatientIDs(cmbPatientName.Text);
        }

        private void LoadPatientIDs(string patientName)
        {
            isLoadingPatientID = true;

            using (SqlConnection conn = new SqlConnection(ConnectionSettings.Current.GetConnectionString()))
            {
                string query = "SELECT PatientID, FullName FROM Patients WHERE FullName = @FullName AND IsArchived = 0 ORDER BY PatientID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FullName", patientName);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<PatientIDItem> patientIDs = new List<PatientIDItem>();

                while (reader.Read())
                {
                    patientIDs.Add(new PatientIDItem
                    {
                        PatientID = Convert.ToInt32(reader["PatientID"]),
                        PatientName = reader["FullName"].ToString()
                    });
                }

                if (patientIDs.Count == 1)
                {
                    SelectedPatientID = patientIDs[0].PatientID;
                    cmbPatientID.Visible = false;
                    lblPatientID.Visible = true;
                    lblPatientID.Text = patientIDs[0].PatientID.ToString();
                }
                else
                {
                    cmbPatientID.Items.Clear();
                    foreach (var item in patientIDs)
                        cmbPatientID.Items.Add(item);

                    cmbPatientID.Visible = true;
                    lblPatientID.Visible = false;
                    cmbPatientID.SelectedIndex = 0;
                }
            }

            isLoadingPatientID = false;
        }



        // ✅ PATIENT ID SELECTED FROM COMBOBOX
        private void cmbPatientID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoadingPatientID) return;

            if (cmbPatientID.SelectedItem is PatientIDItem selectedPatient)
                SelectedPatientID = selectedPatient.PatientID;
        }

        private void LoadTimeComboBoxes()
        {
            cmbStartHour.Items.Clear();
            cmbStartMinute.Items.Clear();
            cmbStartAMPM.Items.Clear();

            cmbEndHour.Items.Clear();
            cmbEndMinute.Items.Clear();
            cmbEndAMPM.Items.Clear();

            // 10AM–5PM
            for (int hour = 10; hour <= 17; hour++)
            {
                int display = hour > 12 ? hour - 12 : hour;
                string ampm = hour >= 12 ? "PM" : "AM";

                cmbStartHour.Items.Add(display.ToString("00"));
                cmbEndHour.Items.Add(display.ToString("00"));
            }

            // 🔥 CLEAN SLOTS: 00 & 30 only
            cmbStartMinute.Items.AddRange(new[] { "00", "30" });
            cmbEndMinute.Items.AddRange(new[] { "00", "30" });

            cmbStartAMPM.Items.AddRange(new[] { "AM", "PM" });
            cmbEndAMPM.Items.AddRange(new[] { "AM", "PM" });

            cmbStartHour.SelectedIndex = 0;
            cmbStartMinute.SelectedIndex = 0;
            cmbStartAMPM.SelectedIndex = 0;

            cmbEndHour.SelectedIndex = 0;
            cmbEndMinute.SelectedIndex = 0;
            cmbEndAMPM.SelectedIndex = 0;
        }

        private TimeSpan GetTime(ComboBox h, ComboBox m, ComboBox a)
        {
            int hour = int.Parse(h.Text);

            if (a.Text == "PM" && hour != 12) hour += 12;
            if (a.Text == "AM" && hour == 12) hour = 0;

            return new TimeSpan(hour, int.Parse(m.Text), 0);
        }

        public class TimeRange
        {
            public TimeSpan Start { get; set; }
            public TimeSpan End { get; set; }
        }

        private void LoadAvailableTimes()
        {
            DateTime date = dtpAppointmentDate.Value.Date;

            List<TimeRange> existing = new List<TimeRange>();

            using(SqlConnection conn = new SqlConnection(ConnectionSettings.Current.GetConnectionString()))
            {
                string query = @"SELECT AppointmentTime, AppointmentEndTime 
                                 FROM Appointments 
                                 WHERE AppointmentDate=@date AND Status!='Cancelled'";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@date", date);

                conn.Open();
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    existing.Add(new TimeRange
                    {
                        Start = (TimeSpan)reader["AppointmentTime"],
                        End = (TimeSpan)reader["AppointmentEndTime"]
                    });
                }
            }

            cmbStartHour.Items.Clear();

            for (int hour = 10; hour <= 17; hour++)
            {
                TimeSpan slotStart = new TimeSpan(hour, 0, 0);
                TimeSpan slotEnd = slotStart.Add(TimeSpan.FromMinutes(30));

                bool conflict = false;

                foreach (var appt in existing)
                {
                    if (slotStart < appt.End && slotEnd > appt.Start)
                    {
                        conflict = true;
                        break;
                    }
                }

                if (!conflict)
                {
                    int display = hour > 12 ? hour - 12 : hour;
                    cmbStartHour.Items.Add(display.ToString("00"));
                }
            }

            if (cmbStartHour.Items.Count > 0)
                cmbStartHour.SelectedIndex = 0;
        }

        private void LoadServices()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionSettings.Current.GetConnectionString()))
            {
                string query = "SELECT ServiceID, ServiceName FROM DentalServices";
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
            }

            cmbService.DisplayMember = "ServiceName";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateFields()) return;

            TimeSpan start = GetTime(cmbStartHour, cmbStartMinute, cmbStartAMPM);
            TimeSpan end = GetTime(cmbEndHour, cmbEndMinute, cmbEndAMPM);

            // 🔥 FINAL OVERLAP CHECK (IMPORTANT)
            if (IsOverlapping(start, end))
            {
                MessageBox.Show("Time slot already occupied!");
                return;
            }

            using (SqlConnection conn = new SqlConnection(ConnectionSettings.Current.GetConnectionString()))
            {
                string query = @"INSERT INTO Appointments 
                (PatientID, PatientName, AppointmentDate, AppointmentTime, AppointmentEndTime, ServiceID, ServiceType, Status, Notes)
                VALUES
                (@PID,@Name,@Date,@Start,@End,@SID,@SType,'Scheduled',@Notes)";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@PID", SelectedPatientID);
                cmd.Parameters.AddWithValue("@Name", cmbPatientName.Text);
                cmd.Parameters.AddWithValue("@Date", dtpAppointmentDate.Value.Date);
                cmd.Parameters.AddWithValue("@Start", start);
                cmd.Parameters.AddWithValue("@End", end);
                cmd.Parameters.AddWithValue("@SID", ((ServiceItem)cmbService.SelectedItem).ServiceID);
                cmd.Parameters.AddWithValue("@SType", ((ServiceItem)cmbService.SelectedItem).ServiceName);
                cmd.Parameters.AddWithValue("@Notes", txtNotes.Text ?? "");

                conn.Open();
                cmd.ExecuteNonQuery();

                MessageBox.Show("Appointment scheduled!");
                this.Close();
            }
        }

        private bool IsOverlapping(TimeSpan newStart, TimeSpan newEnd)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionSettings.Current.GetConnectionString()))
            {
                string query = @"SELECT COUNT(*) FROM Appointments
                                 WHERE AppointmentDate=@date
                                 AND Status!='Cancelled'
                                 AND @newStart < AppointmentEndTime
                                 AND @newEnd > AppointmentTime";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@date", dtpAppointmentDate.Value.Date);
                cmd.Parameters.AddWithValue("@newStart", newStart);
                cmd.Parameters.AddWithValue("@newEnd", newEnd);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();

                return count > 0;
            }
        }

        private bool ValidateFields()
        {
            if (dtpAppointmentDate.Value.DayOfWeek == DayOfWeek.Wednesday ||
                dtpAppointmentDate.Value.DayOfWeek == DayOfWeek.Sunday)
            {
                MessageBox.Show("Clinic closed.");
                return false;
            }

            TimeSpan start = GetTime(cmbStartHour, cmbStartMinute, cmbStartAMPM);
            TimeSpan end = GetTime(cmbEndHour, cmbEndMinute, cmbEndAMPM);

            if (start < new TimeSpan(10, 0, 0) || end > new TimeSpan(17, 0, 0))
            {
                MessageBox.Show("10AM–5PM only.");
                return false;
            }

            if (end <= start)
            {
                MessageBox.Show("Invalid time.");
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

    public class PatientIDItem
    {
        public int PatientID { get; set; }
        public string PatientName { get; set; }
        public override string ToString() => $"ID: {PatientID} - {PatientName}";
    }

    public class ServiceItem
    {
        public int ServiceID { get; set; }
        public string ServiceName { get; set; }
        public override string ToString() => ServiceName;
    }
}