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
        private bool isLoadingPatientID = false;

        public AppointmentForm()
        {
            InitializeComponent();
        }

        private void AppointmentForm_Load(object sender, EventArgs e)
        {
            LoadPatientNames();
            LoadServices();

            // Wire events BEFORE EnsureValidDate so date corrections work
            cmbStartSlot.SelectedIndexChanged += cmbStartSlot_SelectedIndexChanged;
            dtpAppointmentDate.ValueChanged += dtpAppointmentDate_ValueChanged;

            EnsureValidDate();
            LoadAvailableTimes();
        }

        private void EnsureValidDate()
        {
            DateTime date = dtpAppointmentDate.Value;
            while (date.DayOfWeek == DayOfWeek.Wednesday || date.DayOfWeek == DayOfWeek.Sunday)
                date = date.AddDays(1);

            if (date != dtpAppointmentDate.Value)
            {
                dtpAppointmentDate.ValueChanged -= dtpAppointmentDate_ValueChanged;
                dtpAppointmentDate.Value = date;
                dtpAppointmentDate.ValueChanged += dtpAppointmentDate_ValueChanged;
            }
        }

        private void dtpAppointmentDate_ValueChanged(object sender, EventArgs e)
        {
            if (dtpAppointmentDate.Value.DayOfWeek == DayOfWeek.Wednesday ||
                dtpAppointmentDate.Value.DayOfWeek == DayOfWeek.Sunday)
            {
                MessageBox.Show("Clinic is closed on Wednesday and Sunday.",
                    "Closed", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                dtpAppointmentDate.ValueChanged -= dtpAppointmentDate_ValueChanged;
                dtpAppointmentDate.Value = DateTime.Today;
                dtpAppointmentDate.ValueChanged += dtpAppointmentDate_ValueChanged;
                return;
            }

            LoadAvailableTimes();
        }

        // ── PATIENTS ────────────────────────────────────────────────────────────

        private void LoadPatientNames()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    string query = "SELECT DISTINCT FullName FROM Patients WHERE IsArchived = 0 ORDER BY FullName ASC";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    cmbPatientName.Items.Clear();
                    while (reader.Read())
                        cmbPatientName.Items.Add(reader["FullName"].ToString());
                }

                cmbPatientName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cmbPatientName.AutoCompleteSource = AutoCompleteSource.ListItems;
                cmbPatientName.DropDownStyle = ComboBoxStyle.DropDown;

                // ← ADD THESE: catch both selection and typed autocomplete
                cmbPatientName.SelectedIndexChanged += cmbPatientName_SelectedIndexChanged;
                cmbPatientName.Leave += cmbPatientName_Leave;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading patients:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void cmbPatientName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbPatientName.Text)) return;
            LoadPatientIDs(cmbPatientName.Text);
        }

        private void LoadPatientIDs(string patientName)
        {
            isLoadingPatientID = true;

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    string query = @"SELECT PatientID, FullName FROM Patients 
                                     WHERE FullName = @FullName AND IsArchived = 0 
                                     ORDER BY PatientID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@FullName", patientName);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<PatientIDItem> list = new List<PatientIDItem>();
                    while (reader.Read())
                    {
                        list.Add(new PatientIDItem
                        {
                            PatientID = Convert.ToInt32(reader["PatientID"]),
                            PatientName = reader["FullName"].ToString()
                        });
                    }

                    if (list.Count == 1)
                    {
                        SelectedPatientID = list[0].PatientID;
                        cmbPatientID.Visible = false;
                        lblPatientID.Visible = true;
                        lblPatientID.Text = list[0].PatientID.ToString();
                    }
                    else if (list.Count > 1)
                    {
                        cmbPatientID.Items.Clear();
                        foreach (var item in list)
                            cmbPatientID.Items.Add(item);

                        cmbPatientID.Visible = true;
                        lblPatientID.Visible = false;
                        cmbPatientID.SelectedIndex = 0;
                        SelectedPatientID = list[0].PatientID;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading patient IDs:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                isLoadingPatientID = false;
            }
        }

        private void cmbPatientID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoadingPatientID) return;
            if (cmbPatientID.SelectedItem is PatientIDItem p)
                SelectedPatientID = p.PatientID;
        }

        // ── TIME SLOTS ──────────────────────────────────────────────────────────

        private void LoadAvailableTimes()
        {
            DateTime date = dtpAppointmentDate.Value.Date;
            List<TimeRange> booked = new List<TimeRange>();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    string query = @"SELECT AppointmentTime, AppointmentEndTime 
                                     FROM Appointments 
                                     WHERE AppointmentDate = @date AND Status != 'Cancelled'";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@date", date);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        booked.Add(new TimeRange
                        {
                            Start = reader["AppointmentTime"] is TimeSpan ts1 ? ts1 : TimeSpan.Parse(reader["AppointmentTime"].ToString()),
                            End = reader["AppointmentEndTime"] is TimeSpan ts2 ? ts2 : TimeSpan.Parse(reader["AppointmentEndTime"].ToString())
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading time slots:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            cmbStartSlot.Items.Clear();

            for (int minutes = 10 * 60; minutes < 17 * 60; minutes += 30)
            {
                TimeSpan slotStart = TimeSpan.FromMinutes(minutes);
                TimeSpan slotEnd = slotStart.Add(TimeSpan.FromMinutes(30));

                bool isBooked = booked.Exists(b => slotStart < b.End && slotEnd > b.Start);

                if (!isBooked)
                    cmbStartSlot.Items.Add(new TimeSlotItem(slotStart));
            }

            if (cmbStartSlot.Items.Count > 0)
                cmbStartSlot.SelectedIndex = 0;
            else
                MessageBox.Show("No available time slots for this date.", "Fully Booked",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

            UpdateEndSlots();
        }

        private void cmbStartSlot_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateEndSlots();
        }

        private void UpdateEndSlots()
        {
            cmbEndSlot.Items.Clear();
            if (cmbStartSlot.SelectedItem == null) return;

            TimeSpan start = ((TimeSlotItem)cmbStartSlot.SelectedItem).Value;

            for (int minutes = (int)start.TotalMinutes + 30; minutes <= 17 * 60; minutes += 30)
                cmbEndSlot.Items.Add(new TimeSlotItem(TimeSpan.FromMinutes(minutes)));

            if (cmbEndSlot.Items.Count > 0)
                cmbEndSlot.SelectedIndex = 0;
        }

        private TimeSpan GetStartTime() => ((TimeSlotItem)cmbStartSlot.SelectedItem).Value;
        private TimeSpan GetEndTime() => ((TimeSlotItem)cmbEndSlot.SelectedItem).Value;

        // ── SERVICES ────────────────────────────────────────────────────────────

        private void LoadServices()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    string query = "SELECT ServiceID, ServiceName FROM DentalServices ORDER BY ServiceName";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    cmbService.Items.Clear();
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

                if (cmbService.Items.Count > 0)
                    cmbService.SelectedIndex = 0;
                else
                    MessageBox.Show(
                        "No dental services found.\n\nPlease add services first.",
                        "No Services", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading services:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── SAVE ────────────────────────────────────────────────────────────────

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (SelectedPatientID <= 0)
            {
                MessageBox.Show("Please select a patient.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbService.SelectedItem == null)
            {
                MessageBox.Show("Please select a service.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateFields()) return;

            TimeSpan start = GetStartTime();
            TimeSpan end = GetEndTime();

            if (IsOverlapping(start, end))
            {
                MessageBox.Show("That time slot is already taken. Please choose another.",
                    "Time Conflict", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ServiceItem selectedService = (ServiceItem)cmbService.SelectedItem;

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    string query = @"
                        INSERT INTO Appointments 
                            (PatientID, PatientName, AppointmentDate, AppointmentTime,
                             AppointmentEndTime, EndTime, ServiceID, ServiceType, Status, Notes)
                        VALUES
                            (@PID, @Name, @Date, @Start, @End, @End, @SID, @SType, 'Scheduled', @Notes)";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PID", SelectedPatientID);
                    cmd.Parameters.AddWithValue("@Name", cmbPatientName.Text);
                    cmd.Parameters.AddWithValue("@Date", dtpAppointmentDate.Value.Date);
                    cmd.Parameters.AddWithValue("@Start", start);
                    cmd.Parameters.AddWithValue("@End", end);
                    cmd.Parameters.AddWithValue("@SID", selectedService.ServiceID);
                    cmd.Parameters.AddWithValue("@SType", selectedService.ServiceName);
                    cmd.Parameters.AddWithValue("@Notes", txtNotes.Text ?? "");

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Appointment scheduled successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving appointment:\n\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsOverlapping(TimeSpan newStart, TimeSpan newEnd)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    string query = @"SELECT COUNT(*) FROM Appointments
                                     WHERE AppointmentDate = @date
                                     AND Status != 'Cancelled'
                                     AND @newStart < AppointmentEndTime
                                     AND @newEnd > AppointmentTime";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@date", dtpAppointmentDate.Value.Date);
                    cmd.Parameters.AddWithValue("@newStart", newStart);
                    cmd.Parameters.AddWithValue("@newEnd", newEnd);

                    conn.Open();
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(cmbPatientName.Text))
            {
                MessageBox.Show("Please select a patient.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (dtpAppointmentDate.Value.DayOfWeek == DayOfWeek.Wednesday ||
                dtpAppointmentDate.Value.DayOfWeek == DayOfWeek.Sunday)
            {
                MessageBox.Show("Clinic is closed on Wednesday and Sunday.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (dtpAppointmentDate.Value.Date < DateTime.Today)
            {
                MessageBox.Show("Cannot schedule appointments in the past.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cmbStartSlot.SelectedItem == null)
            {
                MessageBox.Show("Please select a start time.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cmbEndSlot.SelectedItem == null)
            {
                MessageBox.Show("Please select an end time.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (GetEndTime() <= GetStartTime())
            {
                MessageBox.Show("End time must be after start time.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        // ── NAVIGATION ──────────────────────────────────────────────────────────

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

        private void cmbPatientName_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbPatientName.Text)) return;
            LoadPatientIDs(cmbPatientName.Text);
        }
    }

    // ── HELPER CLASSES ───────────────────────────────────────────────────────────

    public class TimeRange
    {
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
    }

    public class TimeSlotItem
    {
        public TimeSpan Value { get; }
        public TimeSlotItem(TimeSpan value) { Value = value; }
        public override string ToString() => DateTime.Today.Add(Value).ToString("hh:mm tt");
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