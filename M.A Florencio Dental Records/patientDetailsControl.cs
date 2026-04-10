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
        private int SelectedRecordID = 0;  // ✅ TRACK SELECTED RECORD


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

        private string SafeDecrypt(object dbValue)
        {
            if (dbValue == DBNull.Value || dbValue == null)
                return "";

            string value = dbValue.ToString();

            if (string.IsNullOrEmpty(value))
                return "";

            try
            {
                return CryptoHelper.Decrypt(value);
            }
            catch
            {
                // If decryption fails, return the value as-is (for old unencrypted data)
                return value;
            }
        }

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

            RepositionPanels(); // ✅ initial layout
        }

        // ✅ PERSONAL INFORMATION WITH DECRYPTION
        private void LoadPersonalInformation()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionSettings.Current.GetConnectionString()))
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

                    DateTime birthDate = Convert.ToDateTime(reader["BirthDate"]);
                    lblBirthDate.Text = birthDate.ToString("MMM dd, yyyy");
                    lblAge.Text = reader["Age"].ToString();

                    // ===== DECRYPT SENSITIVE FIELDS =====
                    lblContactNumber.Text = SafeDecrypt(reader["ContactNumber"]);
                    lblAddress.Text = SafeDecrypt(reader["Address"]);
                    lblCivilStatus.Text = SafeDecrypt(reader["CivilStatus"]);
                    lblReligion.Text = SafeDecrypt(reader["Religion"]);

                    DateTime dateReg = Convert.ToDateTime(reader["DateRegistered"]);
                    lblDateRegistered.Text = dateReg.ToString("MMM dd, yyyy");

                    // ===== DECRYPT GUARDIAN FIELDS =====
                    string guardianName = SafeDecrypt(reader["GuardianName"]);
                    if (!string.IsNullOrEmpty(guardianName))
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
                    MessageBox.Show("Patient not found!");
                }

                conn.Close();
            }
        }

        private int _startY = -1;

        private void RepositionPanels()
        {
            // ✅ Save the original Y position of btnTogglePersonal ONCE
            if (_startY == -1)
                _startY = btnTogglePersonal.Top;

            int controlWidth = this.Width - 20;

            btnTogglePersonal.Width = controlWidth;
            panelPersonal.Width = controlWidth;
            btnToggleMedical.Width = controlWidth;
            panelMedical.Width = controlWidth;
            btnToggleMedicalRecords.Width = controlWidth;
            panelMedicalRecords.Width = controlWidth;

            // ✅ Always start from the fixed Y position
            int yPosition = _startY + btnTogglePersonal.Height + 5;

            if (isPersonalExpanded)
            {
                panelPersonal.Location = new Point(panelPersonal.Left, yPosition);
                panelPersonal.Visible = true;
                yPosition = panelPersonal.Bottom + 5;
            }
            else
            {
                panelPersonal.Visible = false;
            }

            btnToggleMedical.Location = new Point(btnToggleMedical.Left, yPosition);
            yPosition = btnToggleMedical.Bottom + 5;

            if (isMedicalExpanded)
            {
                panelMedical.Location = new Point(panelMedical.Left, yPosition);
                panelMedical.Visible = true;
                yPosition = panelMedical.Bottom + 5;
            }
            else
            {
                panelMedical.Visible = false;
            }

            btnToggleMedicalRecords.Location = new Point(btnToggleMedicalRecords.Left, yPosition);
            yPosition = btnToggleMedicalRecords.Bottom + 5;

            if (isMedicalRecordsExpanded)
            {
                panelMedicalRecords.Location = new Point(panelMedicalRecords.Left, yPosition);
                panelMedicalRecords.Visible = true;
                yPosition = panelMedicalRecords.Bottom + 5;
            }
            else
            {
                panelMedicalRecords.Visible = false;
            }

            this.Height = yPosition + 20;

            this.AutoScroll = false;
            this.HorizontalScroll.Maximum = 0;
            this.HorizontalScroll.Enabled = false;
            this.HorizontalScroll.Visible = false;
            this.AutoScroll = false;
        }

        // ✅ MEDICAL HISTORY WITH DECRYPTION
        private void LoadMedicalHistory()
        {
            using(SqlConnection conn = new SqlConnection(ConnectionSettings.Current.GetConnectionString()))
            {
                string query = "SELECT * FROM PatientMedicalHistory WHERE PatientID = @PatientID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientID", PatientID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    List<string> generalHealth = new List<string>();

                    if (reader["is_healthy"] != DBNull.Value && Convert.ToBoolean(reader["is_healthy"]))
                    {
                        generalHealth.Add("✓ Good Health \n");
                    }

                    if (reader["under_treatment"] != DBNull.Value && Convert.ToBoolean(reader["under_treatment"]))
                    {
                        generalHealth.Add("✓ Under Medical Treatment");
                        // ===== DECRYPT =====
                        string treatmentDetails = SafeDecrypt(reader["treatment_details"]);
                        if (!string.IsNullOrEmpty(treatmentDetails))
                            generalHealth.Add("   Details: " + treatmentDetails + "\n");
                    }

                    if (reader["serious_illness"] != DBNull.Value && Convert.ToBoolean(reader["serious_illness"]))
                    {
                        generalHealth.Add("✓ Serious Illness");
                        // ===== DECRYPT =====
                        string illnessDetails = SafeDecrypt(reader["illness_details"]);
                        if (!string.IsNullOrEmpty(illnessDetails))
                            generalHealth.Add("   Details: " + illnessDetails + "\n");
                    }

                    if (reader["recently_hospitalized"] != DBNull.Value && Convert.ToBoolean(reader["recently_hospitalized"]))
                    {
                        generalHealth.Add("✓ Recently Hospitalized");
                        // ===== DECRYPT =====
                        string hospitalizationDetails = SafeDecrypt(reader["hospitalization_details"]);
                        if (!string.IsNullOrEmpty(hospitalizationDetails))
                            generalHealth.Add("   Details: " + hospitalizationDetails + "\n");
                    }

                    lblGeneralHealth.Text = generalHealth.Count > 0 ? string.Join("\n", generalHealth) : "None recorded";

                    List<string> allergies = new List<string>();
                    if (Convert.ToBoolean(reader["LocalAestheticAllergy"] ?? false)) allergies.Add("Local Anesthetic");
                    if (Convert.ToBoolean(reader["PenicillinAllergy"] ?? false)) allergies.Add("Penicillin");
                    if (Convert.ToBoolean(reader["SulfaAllergy"] ?? false)) allergies.Add("Sulfa");
                    if (Convert.ToBoolean(reader["AspirinAllergy"] ?? false)) allergies.Add("Aspirin");
                    if (Convert.ToBoolean(reader["LatexAllergy"] ?? false)) allergies.Add("Latex");

                    // ===== DECRYPT OTHER ALLERGIES =====
                    string otherAllergies = SafeDecrypt(reader["OtherAllergies"]);
                    if (!string.IsNullOrEmpty(otherAllergies)) allergies.Add(otherAllergies);

                    lblAllergies.Text = allergies.Count > 0 ? string.Join(", ", allergies) : "None";

                    lblTakingMeds.Text = Convert.ToBoolean(reader["TakingPrescriptionMeds"] ?? false) ? "Yes" : "No";

                    // ===== DECRYPT MEDICATION LIST =====
                    string medicationList = SafeDecrypt(reader["MedicationList"]);
                    if (!string.IsNullOrEmpty(medicationList))
                    {
                        string[] medications = medicationList.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                        lblMedicationList.Text = string.Join(", ", medications.Select(m => m.Trim()));
                    }
                    else
                    {
                        lblMedicationList.Text = "None";
                    }

                    List<string> substances = new List<string>();
                    if (Convert.ToBoolean(reader["UsesTobacco"] ?? false)) substances.Add("Tobacco");
                    if (Convert.ToBoolean(reader["UsesAlcoholDrugs"] ?? false)) substances.Add("Alcohol/Drugs");

                    lblSubstances.Text = substances.Count > 0 ? string.Join(", ", substances) : "None";

                    List<string> conditions = new List<string>();
                    if (Convert.ToBoolean(reader["HighBP"] ?? false)) conditions.Add("High Blood Pressure");
                    if (Convert.ToBoolean(reader["LowBP"] ?? false)) conditions.Add("Low Blood Pressure");
                    if (Convert.ToBoolean(reader["HeartDisease"] ?? false)) conditions.Add("Heart Disease");
                    if (Convert.ToBoolean(reader["HeartMurmur"] ?? false)) conditions.Add("Heart Murmur");
                    if (Convert.ToBoolean(reader["Diabetes"] ?? false)) conditions.Add("Diabetes");
                    if (Convert.ToBoolean(reader["Thyroid"] ?? false)) conditions.Add("Thyroid Problem");
                    if (Convert.ToBoolean(reader["Asthma"] ?? false)) conditions.Add("Asthma");
                    if (Convert.ToBoolean(reader["RespiratoryProblems"] ?? false)) conditions.Add("Respiratory Problems");
                    if (Convert.ToBoolean(reader["Arthritis"] ?? false)) conditions.Add("Arthritis");
                    if (Convert.ToBoolean(reader["KidneyDisease"] ?? false)) conditions.Add("Kidney Disease");

                    lblMedicalConditions.Text = conditions.Count > 0 ? string.Join(", ", conditions) : "None";

                    // ===== DECRYPT BLOOD TYPE =====
                    lblBloodType.Text = SafeDecrypt(reader["BloodType"]) ?? "Not recorded";

                    if (GetPatientGender() == "Female")
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
                else
                {
                    lblAllergies.Text = "No medical history recorded";
                }

                conn.Close();
            }
        }

        // ✅ MEDICAL RECORDS WITH DECRYPTION AND PRESCRIPTION SUPPORT
        private void LoadMedicalRecords()
        {
            panelMedicalRecords.Controls.Clear();

            using (SqlConnection conn = new SqlConnection(ConnectionSettings.Current.GetConnectionString()))
            {
                string query = @"
                    SELECT mr.record_id, mr.visit_date, mr.diagnosis, mr.[procedure], mr.notes, mr.appointment_id,
                           a.AppointmentDate, a.ServiceType
                    FROM MedicalRecords mr
                    LEFT JOIN Appointments a ON mr.appointment_id = a.AppointmentID
                    WHERE mr.patient_id = @PatientID
                    ORDER BY mr.visit_date DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientID", PatientID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                int yPosition = 10;

                if (!reader.HasRows)
                {
                    Label noRecords = new Label();
                    noRecords.Text = "No medical records";
                    noRecords.AutoSize = true;
                    noRecords.ForeColor = Color.Gray;
                    noRecords.Location = new Point(10, yPosition);
                    panelMedicalRecords.Controls.Add(noRecords);
                }
                else
                {
                    while (reader.Read())
                    {
                        int recordID = Convert.ToInt32(reader["record_id"]);
                        DateTime visitDate = Convert.ToDateTime(reader["visit_date"]);
                        // ===== DECRYPT DIAGNOSIS AND PROCEDURE =====
                        string diagnosis = SafeDecrypt(reader["diagnosis"]);
                        string procedure = SafeDecrypt(reader["procedure"]);
                        string serviceType = reader["ServiceType"]?.ToString() ?? "N/A";

                        Label lblRecord = new Label();
                        lblRecord.Text = visitDate.ToString("MMM dd, yyyy") + " | " +
                            (string.IsNullOrEmpty(serviceType) ? "Walk-in" : serviceType) + " | " +
                            "Diagnosis: " + (string.IsNullOrEmpty(diagnosis) ? "N/A" : diagnosis) + " | " +
                            "Procedure: " + (string.IsNullOrEmpty(procedure) ? "N/A" : procedure);

                        lblRecord.AutoSize = false;                          // ✅ disable autosize
                        lblRecord.Width = panelMedicalRecords.Width - 20;    // ✅ fit panel width
                        lblRecord.Height = 40;                               // ✅ allow two lines
                        lblRecord.AutoEllipsis = true;                       // ✅ cut off with ...
                        lblRecord.Tag = recordID;
                        lblRecord.Cursor = Cursors.Hand;
                        lblRecord.Location = new Point(10, yPosition);
                        lblRecord.ForeColor = Color.Blue;
                        lblRecord.Click += (s, e) => ViewRecordDetails(recordID);
                        lblRecord.MouseEnter += (s, e) => lblRecord.ForeColor = Color.DarkBlue;
                        lblRecord.MouseLeave += (s, e) => lblRecord.ForeColor = Color.Blue;

                        panelMedicalRecords.Controls.Add(lblRecord);
                        yPosition += 50;  // ✅ increase spacing to match taller label
                    }
                }

                conn.Close();
            }
        }

        // ✅ VIEW RECORD DETAILS WITH PRESCRIPTIONS AND DECRYPTION
        private void ViewRecordDetails(int recordID)
        {
            SelectedRecordID = recordID;

            using (SqlConnection conn = new SqlConnection(ConnectionSettings.Current.GetConnectionString()))
            {
                string query = @"
                SELECT mr.*, a.ServiceType, a.AppointmentDate
                FROM MedicalRecords mr
                LEFT JOIN Appointments a ON mr.appointment_id = a.AppointmentID
                WHERE mr.record_id = @RecordID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@RecordID", recordID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    int lineWidth = 45;
                    string divider = new string('═', lineWidth);
                    string thinDivider = new string('─', lineWidth);

                    string serviceType = reader["ServiceType"]?.ToString();
                    // ===== DECRYPT DIAGNOSIS =====
                    string diagnosis = SafeDecrypt(reader["diagnosis"]);
                    // ===== DECRYPT PROCEDURE =====
                    string procedure = SafeDecrypt(reader["procedure"]);
                    // ===== DECRYPT NOTES =====
                    string notes = SafeDecrypt(reader["notes"]);

                    string details = "";
                    details += divider + "\n";
                    details += "        MEDICAL RECORD DETAILS\n";
                    details += divider + "\n\n";

                    details += "Visit Date : " + Convert.ToDateTime(reader["visit_date"]).ToString("MMM dd, yyyy") + "\n";
                    details += "Service    : " + (string.IsNullOrEmpty(serviceType) ? "Walk-in" : serviceType) + "\n\n";

                    details += thinDivider + "\n";
                    details += "DIAGNOSIS:\n";
                    details += WrapText(string.IsNullOrEmpty(diagnosis) ? "N/A" : diagnosis, lineWidth) + "\n\n";

                    details += "PROCEDURE:\n";
                    details += WrapText(string.IsNullOrEmpty(procedure) ? "N/A" : procedure, lineWidth) + "\n\n";

                    details += "NOTES:\n";
                    details += WrapText(string.IsNullOrEmpty(notes) ? "None" : notes, lineWidth) + "\n";
                    details += thinDivider + "\n";

                    reader.Close();

                    // ===== DECRYPT PRESCRIPTION DETAILS =====
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
                            // ===== DECRYPT MEDICATION AND INSTRUCTIONS =====
                            string medication = SafeDecrypt(prescReader["medication"]);
                            string instructions = SafeDecrypt(prescReader["med_instructions"]);

                            details += $"  [{count}] {(string.IsNullOrEmpty(medication) ? "N/A" : medication)}\n";
                            details += $"      Date: {Convert.ToDateTime(prescReader["prescription_date"]).ToString("MMM dd, yyyy")}\n";
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

                    MessageBox.Show(details, "Medical Record Details", MessageBoxButtons.OK, MessageBoxIcon.None);
                }

                conn.Close();
            }
        }

        private string WrapText(string text, int maxWidth)
        {
            if (string.IsNullOrEmpty(text)) return "N/A";

            var words = text.Split(' ');
            var lines = new List<string>();
            string currentLine = "";

            foreach (var word in words)
            {
                if ((currentLine + " " + word).Trim().Length > maxWidth)
                {
                    if (!string.IsNullOrEmpty(currentLine))
                        lines.Add(currentLine);
                    currentLine = word;
                }
                else
                {
                    currentLine = (currentLine + " " + word).Trim();
                }
            }

            if (!string.IsNullOrEmpty(currentLine))
                lines.Add(currentLine);

            return string.Join("\n", lines);
        }

        // ✅ HELPER METHODS
        private string GetPatientGender()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionSettings.Current.GetConnectionString()))
            {
                string query = "SELECT Gender FROM Patients WHERE PatientID = @PatientID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientID", PatientID);
                conn.Open();
                object result = cmd.ExecuteScalar();
                conn.Close();
                return result?.ToString() ?? "";
            }
        }

        // ✅ BUTTON EVENTS
        private void btnBack_Click(object sender, EventArgs e)
        {
            Form1 mainForm = (Form1)this.FindForm();
            if (IsArchived)
                mainForm.LoadControl(new Archive()); // whatever your archive list control is named
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

        private void btnEdit_Click(object sender, EventArgs e)
        {
            // ✅ OPEN EDIT FORM
            EditPatientForm editForm = new EditPatientForm(PatientID);

            if (editForm.ShowDialog() == DialogResult.OK)
            {
                // ✅ RELOAD PATIENT DATA
                LoadPatientDetails(PatientID);
                MessageBox.Show("Patient updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void PermanentlyDeletePatient()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionSettings.Current.GetConnectionString()))
                {
                    conn.Open();

                    // 1. Delete Prescriptions linked to this patient's medical records
                    string deletePrescriptions = @"
                DELETE FROM Prescription 
                WHERE record_id IN (
                    SELECT record_id FROM MedicalRecords WHERE patient_id = @PatientID
                )";
                    new SqlCommand(deletePrescriptions, conn) { Parameters = { new SqlParameter("@PatientID", PatientID) } }.ExecuteNonQuery();

                    // 2. Delete Medical Records
                    string deleteMedical = "DELETE FROM MedicalRecords WHERE patient_id = @PatientID";
                    new SqlCommand(deleteMedical, conn) { Parameters = { new SqlParameter("@PatientID", PatientID) } }.ExecuteNonQuery();

                    // 3. Delete Medical History
                    string deleteHistory = "DELETE FROM PatientMedicalHistory WHERE PatientID = @PatientID";
                    new SqlCommand(deleteHistory, conn) { Parameters = { new SqlParameter("@PatientID", PatientID) } }.ExecuteNonQuery();

                    // 4. Delete Appointments
                    string deleteAppointments = "DELETE FROM Appointments WHERE PatientID = @PatientID";
                    new SqlCommand(deleteAppointments, conn) { Parameters = { new SqlParameter("@PatientID", PatientID) } }.ExecuteNonQuery();

                    // 5. Finally, delete the Patient
                    string deletePatient = "DELETE FROM Patients WHERE PatientID = @PatientID";
                    new SqlCommand(deletePatient, conn) { Parameters = { new SqlParameter("@PatientID", PatientID) } }.ExecuteNonQuery();

                    conn.Close();
                    MessageBox.Show("Patient permanently deleted!");
                    this.Parent.Controls.Remove(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to PERMANENTLY DELETE this patient?\n\nThis action CANNOT be undone!",
                "Permanent Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                // Double confirmation for safety
                DialogResult confirm = MessageBox.Show(
                    "All patient data and appointments will be deleted.\n\nAre you absolutely sure?",
                    "Confirm Permanent Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirm == DialogResult.Yes)
                {
                    PermanentlyDeletePatient();
                }
            }
        }

        private void ArchivePatient()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionSettings.Current.GetConnectionString()))
                {
                    string query = "UPDATE Patients SET IsArchived = 1 WHERE PatientID = @PatientID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PatientID", PatientID);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Patient archived successfully!");
                    btnBack_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void panelMedicalRecords_Paint(object sender, PaintEventArgs e)
        {
        }

        private void btnAddPrescription_Click_1(object sender, EventArgs e)
        {
            AddPrescriptionDialog dialog = new AddPrescriptionDialog(PatientID);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                LoadMedicalRecords();
                LoadPatientDetails(PatientID);
                MessageBox.Show("Prescription added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // ✅ THIS IS ENOUGH - Force refresh
            this.Refresh();
            this.Invalidate();
        }

        private void btnToggleMedicalRecords_Click_1(object sender, EventArgs e)
        {
            isMedicalRecordsExpanded = !isMedicalRecordsExpanded;
            btnToggleMedicalRecords.Text = isMedicalRecordsExpanded ? "Medical Records ▼" : "Medical Records ▲";
            RepositionPanels();
        }
    }
}