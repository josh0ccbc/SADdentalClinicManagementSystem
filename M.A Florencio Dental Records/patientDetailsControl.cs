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
    public partial class patientDetailsControl : UserControl
    {
        public int PatientID { get; set; }
        public bool IsArchived { get; set; } = false;

        private bool isPersonalExpanded = true;
        private bool isMedicalExpanded = true;
        private bool isMedicalRecordsExpanded = false;
        private int SelectedRecordID = 0;

        public patientDetailsControl()
        {
            InitializeComponent();
        }

        private void patientDetailsControl_Load(object sender, EventArgs e)
        {
            this.AutoScroll = false;
            this.HorizontalScroll.Maximum = 0;
            this.HorizontalScroll.Enabled = false;
            this.HorizontalScroll.Visible = false;
            this.AutoScroll = true;
        }

        // ─── Safe decrypt helper ─────────────────────────────────────────
        private string SafeDecrypt(object dbValue)
        {
            if (dbValue == DBNull.Value || dbValue == null) return "";
            string value = dbValue.ToString();
            if (string.IsNullOrEmpty(value)) return "";
            try { return CryptoHelper.Decrypt(value); }
            catch { return value; } // fallback for old unencrypted data
        }

        private string GetCurrentUserRole()
        {
            return LoginForm.CurrentUserRole ?? "";
        }

        // ─── Load everything ─────────────────────────────────────────────
        public void LoadPatientDetails(int patientID)
        {
            PatientID = patientID;
            LoadPersonalInformation();
            LoadMedicalHistory();
            LoadMedicalRecords();

            panelMedical.Visible = false;
            btnToggleMedical.Text = "Medical Information ▲";
            isMedicalExpanded = false;

            panelMedicalRecords.Visible = false;
            btnToggleMedicalRecords.Text = "Medical Records ▲";
            isMedicalRecordsExpanded = false;

            RepositionPanels();
        }

        // ─── Personal information ─────────────────────────────────────────
        private void LoadPersonalInformation()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
            {
                string query = "SELECT * FROM Patients WHERE PatientID = @PatientID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientID", PatientID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    lblPatientID.Text = reader["PatientID"].ToString();
                    lblFullName.Text = reader["FullName"].ToString();
                    lblGender.Text = SafeDecrypt(reader["Gender"]);
                    lblAge.Text = reader["Age"].ToString();
                    lblContactNumber.Text = SafeDecrypt(reader["ContactNumber"]);
                    lblAddress.Text = SafeDecrypt(reader["Address"]);
                    lblCivilStatus.Text = SafeDecrypt(reader["CivilStatus"]);
                    lblReligion.Text = SafeDecrypt(reader["Religion"]);

                    // Dates
                    if (reader["BirthDate"] != DBNull.Value)
                        lblBirthDate.Text = Convert.ToDateTime(reader["BirthDate"]).ToString("MMM dd, yyyy");
                    else
                        lblBirthDate.Text = "N/A";

                    if (reader["DateRegistered"] != DBNull.Value)
                        lblDateRegistered.Text = Convert.ToDateTime(reader["DateRegistered"]).ToString("MMM dd, yyyy");
                    else
                        lblDateRegistered.Text = "N/A";

                    // Guardian — only show panel if guardian exists
                    string guardianName = SafeDecrypt(reader["GuardianName"]);
                    if (!string.IsNullOrWhiteSpace(guardianName))
                    {
                        panelGuardian.Visible = true;
                        lblGuardianName.Text = guardianName;
                        lblGuardianContact.Text = SafeDecrypt(reader["GuardianContact"]);
                        lblGuardianRelationship.Text = SafeDecrypt(reader["GuardianRelationship"]);
                    }
                    else
                    {
                        panelGuardian.Visible = false;
                    }
                }
                else
                {
                    MessageBox.Show("Patient not found!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        // ─── Medical history ──────────────────────────────────────────────
        private void LoadMedicalHistory()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
            {
                string query = "SELECT * FROM PatientMedicalHistory WHERE PatientID = @PatientID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientID", PatientID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.Read())
                {
                    lblGeneralHealth.Text = "No medical history recorded";
                    lblAllergies.Text = "None";
                    lblTakingMeds.Text = "No";
                    lblMedicationList.Text = "None";
                    lblSubstances.Text = "None";
                    lblMedicalConditions.Text = "None";
                    lblBloodType.Text = "Not recorded";
                    panelWomenInfo.Visible = false;
                    return;
                }

                // General health
                var generalHealth = new List<string>();
                if (reader["is_healthy"] != DBNull.Value && Convert.ToBoolean(reader["is_healthy"]))
                    generalHealth.Add("✓ Good Health\n");

                if (reader["under_treatment"] != DBNull.Value && Convert.ToBoolean(reader["under_treatment"]))
                {
                    generalHealth.Add("✓ Under Medical Treatment");
                    string details = SafeDecrypt(reader["treatment_details"]);
                    if (!string.IsNullOrEmpty(details))
                        generalHealth.Add("   Details: " + details + "\n");
                }

                if (reader["serious_illness"] != DBNull.Value && Convert.ToBoolean(reader["serious_illness"]))
                {
                    generalHealth.Add("✓ Serious Illness");
                    string details = SafeDecrypt(reader["illness_details"]);
                    if (!string.IsNullOrEmpty(details))
                        generalHealth.Add("   Details: " + details + "\n");
                }

                if (reader["recently_hospitalized"] != DBNull.Value && Convert.ToBoolean(reader["recently_hospitalized"]))
                {
                    generalHealth.Add("✓ Recently Hospitalized");
                    string details = SafeDecrypt(reader["hospitalization_details"]);
                    if (!string.IsNullOrEmpty(details))
                        generalHealth.Add("   Details: " + details + "\n");
                }

                lblGeneralHealth.Text = generalHealth.Count > 0
                    ? string.Join("\n", generalHealth)
                    : "None recorded";

                // Allergies
                var allergies = new List<string>();
                if (reader["LocalAestheticAllergy"] != DBNull.Value && Convert.ToBoolean(reader["LocalAestheticAllergy"])) allergies.Add("Local Anesthetic");
                if (reader["PenicillinAllergy"] != DBNull.Value && Convert.ToBoolean(reader["PenicillinAllergy"])) allergies.Add("Penicillin");
                if (reader["SulfaAllergy"] != DBNull.Value && Convert.ToBoolean(reader["SulfaAllergy"])) allergies.Add("Sulfa");
                if (reader["AspirinAllergy"] != DBNull.Value && Convert.ToBoolean(reader["AspirinAllergy"])) allergies.Add("Aspirin");
                if (reader["LatexAllergy"] != DBNull.Value && Convert.ToBoolean(reader["LatexAllergy"])) allergies.Add("Latex");

                string otherAllergies = SafeDecrypt(reader["OtherAllergies"]);
                if (!string.IsNullOrEmpty(otherAllergies)) allergies.Add(otherAllergies);

                lblAllergies.Text = allergies.Count > 0 ? string.Join(", ", allergies) : "None";

                // Medications
                bool takingMeds = reader["TakingPrescriptionMeds"] != DBNull.Value && Convert.ToBoolean(reader["TakingPrescriptionMeds"]);
                lblTakingMeds.Text = takingMeds ? "Yes" : "No";

                string medList = SafeDecrypt(reader["MedicationList"]);
                if (!string.IsNullOrEmpty(medList))
                {
                    string[] meds = medList.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    lblMedicationList.Text = string.Join(", ", meds.Select(m => m.Trim()));
                }
                else
                {
                    lblMedicationList.Text = "None";
                }

                // Substances
                var substances = new List<string>();
                if (reader["UsesTobacco"] != DBNull.Value && Convert.ToBoolean(reader["UsesTobacco"])) substances.Add("Tobacco");
                if (reader["UsesAlcoholDrugs"] != DBNull.Value && Convert.ToBoolean(reader["UsesAlcoholDrugs"])) substances.Add("Alcohol/Drugs");
                lblSubstances.Text = substances.Count > 0 ? string.Join(", ", substances) : "None";

                // Medical conditions
                var conditions = new List<string>();
                if (reader["HighBP"] != DBNull.Value && Convert.ToBoolean(reader["HighBP"])) conditions.Add("High Blood Pressure");
                if (reader["LowBP"] != DBNull.Value && Convert.ToBoolean(reader["LowBP"])) conditions.Add("Low Blood Pressure");
                if (reader["HeartDisease"] != DBNull.Value && Convert.ToBoolean(reader["HeartDisease"])) conditions.Add("Heart Disease");
                if (reader["HeartMurmur"] != DBNull.Value && Convert.ToBoolean(reader["HeartMurmur"])) conditions.Add("Heart Murmur");
                if (reader["Diabetes"] != DBNull.Value && Convert.ToBoolean(reader["Diabetes"])) conditions.Add("Diabetes");
                if (reader["Thyroid"] != DBNull.Value && Convert.ToBoolean(reader["Thyroid"])) conditions.Add("Thyroid Problem");
                if (reader["Asthma"] != DBNull.Value && Convert.ToBoolean(reader["Asthma"])) conditions.Add("Asthma");
                if (reader["RespiratoryProblems"] != DBNull.Value && Convert.ToBoolean(reader["RespiratoryProblems"])) conditions.Add("Respiratory Problems");
                if (reader["Arthritis"] != DBNull.Value && Convert.ToBoolean(reader["Arthritis"])) conditions.Add("Arthritis");
                if (reader["KidneyDisease"] != DBNull.Value && Convert.ToBoolean(reader["KidneyDisease"])) conditions.Add("Kidney Disease");
                lblMedicalConditions.Text = conditions.Count > 0 ? string.Join(", ", conditions) : "None";

                // Blood type
                string bloodType = SafeDecrypt(reader["BloodType"]);
                lblBloodType.Text = string.IsNullOrEmpty(bloodType) ? "Not recorded" : bloodType;

                // ✅ FIXED: decrypt gender before comparing
                string gender = SafeDecrypt(GetRawPatientGender());
                if (gender.Trim().ToLower() == "female")
                {
                    panelWomenInfo.Visible = true;
                    lblPregnant.Text = (reader["IsPregnant"] != DBNull.Value && Convert.ToBoolean(reader["IsPregnant"])) ? "Yes" : "No";
                    lblNursing.Text = (reader["IsNursing"] != DBNull.Value && Convert.ToBoolean(reader["IsNursing"])) ? "Yes" : "No";
                    lblBirthControl.Text = (reader["OnBirthControl"] != DBNull.Value && Convert.ToBoolean(reader["OnBirthControl"])) ? "Yes" : "No";
                }
                else
                {
                    panelWomenInfo.Visible = false;
                }
            }
        }

        // ─── Get raw (encrypted) gender from DB ──────────────────────────
        // Returns the raw value so SafeDecrypt can handle it properly
        private object GetRawPatientGender()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
            {
                string query = "SELECT Gender FROM Patients WHERE PatientID = @PatientID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientID", PatientID);
                conn.Open();
                object result = cmd.ExecuteScalar();
                return result ?? DBNull.Value;
            }
        }

        // ─── Medical records list ─────────────────────────────────────────
        private void LoadMedicalRecords()
        {
            panelMedicalRecords.Controls.Clear();
            panelMedicalRecords.AutoScroll = true;

            Panel headerRow = new Panel();
            headerRow.Size = new Size(panelMedicalRecords.Width - 20, 30);
            headerRow.Location = new Point(10, 5);
            headerRow.BackColor = Color.FromArgb(0, 150, 136);

            string[] headers = { "Date & Time", "Diagnosis", "Procedure", "Action"};
            int[] xPositions = { 72, 246, 429, 732};

            foreach (var (text, x) in headers.Zip(xPositions, (t, p) => (t, p)))
            {
                Label lbl = new Label();
                lbl.Text = text;
                lbl.Font = new Font("Arial Rounded MT Bold", 9f, FontStyle.Regular);
                lbl.ForeColor = Color.White;
                lbl.AutoSize = false;
                lbl.Width = 170;
                lbl.Height = 30;
                lbl.TextAlign = ContentAlignment.MiddleLeft;
                lbl.Location = new Point(x, 0);
                headerRow.Controls.Add(lbl);
            }

            panelMedicalRecords.Controls.Add(headerRow);

            // ✅ Separator line
            Panel separator = new Panel();
            separator.Size = new Size(panelMedicalRecords.Width - 20, 1);
            separator.Location = new Point(10, 36);
            separator.BackColor = Color.LightGray;
            panelMedicalRecords.Controls.Add(separator);

            // ✅ Single yPosition declaration here — NOT inside the using block
            int yPosition = 45;


            using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
            {
                string query = @"
            SELECT mr.record_id, mr.visit_date, mr.diagnosis, mr.[procedure],
                   mr.notes, a.AppointmentTime
            FROM MedicalRecords mr
            LEFT JOIN Appointments a 
                ON mr.appointment_id = a.AppointmentID
                OR (mr.appointment_id IS NULL 
                    AND a.PatientID = mr.patient_id 
                    AND CAST(a.AppointmentDate AS DATE) = CAST(mr.visit_date AS DATE))
            WHERE mr.patient_id = @PatientID
            ORDER BY mr.visit_date DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientID", PatientID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    Label noRecords = new Label();
                    noRecords.Text = "No medical records found.";
                    noRecords.Font = new Font("Segoe UI", 10);
                    noRecords.ForeColor = Color.Gray;
                    noRecords.AutoSize = true;
                    noRecords.Location = new Point(10, 10);
                    panelMedicalRecords.Controls.Add(noRecords);
                    return;
                }

                while (reader.Read())
                {
                    int recordID = Convert.ToInt32(reader["record_id"]);
                    DateTime visitDate = Convert.ToDateTime(reader["visit_date"]);
                    string diagnosis = SafeDecrypt(reader["diagnosis"]);
                    string procedure = SafeDecrypt(reader["procedure"]);
                    string notes = SafeDecrypt(reader["notes"]);

                    string appointmentTime = "";
                    if (reader["AppointmentTime"] != DBNull.Value)
                    {
                        TimeSpan time = (TimeSpan)reader["AppointmentTime"];
                        appointmentTime = DateTime.Today.Add(time).ToString("hh:mm tt");
                    }

                    // ✅ Create card
                    MedicalRecordCard card = new MedicalRecordCard();
                    card.SetRecord(recordID, visitDate, appointmentTime, diagnosis, procedure, notes);
                    card.Width = panelMedicalRecords.Width - 20;
                    card.Location = new Point(10, yPosition);

                    // ✅ Wire up view button
                    card.OnViewClicked += (s, id) => ViewRecordDetails(id);
                    card.OnEditClicked += (s, id) => EditRecordDetails(id);

                    panelMedicalRecords.Controls.Add(card);
                    yPosition += card.Height + 8;
                }
            }
        }

        private void EditRecordDetails(int recordID)
        {
            // ✅ Staff requires admin authorization
            if (GetCurrentUserRole() == "Staff")
            {
                using (AdminPasswordDialog adminPrompt = new AdminPasswordDialog())
                {
                    if (adminPrompt.ShowDialog() != DialogResult.OK)
                    {
                        MessageBox.Show("Edit cancelled. Admin authorization required.",
                            "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
            }

            using (EditMedicalRecordForm editForm = new EditMedicalRecordForm(recordID))
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadMedicalRecords(); // ✅ Refresh the list
                    MessageBox.Show("Medical record updated successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // ─── Record detail popup ──────────────────────────────────────────
        private void ViewRecordDetails(int recordID)
        {
            SelectedRecordID = recordID;

            using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
            {
                conn.Open();

                string query = @"
                    SELECT mr.*, a.ServiceType, a.AppointmentDate, a.AppointmentTime
                    FROM MedicalRecords mr
                    LEFT JOIN Appointments a ON mr.appointment_id = a.AppointmentID
                    WHERE mr.record_id = @RecordID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@RecordID", recordID);

                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.Read())
                {
                    MessageBox.Show("Record not found.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int lineWidth = 45;
                string divider = new string('═', lineWidth);
                string thin = new string('─', lineWidth);

                string serviceType = reader["ServiceType"]?.ToString() ?? "";
                string diagnosis = SafeDecrypt(reader["diagnosis"]);
                string procedure = SafeDecrypt(reader["procedure"]);
                string notes = SafeDecrypt(reader["notes"]);
                DateTime visitDate = Convert.ToDateTime(reader["visit_date"]);

                string appointmentTime = "";
                if (reader["AppointmentTime"] != DBNull.Value)
                {
                    TimeSpan time = (TimeSpan)reader["AppointmentTime"];
                    appointmentTime = DateTime.Today.Add(time).ToString("hh:mm tt");
                }

                reader.Close();

                string details = "";
                details += divider + "\n";
                details += "        MEDICAL RECORD DETAILS\n";
                details += divider + "\n";
                details += "Visit Date : " + visitDate.ToString("MMM dd, yyyy") + "\n";
                if (!string.IsNullOrEmpty(appointmentTime))
                    details += "Time       : " + appointmentTime + "\n";
                details += thin + "\n";
                details += "DIAGNOSIS:\n";
                details += WrapText(string.IsNullOrEmpty(diagnosis) ? "N/A" : diagnosis, lineWidth) + "\n\n";
                details += "PROCEDURE:\n";
                details += WrapText(string.IsNullOrEmpty(procedure) ? "N/A" : procedure, lineWidth) + "\n\n";
                details += "NOTES:\n";
                details += WrapText(string.IsNullOrEmpty(notes) ? "None" : notes, lineWidth) + "\n";
                details += thin + "\n";

                // Prescriptions
                string prescQuery = @"
                    SELECT medication, prescription_date, med_instructions
                    FROM Prescription
                    WHERE record_id = @RecordID
                    ORDER BY prescription_date DESC";

                SqlCommand prescCmd = new SqlCommand(prescQuery, conn);
                prescCmd.Parameters.AddWithValue("@RecordID", recordID);
                SqlDataReader prescReader = prescCmd.ExecuteReader();

                if (prescReader.HasRows)
                {
                    details += "PRESCRIPTIONS:\n\n";
                    int count = 1;
                    while (prescReader.Read())
                    {
                        string medication = SafeDecrypt(prescReader["medication"]);
                        string instructions = SafeDecrypt(prescReader["med_instructions"]);
                        DateTime prescDate = Convert.ToDateTime(prescReader["prescription_date"]);

                        details += $"  [{count}] {(string.IsNullOrEmpty(medication) ? "N/A" : medication)}\n";
                        details += $"      Date: {prescDate:MMM dd, yyyy}\n";
                        details += $"      Instructions:\n";
                        details += $"      {WrapText(string.IsNullOrEmpty(instructions) ? "None" : instructions, lineWidth - 6)}\n\n";
                        count++;
                    }
                }
                else
                {
                    details += "PRESCRIPTIONS: None\n";
                }

                details += divider;

                MessageBox.Show(details, "Medical Record Details",
                    MessageBoxButtons.OK, MessageBoxIcon.None);
            }
        }

        // ─── Layout ───────────────────────────────────────────────────────
        private int _startY = -1;

        private void RepositionPanels()
        {
            if (_startY == -1)
                _startY = btnTogglePersonal.Top;

            int controlWidth = this.Width - 20;

            btnTogglePersonal.Width = controlWidth;
            panelPersonal.Width = controlWidth;
            btnToggleMedical.Width = controlWidth;
            panelMedical.Width = controlWidth;
            btnToggleMedicalRecords.Width = controlWidth;
            panelMedicalRecords.Width = controlWidth;

            int y = _startY + btnTogglePersonal.Height + 5;

            if (isPersonalExpanded)
            {
                panelPersonal.Location = new Point(panelPersonal.Left, y);
                panelPersonal.Visible = true;
                y = panelPersonal.Bottom + 5;
            }
            else
            {
                panelPersonal.Visible = false;
            }

            btnToggleMedical.Location = new Point(btnToggleMedical.Left, y);
            y = btnToggleMedical.Bottom + 5;

            if (isMedicalExpanded)
            {
                panelMedical.Location = new Point(panelMedical.Left, y);
                panelMedical.Visible = true;
                y = panelMedical.Bottom + 5;
            }
            else
            {
                panelMedical.Visible = false;
            }

            btnToggleMedicalRecords.Location = new Point(btnToggleMedicalRecords.Left, y);
            y = btnToggleMedicalRecords.Bottom + 5;

            if (isMedicalRecordsExpanded)
            {
                panelMedicalRecords.Location = new Point(panelMedicalRecords.Left, y);
                panelMedicalRecords.Visible = true;
                y = panelMedicalRecords.Bottom + 5;
            }
            else
            {
                panelMedicalRecords.Visible = false;
            }

            this.Height = y + 20;

            this.AutoScroll = false;
            this.HorizontalScroll.Maximum = 0;
            this.HorizontalScroll.Enabled = false;
            this.HorizontalScroll.Visible = false;
            this.AutoScroll = true;
        }

        // ─── Wrap text helper ─────────────────────────────────────────────
        private string WrapText(string text, int maxWidth)
        {
            if (string.IsNullOrEmpty(text)) return "N/A";

            var words = text.Split(' ');
            var lines = new List<string>();
            string current = "";

            foreach (var word in words)
            {
                if ((current + " " + word).Trim().Length > maxWidth)
                {
                    if (!string.IsNullOrEmpty(current)) lines.Add(current);
                    current = word;
                }
                else
                {
                    current = (current + " " + word).Trim();
                }
            }

            if (!string.IsNullOrEmpty(current)) lines.Add(current);
            return string.Join("\n", lines);
        }

        // ─── Delete patient ───────────────────────────────────────────────
        private void PermanentlyDeletePatient()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    conn.Open();

                    // 1. Prescriptions linked to this patient's records
                    string sql1 = @"DELETE FROM Prescription WHERE record_id IN
                                    (SELECT record_id FROM MedicalRecords WHERE patient_id = @PID)";
                    SqlCommand cmd1 = new SqlCommand(sql1, conn);
                    cmd1.Parameters.AddWithValue("@PID", PatientID);
                    cmd1.ExecuteNonQuery();

                    // 2. Medical records
                    SqlCommand cmd2 = new SqlCommand("DELETE FROM MedicalRecords WHERE patient_id = @PID", conn);
                    cmd2.Parameters.AddWithValue("@PID", PatientID);
                    cmd2.ExecuteNonQuery();

                    // 3. Medical history
                    SqlCommand cmd3 = new SqlCommand("DELETE FROM PatientMedicalHistory WHERE PatientID = @PID", conn);
                    cmd3.Parameters.AddWithValue("@PID", PatientID);
                    cmd3.ExecuteNonQuery();

                    // 4. Appointments
                    SqlCommand cmd4 = new SqlCommand("DELETE FROM Appointments WHERE PatientID = @PID", conn);
                    cmd4.Parameters.AddWithValue("@PID", PatientID);
                    cmd4.ExecuteNonQuery();

                    // 5. Patient
                    SqlCommand cmd5 = new SqlCommand("DELETE FROM Patients WHERE PatientID = @PID", conn);
                    cmd5.Parameters.AddWithValue("@PID", PatientID);
                    cmd5.ExecuteNonQuery();
                }

                MessageBox.Show("Patient permanently deleted.", "Deleted",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                Form1 mainForm = Application.OpenForms.OfType<Form1>().FirstOrDefault();
                mainForm?.LoadControl(new patientControl());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting patient:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─── Button events ────────────────────────────────────────────────
        private void btnBack_Click(object sender, EventArgs e)
        {
            Form1 mainForm = Application.OpenForms.OfType<Form1>().FirstOrDefault();
            if (mainForm == null) return;

            if (IsArchived)
                mainForm.LoadControl(new Archive());
            else
                mainForm.LoadControl(new patientControl());
        }

        private void btnTogglePersonal_Click(object sender, EventArgs e)
        {
            isPersonalExpanded = !isPersonalExpanded;
            btnTogglePersonal.Text = isPersonalExpanded ? "Personal Information ▼" : "Personal Information ▲";
            RepositionPanels();
        }

        private void btnToggleMedical_Click(object sender, EventArgs e)
        {
            isMedicalExpanded = !isMedicalExpanded;
            btnToggleMedical.Text = isMedicalExpanded ? "Medical Information ▼" : "Medical Information ▲";
            RepositionPanels();
        }

        private void btnToggleMedicalRecords_Click(object sender, EventArgs e)
        {
            isMedicalRecordsExpanded = !isMedicalRecordsExpanded;
            btnToggleMedicalRecords.Text = isMedicalRecordsExpanded ? "Medical Records ▼" : "Medical Records ▲";
            RepositionPanels();
        }

        private void btnToggleMedicalRecords_Click_1(object sender, EventArgs e)
        {
            btnToggleMedicalRecords_Click(sender, e);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (GetCurrentUserRole() == "Staff")
            {
                using (AdminPasswordDialog adminPrompt = new AdminPasswordDialog())
                {
                    if (adminPrompt.ShowDialog() != DialogResult.OK)
                    {
                        MessageBox.Show("Edit cancelled. Admin authorization required.",
                            "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
            }

            using (EditPatientForm editForm = new EditPatientForm(PatientID))
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadPatientDetails(PatientID);
                    MessageBox.Show("Patient updated successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                "Are you sure you want to PERMANENTLY DELETE this patient?\n\nThis cannot be undone!",
                "Permanent Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            if (MessageBox.Show(
                "All records and appointments will be deleted.\n\nAre you absolutely sure?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            if (GetCurrentUserRole() == "Staff")
            {
                using (AdminPasswordDialog adminPrompt = new AdminPasswordDialog())
                {
                    if (adminPrompt.ShowDialog() != DialogResult.OK)
                    {
                        MessageBox.Show("Delete cancelled. Admin authorization required.",
                            "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
            }

            PermanentlyDeletePatient();
        }

        private void ArchivePatient()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    string query = "UPDATE Patients SET IsArchived = 1 WHERE PatientID = @PatientID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PatientID", PatientID);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Patient archived successfully!", "Archived",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                btnBack_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error archiving patient:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddPrescription_Click_1(object sender, EventArgs e)
        {
            using (AddPrescriptionDialog dialog = new AddPrescriptionDialog(PatientID))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    LoadMedicalRecords();
                    LoadPatientDetails(PatientID);
                    MessageBox.Show("Prescription added successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            this.Refresh();
            this.Invalidate();
        }

        private void panelMedicalRecords_Paint(object sender, PaintEventArgs e) { }
    }

    public static class UserSession
    {
        public static string CurrentUsername { get; set; }
        public static string CurrentRole { get; set; }
    }
}