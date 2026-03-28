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

        public patientDetailsControl()
        {
            InitializeComponent();
        }

        private void patientDetailsControl_Load(object sender, EventArgs e)
        {
        }

        public void LoadPatientDetails(int patientID)
        {
            PatientID = patientID;
            LoadPersonalInformation();
            LoadMedicalHistory();
        }

        // ✅ PERSONAL INFORMATION
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
                    // Basic Info
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

                    // Guardian Info
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

        // ✅ MEDICAL HISTORY
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
                    // General Health
                    lblGoodHealth.Text = Convert.ToBoolean(reader["GoodHealth"]) ? "Yes" : "No";
                    lblUnderTreatment.Text = Convert.ToBoolean(reader["UnderMedicalTreatment"]) ? "Yes" : "No";
                    lblSeriousIllness.Text = Convert.ToBoolean(reader["SeriousIllnessOrSurgery"]) ? "Yes" : "No";
                    lblHospitalized.Text = Convert.ToBoolean(reader["Hospitalized"]) ? "Yes" : "No";

                    // Allergies
                    List<string> allergies = new List<string>();
                    if (Convert.ToBoolean(reader["LocalAestheticAllergy"])) allergies.Add("Local Anesthetic");
                    if (Convert.ToBoolean(reader["PenicillinAllergy"])) allergies.Add("Penicillin");
                    if (Convert.ToBoolean(reader["SulfaAllergy"])) allergies.Add("Sulfa");
                    if (Convert.ToBoolean(reader["AspirinAllergy"])) allergies.Add("Aspirin");
                    if (Convert.ToBoolean(reader["LatexAllergy"])) allergies.Add("Latex");

                    string otherAllergies = reader["OtherAllergies"].ToString();
                    if (!string.IsNullOrEmpty(otherAllergies)) allergies.Add(otherAllergies);

                    lblAllergies.Text = allergies.Count > 0 ? string.Join(", ", allergies) : "None";

                    // Medications
                    lblTakingMeds.Text = Convert.ToBoolean(reader["TakingPrescriptionMeds"]) ? "Yes" : "No";
                    lblMedicationList.Text = reader["MedicationList"].ToString() ?? "None";

                    // Substances
                    lblUsesTobacco.Text = Convert.ToBoolean(reader["UsesTobacco"]) ? "Yes" : "No";
                    lblUsesAlcoholDrugs.Text = Convert.ToBoolean(reader["UsesAlcoholDrugs"]) ? "Yes" : "No";

                    // Medical Conditions
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

                    // Vitals
                    lblBloodType.Text = reader["BloodType"].ToString() ?? "Not recorded";
                    lblBloodPressure.Text = reader["BloodPressure"].ToString() ?? "Not recorded";
                    lblBleedingTime.Text = reader["BleedingTime"].ToString() ?? "Not recorded";

                    // Women Info
                    if (GetPatientGender() == "Female")
                    {
                        panelWomenInfo.Visible = true;
                        lblPregnant.Text = Convert.ToBoolean(reader["IsPregnant"]) ? "Yes" : "No";
                        lblNursing.Text = Convert.ToBoolean(reader["IsNursing"]) ? "Yes" : "No";
                        lblBirthControl.Text = Convert.ToBoolean(reader["OnBirthControl"]) ? "Yes" : "No";
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

        // ✅ HELPER METHODS
        private string GetPatientName()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT FullName FROM Patients WHERE PatientID = @PatientID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientID", PatientID);
                conn.Open();
                object result = cmd.ExecuteScalar();
                conn.Close();
                return result?.ToString() ?? "";
            }
        }

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

        private void btnViewMedicalHistory_Click(object sender, EventArgs e)
        {
            MedicalHistoryForm mhForm = new MedicalHistoryForm(PatientID);
            mhForm.ShowDialog();
            LoadMedicalHistory(); // Refresh after closing
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