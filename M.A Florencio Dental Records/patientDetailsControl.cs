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
        string connectionString = @"Data Source=DESKTOP-ASL74A6;Initial Catalog=DentalClinicDB;Integrated Security=True";
        public int PatientID { get; set; }

        private bool isPersonalExpanded = true;
        private bool isMedicalExpanded = true;
        private bool isMedicalRecordsExpanded = false;  // ✅ ADD THIS

        public patientDetailsControl()
        {
            InitializeComponent();
        }

        private void patientDetailsControl_Load(object sender, EventArgs e)
        {
            this.AutoScroll = true;
        }

        public void LoadPatientDetails(int patientID)
        {
            PatientID = patientID;
            LoadPersonalInformation();
            LoadMedicalHistory();
            LoadMedicalRecords();  // ✅ ADD THIS

            panelMedical.Visible = false;
            btnToggleMedical.Text = "Medical Information ▲";
            isMedicalExpanded = false;

            panelMedicalRecords.Visible = false;  // ✅ ADD THIS
            btnToggleMedicalRecords.Text = "Medical Records ▲";
            isMedicalRecordsExpanded = false;
        }

        // ✅ PERSONAL INFORMATION (EXISTING - NO CHANGES)
        private void LoadPersonalInformation()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
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
                    lblGender.Text = reader["Gender"].ToString();

                    DateTime birthDate = Convert.ToDateTime(reader["BirthDate"]);
                    lblBirthDate.Text = birthDate.ToString("MMM dd, yyyy");
                    lblAge.Text = reader["Age"].ToString();

                    lblContactNumber.Text = reader["ContactNumber"].ToString();
                    lblAddress.Text = reader["Address"].ToString();
                    lblCivilStatus.Text = reader["CivilStatus"].ToString();
                    lblReligion.Text = reader["Religion"].ToString();

                    DateTime dateReg = Convert.ToDateTime(reader["DateRegistered"]);
                    lblDateRegistered.Text = dateReg.ToString("MMM dd, yyyy");

                    string guardianName = reader["GuardianName"].ToString();
                    if (!string.IsNullOrEmpty(guardianName))
                    {
                        panelGuardian.Visible = true;
                        lblGuardianName.Text = guardianName;
                        lblGuardianContact.Text = reader["GuardianContact"].ToString();
                        lblGuardianRelationship.Text = reader["GuardianRelationship"].ToString();
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

        // ✅ MEDICAL HISTORY (EXISTING - NO CHANGES)
        private void LoadMedicalHistory()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM PatientMedicalHistory WHERE PatientID = @PatientID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientID", PatientID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    List<string> generalHealth = new List<string>();

                    if (Convert.ToBoolean(reader["is_healthy"] ?? false))
                    {
                        generalHealth.Add("✓ Good Health \n");
                    }

                    if (Convert.ToBoolean(reader["under_treatment"] ?? false))
                    {
                        generalHealth.Add("✓ Under Medical Treatment");
                        string treatmentDetails = reader["treatment_details"].ToString();
                        if (!string.IsNullOrEmpty(treatmentDetails))
                            generalHealth.Add("   Details: " + treatmentDetails + "\n");
                    }

                    if (Convert.ToBoolean(reader["serious_illness"] ?? false))
                    {
                        generalHealth.Add("✓ Serious Illness");
                        string illnessDetails = reader["illness_details"].ToString();
                        if (!string.IsNullOrEmpty(illnessDetails))
                            generalHealth.Add("   Details: " + illnessDetails + "\n");
                    }

                    if (Convert.ToBoolean(reader["recently_hospitalized"] ?? false))
                    {
                        generalHealth.Add("✓ Recently Hospitalized");
                        string hospitalizationDetails = reader["hospitalization_details"].ToString();
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

                    string otherAllergies = reader["OtherAllergies"].ToString();
                    if (!string.IsNullOrEmpty(otherAllergies)) allergies.Add(otherAllergies);

                    lblAllergies.Text = allergies.Count > 0 ? string.Join(", ", allergies) : "None";

                    lblTakingMeds.Text = Convert.ToBoolean(reader["TakingPrescriptionMeds"] ?? false) ? "Yes" : "No";

                    string medicationList = reader["MedicationList"].ToString() ?? "";
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

                    lblBloodType.Text = reader["BloodType"].ToString() ?? "Not recorded";

                    if (GetPatientGender() == "Female")
                    {
                        panelWomenInfo.Visible = true;
                        lblPregnant.Text = Convert.ToBoolean(reader["IsPregnant"] ?? false) ? "Yes" : "No";
                        lblNursing.Text = Convert.ToBoolean(reader["IsNursing"] ?? false) ? "Yes" : "No";
                        lblBirthControl.Text = Convert.ToBoolean(reader["OnBirthControl"] ?? false) ? "Yes" : "No";
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

        // ✅ NEW - MEDICAL RECORDS (SAME PATTERN AS PERSONAL & MEDICAL)
        private void LoadMedicalRecords()
        {
            panelMedicalRecords.Controls.Clear();

            using (SqlConnection conn = new SqlConnection(connectionString))
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
                        string diagnosis = reader["diagnosis"]?.ToString() ?? "N/A";
                        string procedure = reader["procedure"]?.ToString() ?? "N/A";
                        string serviceType = reader["ServiceType"]?.ToString() ?? "N/A";

                        Label lblRecord = new Label();
                        lblRecord.Text = visitDate.ToString("MMM dd, yyyy") + " | " +
                                        serviceType + " | " +
                                        "Diagnosis: " + diagnosis + " | " +
                                        "Procedure: " + procedure;

                        lblRecord.AutoSize = true;
                        lblRecord.Tag = recordID;
                        lblRecord.Cursor = Cursors.Hand;
                        lblRecord.Location = new Point(10, yPosition);
                        lblRecord.ForeColor = Color.Blue;
                        lblRecord.Click += (s, e) => ViewRecordDetails(recordID);
                        lblRecord.MouseEnter += (s, e) => lblRecord.ForeColor = Color.DarkBlue;
                        lblRecord.MouseLeave += (s, e) => lblRecord.ForeColor = Color.Blue;

                        panelMedicalRecords.Controls.Add(lblRecord);
                        yPosition += 30;
                    }
                }

                conn.Close();
            }
        }

        // ✅ NEW - VIEW RECORD DETAILS
        private void ViewRecordDetails(int recordID)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM MedicalRecords WHERE record_id = @RecordID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@RecordID", recordID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string details = "";
                    details += "Visit Date: " + Convert.ToDateTime(reader["visit_date"]).ToString("MMM dd, yyyy") + "\n\n";
                    details += "Diagnosis:\n" + (reader["diagnosis"]?.ToString() ?? "N/A") + "\n\n";
                    details += "Procedure:\n" + (reader["procedure"]?.ToString() ?? "N/A") + "\n\n";
                    details += "Notes:\n" + (reader["notes"]?.ToString() ?? "None");

                    // Show prescriptions if any
                    List<string> prescriptions = GetPrescriptionsForRecord(recordID);
                    if (prescriptions.Count > 0)
                    {
                        details += "\n\n💊 Prescriptions:\n";
                        details += string.Join("\n\n", prescriptions);
                    }

                    MessageBox.Show(details, "Medical Record Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                conn.Close();
            }
        }

        // ✅ NEW - GET PRESCRIPTIONS FOR RECORD
        private List<string> GetPrescriptionsForRecord(int recordID)
        {
            List<string> prescriptions = new List<string>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT medication, prescription_date, med_instructions FROM Prescription WHERE record_id = @RecordID ORDER BY prescription_date DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@RecordID", recordID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string presc = reader["medication"]?.ToString() + "\n";
                    presc += "Date: " + Convert.ToDateTime(reader["prescription_date"]).ToString("MMM dd, yyyy") + "\n";
                    presc += "Instructions: " + (reader["med_instructions"]?.ToString() ?? "None");

                    prescriptions.Add(presc);
                }

                conn.Close();
            }

            return prescriptions;
        }

        // ✅ HELPER METHODS
        private string GetPatientGender()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
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
            mainForm.LoadControl(new patientControl());
        }

        private void btnTogglePersonal_Click(object sender, EventArgs e)
        {
            isPersonalExpanded = !isPersonalExpanded;
            panelPersonal.Visible = isPersonalExpanded;
            btnTogglePersonal.Text = isPersonalExpanded ? "Personal Information ▼" : "Personal Information ▲";
        }

        private void btnToggleMedical_Click(object sender, EventArgs e)
        {
            isMedicalExpanded = !isMedicalExpanded;
            panelMedical.Visible = isMedicalExpanded;
            btnToggleMedical.Text = isMedicalExpanded ? "Medical Information ▼" : "Medical Information ▲";
        }

        // ✅ NEW - TOGGLE MEDICAL RECORDS
        private void btnToggleMedicalRecords_Click(object sender, EventArgs e)
        {
            isMedicalRecordsExpanded = !isMedicalRecordsExpanded;
            panelMedicalRecords.Visible = isMedicalRecordsExpanded;
            btnToggleMedicalRecords.Text = isMedicalRecordsExpanded ? "Medical Records ▼" : "Medical Records ▲";
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Edit functionality coming soon!", "Edit Patient", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Archive this patient record?",
                "Archive Patient",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                ArchivePatient();
            }
        }

        private void ArchivePatient()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
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
    }
}